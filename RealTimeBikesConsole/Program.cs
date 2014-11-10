using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

        static void Main()
        {
            RunClient().Wait();
            GetContractList().Wait();
            StationDetails.ForEach(t => Console.WriteLine(t.Name, t.AvailableBikes));
        }

        static async Task RunClient()
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

        static async Task GetContractList()
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
