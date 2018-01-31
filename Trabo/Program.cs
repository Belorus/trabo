using System;
using System.Collections.Generic;
using System.Globalization;
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
        static async Task Main(string[] args)
        {
            // To avoid decimal parse issues
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            
            await ReadHistoricalData();
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