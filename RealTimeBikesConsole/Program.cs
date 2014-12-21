using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using CommandLine;
using Newtonsoft.Json;

namespace RealTimeBikesConsole
{
    class Program
    {   
        public static string contracts = ConfigurationManager.AppSettings["Contracts"];
        public static string stations = ConfigurationManager.AppSettings["Stations"];
        public static string apiKey = ConfigurationManager.AppSettings["ApiKey"];
        
        public static List<Station> StationDetails { get; set; }
        public static List<ContractsDO> ContractsDo { get; set; }

        public static string Query { get; set; }

        static void Main(string[] args)
        {
            GetStationDetailsAsync().Wait();
            //StationDetails = GetSyncStations();

            foreach (var stationDetail in StationDetails)
            {
                var output = String.Format("Station: {0}, Available Bikes: {1}, Bike Stands: {2}, Available Bike Stands: {3}",
                    stationDetail.Name, stationDetail.Available_Bikes, stationDetail.Bike_Stands, stationDetail.Available_Bike_Stands);
                Console.WriteLine(output);
            }
        }

        static List<Station> GetSyncStations()
        {
            var queryResult = new List<Station>();
            var query = String.Format("?contract=dublin&apiKey={0}", apiKey);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(stations);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(query).Result;

                if (response.IsSuccessStatusCode)
                {
                    queryResult = response.Content.ReadAsAsync<List<Station>>().Result;
                }
            }
            return queryResult;
        }

        static async Task GetStationDetailsAsync()
        {
            var query = String.Format("?contract=dublin&apiKey={0}", apiKey);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(stations);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(query);
                
                if (response.IsSuccessStatusCode)
                {
                    StationDetails = await response.Content.ReadAsAsync<List<Station>>();
                }
            }
        }

        static async Task GetContractListAsync()
        {
            var query = String.Format("?apiKey={0}", apiKey);
      
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(contracts);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(query);

                if (response.IsSuccessStatusCode)
                {
                    ContractsDo = await response.Content.ReadAsAsync<List<ContractsDO>>();
                }
            }
        }

    }


    
}
