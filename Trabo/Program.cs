using System;
using System.Threading.Tasks;
using Trabo.Model;

namespace Trabo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var api = new LiveCoinApi("", "keRNy55jkKnute9MtmKZjZcrk9eprvHn");

            while (true)
            {
                //BidAsk dto = await api.GetBidAsk("BTC/USD");
                var ob = (await api.GetOrderbook("BTC/USD", true, 5)).ToOrderBook();
                //Console.WriteLine($"Bid: {dto.maxBid}, Ask: {dto.minAsk}");
            }
        }
    }
}