﻿using System;
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

            var cl = new LineProtocolClient(new Uri("http://localhost:8086"), "bitstamp");

            var api = new LiveCoinApi("BN1tB4kJdAZASXYvwZNUsHGpgGsmxua6", "keRNy55jkKnute9MtmKZjZcrk9eprvHn");
            //using (var api = new CsvApi(@"D:\bitcoin-historical-data\bitstampUSD_1-min_data_2012-01-01_to_2018-01-08.csv"))
            {
                var monitor = new ExchangeMonitor(api, "BCH/BTC");
                monitor.Start();

                Observable.CombineLatest(
                        monitor.MovingAverage,
                        monitor.OrderBook,
                        (avg, ob) => new { avg, ob, str = $"{ob.Time}: Orderbook" })
                        .Subscribe(an =>
                        {
                            var payload = new LineProtocolPayload();
                            payload.Add(
                                new LineProtocolPoint("stat", new Dictionary<string, object>()
                                {
                                    {"dt", (an.ob.Bids[0].Price - an.avg) / an.avg}
                                                
                                }, utcTimestamp: an.ob.Time));

                            cl.WriteAsync(payload).Wait();
                        });

                Observable.CombineLatest(
                        monitor.MovingAverage,
                        monitor.Trades,
                        (avg, tr) => new { avg, tr, str = ($"{tr.Time}  SMA: {avg:C4} Price: {tr.Price}") })
                    .Subscribe(d =>
                    {
                        Console.WriteLine(d.str);
                        var payload = new LineProtocolPayload();
                        payload.Add(
                            new LineProtocolPoint("trade", new Dictionary<string, object>()
                            {
                                                {"avg", d.avg},
                                                {"price", d.tr.Price},
                            }, utcTimestamp: d.tr.Time));

                        cl.WriteAsync(payload).Wait();
                        //.Subscribe(s => Console.WriteLine(s));
                    });
                Console.ReadLine();    
            }
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
            using (var reader = new HistoricalDataReader())
            {
                reader.OpenData(@"C:\STUFF\Data\coinbaseUSD_1-min_data_2014-12-01_to_2018-01-08.csv");
                var data = reader.Data();
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
                            }, utcTimestamp: DataExtensions.ToDateTime(item.Date, seconds: true)));
                    }

                    await cl.WriteAsync(payload);
                }
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