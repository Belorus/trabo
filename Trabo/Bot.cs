using System;
using Accord.Statistics.Moving;
using Trabo.Model;

namespace Trabo
{
    internal class Bot
    {
        public readonly MovingNormalStatistics MovingAverage = new MovingNormalStatistics(250);

        public decimal Delta { get; private set; }
        public decimal DeltaWithComission { get; private set; }

        public OrderBook LastOrderbook { get; private set; }

        public event Action Updated = delegate { };

        public void Start()
        {
            var api = new LiveCoinApi("BN1tB4kJdAZASXYvwZNUsHGpgGsmxua6", "keRNy55jkKnute9MtmKZjZcrk9eprvHn");
            var monitor = new ExchangeMonitor(api, "BCH/BTC");

            monitor.Start();
            monitor.Trades.Subscribe(OnTrade);
            monitor.OrderBook.Subscribe(OnOrderbooksUpdate);
        }

        private void OnOrderbooksUpdate(OrderBook ob)
        {
            if (MovingAverage.Count > 0)
            {
                Delta = 100 * ((decimal) MovingAverage.Mean - ob.Asks[0].Price) / (decimal) MovingAverage.Mean;
                DeltaWithComission = 100 * ((decimal)MovingAverage.Mean - ob.Asks[0].Price * 1.0036m) / (decimal)MovingAverage.Mean;
            }

            LastOrderbook = ob;

            OnUpdated();
        }

        private void OnUpdated()
        {
            if (LastOrderbook != null && MovingAverage.Count > 0)
            {
                Updated();
            }
        }

        private void OnTrade(Trade trade)
        {
            MovingAverage.Push((double) trade.Price);
            OnUpdated();
        }
    }
}