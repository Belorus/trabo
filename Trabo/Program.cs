using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using Linq.Extras;
using Trabo.History;
using Trabo.Model;

namespace Trabo
{
    class Program
    {
        static void Main(string[] args)
        {
            // To avoid decimal parse issues
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            var cl = new LineProtocolClient(new Uri("http://localhost:8086"), "trabo");
            var api = new LiveCoinApi("BN1tB4kJdAZASXYvwZNUsHGpgGsmxua6", "keRNy55jkKnute9MtmKZjZcrk9eprvHn");

            var monitor = new ExchangeMonitor(api, "BTC/USD");
            monitor.Start();


            Observable.CombineLatest(
                    monitor.MovingAverage,
                    monitor.Delta,
                    monitor.Bets,
                    (avg, tup, bet) =>
                        ($"SMA: {avg:C0} Bet: {bet.MaxBid:F0}/{bet.MinAsk:F0} D: {tup.Item1:P1}/{tup.Item2:P1}"))
                .Subscribe(s => Console.WriteLine(s));
            
            Console.ReadLine();
        }

        private static async Task SavetoDb(LiveCoinApi api)
        {
//            while (true)
//            {
//                var model = (await api.GetBidAsk("BTC/USD")).ToBidAsk();
//                var trades = (await api.GetLastTrades("BTC/USD")).Select(d => d.ToTrade()).ToArray();
//
//                var payload = new LineProtocolPayload();
//                payload.Add(
//                    new LineProtocolPoint("price", new Dictionary<string, object>()
//                    {
//                        {"maxBid", model.MaxBid},
//                        {"minAsk", model.MinAsk},
//                    }, utcTimestamp: DateTime.UtcNow));
//
//                foreach (var t in trades)
//                {
//                    movingAvarage.Push((double) t.Price);
//                    payload.Add(
//                        new LineProtocolPoint("trades", new Dictionary<string, object>()
//                        {
//                            {"price", t.Price},
//                            {"quantity", t.Quantity},
//                            {"movingAvarage", movingAvarage.Mean}
//                        }, utcTimestamp: t.Time));
//                }
//            }
        }

        static async Task ReadHistoricalData()
        {
            var reader = new HistoricalDataReader();
            var data = reader.ReadData(@"C:\STUFF\Data\coinbaseUSD_1-min_data_2014-12-01_to_2018-01-08.csv");
            var cl = new LineProtocolClient(new Uri("http://localhost:8086"), "history");

            foreach (var items in data.Batch(50))
            {
                var payload = new LineProtocolPayload();
                foreach (var item in items)
                {
                    payload.Add(
                        new LineProtocolPoint("price", new Dictionary<string, object>()
                        {
                            {"open", item.Open},
                            {"close", item.Close},
                            {"low", item.Low},
                            {"high", item.Hight}
                        }, utcTimestamp: item.Date.ToUniversalTime()));
                }

                await cl.WriteAsync(payload);
            }
        }

        static async void PerformSomeApi()
        {
            var api = new LiveCoinApi("", "keRNy55jkKnute9MtmKZjZcrk9eprvHn");

            while (true)
            {
                //BidAsk dto = await api.GetBidAsk("BTC/USD");
                // var ob = (await api.GetOrderbook("BTC/USD", true, 5)).ToOrderBook();

                //await api.BuyLimit("BTC/USD", 0.001m, 0.001m);

                //Console.WriteLine($"Bid: {dto.maxBid}, Ask: {dto.minAsk}");
            }
        }
    }
}