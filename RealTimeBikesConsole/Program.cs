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
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Net;

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
            //var sub = Observable.Interval(TimeSpan.FromSeconds(5))
            //                    .Subscribe(_ =>
            //                         {
            //                             Observable.FromAsync(ReturnStationDetailsAsync)
            //                                 .Subscribe(t =>
            //                             {
            //                                 PrintStations(t);
            //                             });
            //                         });
            //Console.ReadLine();
            //sub.Dispose();

             var obsStations = Observable.Create((IObserver<Station> obsSt) =>
                {
                    var result = ReturnStationDetailsAsync();
                    foreach (var item in result.Result)
                        obsSt.OnNext(item);
                    obsSt.OnCompleted();
                    return Disposable.Create(() => Console.WriteLine("Completed"));
                });

             var interVal = Observable.Interval(TimeSpan.FromSeconds(5))
                 .Subscribe(_ =>
                 {
                     obsStations.Subscribe(t => Console.WriteLine(t.Address));
                 });

            Console.ReadLine();
        }

        static void PrintStations(List<Station> stations)
        {
            foreach (var stationDetail in stations)
            {
                var output = String.Format("Station: {0}, Available Bikes: {1}, Bike Stands: {2}, Available Bike Stands: {3}",
                    stationDetail.Name, stationDetail.Available_Bikes, stationDetail.Bike_Stands, stationDetail.Available_Bike_Stands);
                Console.WriteLine(output);
            }
        }

        static async Task<List<Station>> ReturnStationDetailsAsync()
        {
            List<Station> results = new List<Station>();
            var query = String.Format("?contract=dublin&apiKey={0}", apiKey);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(stations);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(query);

                if (response.IsSuccessStatusCode)
                {
                    results = await response.Content.ReadAsAsync<List<Station>>();
                }
                return results;
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
