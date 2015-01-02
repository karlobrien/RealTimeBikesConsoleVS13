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
using System.Threading;

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
            var query = String.Format("?contract=dublin&apiKey={0}", apiKey);

            /*
            var sub = Observable.Interval(TimeSpan.FromSeconds(5))
                                .Subscribe(_ =>
                                     {
                                         Observable.FromAsync(c => BikesQueryAsync<Station>(stations, query))
                                             .Subscribe(t =>
                                         {
                                             PrintStations(t);
                                         });
                                     });
            Console.ReadLine();
            sub.Dispose();

            var sub2 = Observable.Interval(TimeSpan.FromSeconds(5))
                .SelectMany(_ => Observable.FromAsync(c => BikesQueryAsync<Station>(stations, query)))
                .Subscribe(data => PrintStations(data));
                        */
            var sub3 = Observable.Interval(TimeSpan.FromSeconds(5))
                .Select(_ => Observable.FromAsync(c => PollBikeApiAsync<Station>(stations, query)))
                .Switch()
                .Subscribe(data => PrintStations(data));

             /*var interVal = Observable.Interval(TimeSpan.FromSeconds(5))
                 .Subscribe(_ =>
                 {
                     StationQuery().Subscribe(t => Console.WriteLine(t.Available_Bikes));
                 });
            */
            Console.ReadLine();
        }

        static IObservable<Station> StationQuery()
        {
            var obsStations = Observable.Create((IObserver<Station> obsSt) =>
            {
                var query = String.Format("?contract=dublin&apiKey={0}", apiKey);

                var result = PollBikeApiAsync<Station>(stations, query);

                foreach (var item in result.Result)
                    obsSt.OnNext(item);
                obsSt.OnCompleted();
                return Disposable.Create(() => Console.WriteLine("Completed"));
            });
            return obsStations;
        }

        public static async Task<List<T>> PollBikeApiAsync<T>(string baseAddress, string query)
        {
            var results = new List<T>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    results = await response.Content.ReadAsAsync<List<T>>();
                }
                return results;
            }
        }

        public static async Task<List<Station>> ReturnStationDetailsAsync()
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

        static void PrintStations(List<Station> stations)
        {
            foreach (var stationDetail in stations)
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
