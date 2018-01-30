using System;
using System.Linq;

namespace Trabo.Model
{
    public static class DataExtensions
    {
        private static readonly DateTime UnixTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

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

        public static DateTime ToDateTime(long unixtime, bool seconds = false)
        {
            if (seconds)
            {
                return UnixTimeStamp.AddSeconds(unixtime);
            }
            else
            {
                return UnixTimeStamp.AddMilliseconds(unixtime);
            }
        }
    }
}