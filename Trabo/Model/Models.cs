using System;

namespace Trabo.Model
{
    public class OrderBook
    {
        public DateTime Time;
        public Bet[] Bids;
        public Bet[] Asks;

        public override string ToString()
        {
            return $"{nameof(Time)}: {Time}";
        }
    }

    public class Trade
    {
        public DateTime Time;
        public long Id;
        public decimal Price;
        public decimal Quantity;
        public string Type;

        public override string ToString()
        {
            return $"{nameof(Time)}: {Time}, {nameof(Price)}: {Price}, {nameof(Quantity)}: {Quantity}";
        }
    }

    public class Bet
    {
        public decimal Volume;
        public decimal Price;

        public override string ToString()
        {
            return $"{nameof(Volume)}: {Volume}, {nameof(Price)}: {Price}";
        }
    }

    public class BidAsk
    {
        public decimal MaxBid;
        public decimal MinAsk;

        public override string ToString()
        {
            return $"{nameof(MaxBid)}: {MaxBid}, {nameof(MinAsk)}: {MinAsk}";
        }
    }
    
}