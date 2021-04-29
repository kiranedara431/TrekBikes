using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace TrekBikes
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            string uri = "https://trekhiringassignments.blob.core.windows.net/interview/bikes.json";

            HttpResponseMessage response = httpClient.GetAsync(uri).Result;

            var bikeDict = new Dictionary<string, int>();

            if (response.IsSuccessStatusCode)
            {
                var jsonData = response.Content.ReadAsStringAsync().Result;
                var lstBikes = JsonConvert.DeserializeObject<List<HouseholdResponse>>(jsonData);

                if (lstBikes.Any())
                {
                    foreach (var objBikes in lstBikes)
                    {
                        var arrBikes = objBikes.bikes.OrderBy(x=>x).ToArray();
                        var str = String.Join(",", arrBikes);
                        if (bikeDict.ContainsKey(str))
                            bikeDict[str] += 1;
                        else
                            bikeDict.Add(str, 1);
                    }
                }

                var top20bikes = (from bike in bikeDict orderby bike.Value descending select bike).Take(20).ToDictionary(x => x.Key, x => x.Value);
                Console.WriteLine("The Top 20 Most Popular Responses are:");
                Console.WriteLine($"{"Bike Combination".PadRight(50, ' ')}Number of Families Owned");
                Console.WriteLine($"{"----------------".PadRight(50,' ')}------------------------");
                foreach (var dict in top20bikes)
                    Console.WriteLine($"{dict.Key.PadRight(50, ' ')}{dict.Value}");
            }
            else
                Console.WriteLine($"Exception {response.StatusCode}");

            Console.ReadLine();
        }
    }

    public class HouseholdResponse
    {
        public IList<string> bikes { get; set; }
    }
}
