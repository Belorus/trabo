using System;
using Accord.Statistics.Moving;
using Trabo.Model;

namespace Trabo
{
    internal class Bot
    {
        public const decimal Comission = 1.0018m;

        public readonly MovingNormalStatistics MovingAverage = new MovingNormalStatistics(250);

        public decimal Delta { get; private set; }
        public decimal DeltaWithComission { get; private set; }

        public OrderBook LastOrderbook { get; private set; }

        public event Action Updated = delegate { };

        private decimal _btcAmount = 1000m;
        private decimal _bchAmount = 1000m;
        private TradeOperation _currentOperation;

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
                var approximateBuyPrice = (ob.Asks[0].Price + ob.Bids[0].Price) / 2;
                Delta = 100 * ((decimal) MovingAverage.Mean - approximateBuyPrice) / (decimal) MovingAverage.Mean;
                DeltaWithComission = 100 * ((decimal)MovingAverage.Mean - approximateBuyPrice * 1.0036m) / (decimal)MovingAverage.Mean;
            }

            LastOrderbook = ob;

            OnUpdated();
        }

        private void OnUpdated()
        {
            if (LastOrderbook != null && MovingAverage.Count > 0)
            {
                Updated();

                if (_currentOperation == null)
                {
                    if (DeltaWithComission > 1.3m)
                    {
                        Console.WriteLine("Buying...");

                        var amountToBuy = 1m;
                        _currentOperation = new TradeOperation()
                        {
                            Price = LastOrderbook.GetAskFor(amountToBuy),
                            PriceWithCommission = LastOrderbook.GetAskFor(amountToBuy) * Comission,
                            Amount = amountToBuy
                        };

                        _bchAmount += amountToBuy;
                        _btcAmount -= _currentOperation.PriceWithCommission * amountToBuy;

                        Console.WriteLine($"Bought: {_currentOperation}");
                        Console.WriteLine($"BTC: {_btcAmount}");
                        Console.WriteLine($"BCH: {_bchAmount}");
                    }
                }
                else
                {
                    var bestSellPriceWithComission = LastOrderbook.GetBidFor(_currentOperation.Amount) / Comission;
                    if (bestSellPriceWithComission / _currentOperation.PriceWithCommission > 1.02m)
                    {
                        Console.WriteLine("Selling...");
                        _bchAmount -= _currentOperation.Amount;
                        _btcAmount += bestSellPriceWithComission * _currentOperation.Amount;

                        _currentOperation = null;

                        Console.WriteLine("Sold!");
                        Console.WriteLine($"BTC: {_btcAmount}");
                        Console.WriteLine($"BCH: {_bchAmount}");
                    }
                }
            }
        }

        private void OnTrade(Trade trade)
        {
            MovingAverage.Push((double) trade.Price);
            OnUpdated();
        }
    }
}