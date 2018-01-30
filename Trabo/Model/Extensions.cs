using System;
using System.Linq;

namespace Trabo.Model
{
    public static class DataExtensions
    {
        public static OrderBook ToOrderBook(this OrderBookDto dto)
        {
            return new OrderBook()
            {
                Time = ToDateTime(dto.timestamp),
                Asks = dto.asks.Select(arr => new Bet(){Price = (decimal)arr[0], Volume =  (decimal)arr[1]}).ToArray(),
                Bids = dto.bids.Select(arr => new Bet(){Price = (decimal)arr[0], Volume =  (decimal)arr[1]}).ToArray(),
            };
        }

        public static BidAsk ToBidAsk(this BidAskDto dto)
        {
            return new BidAsk()
            {
                MaxBid = (decimal) dto.maxBid,
                MinAsk = (decimal) dto.minAsk
            };
        }

        public static DateTime ToDateTime(long unixtime)
        {
            return new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixtime);
        }
    }
}