using System;
using System.Linq;

namespace Trabo.Model
{
    public class OrderBook : IEquatable<OrderBook>
    {
        public DateTime Time;
        public Bet[] Bids;
        public Bet[] Asks;

        private static decimal GetPriceFor(Bet[] array, decimal amount)
        {
            var amountLeft = amount;
            decimal totalSum = 0;
            int index = 0;
            while (amountLeft > 0 && index < array.Length)
            {
                var volumeToSubstract = Math.Min(amount, array[index].Volume);
                totalSum += array[index].Price * volumeToSubstract;
                amountLeft -= volumeToSubstract;
                index++;
            }

            if (amountLeft > 0)
                throw new InvalidOperationException("Not enough data");


            return totalSum / amount;
        }

        public decimal GetBidFor(decimal amount)
        {
            return GetPriceFor(Bids, amount);
        }

        public decimal GetAskFor(decimal amount)
        {
            return GetPriceFor(Asks, amount);
        }

        public override string ToString()
        {
            return $"{nameof(Time)}: {Time}";
        }

        public bool Equals(OrderBook other)
        {
            return Bids.SequenceEqual(other.Bids) && Asks.SequenceEqual(other.Asks);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OrderBook) obj);
        }

        public override int GetHashCode()
        {
            return 0;
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

        protected bool Equals(Bet other)
        {
            return Volume == other.Volume && Price == other.Price;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Bet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Volume.GetHashCode() * 13) ^ Price.GetHashCode();
            }
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