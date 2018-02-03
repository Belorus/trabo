using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using Accord.Statistics.Moving;
using Trabo.Model;

namespace Trabo
{
    internal class ExchangeMonitor
    {
        private readonly ILiveCoinApi _api;
        private readonly string _currencyPair;

        public ExchangeMonitor(
            ILiveCoinApi api,
            string currencyPair)
        {
            _api = api;
            _currencyPair = currencyPair;
        }

        public IObservable<decimal> MovingAverage { get; set; }
        public IObservable<Trade> Trades { get; private set; }
        public IObservable<OrderBook> OrderBook { get; private set; }

        private MovingNormalStatistics movingAvarage;

        public void Start()
        {
            Trades = new TaskRepeatObservable().Create(() => _api.GetLastTrades(_currencyPair), CancellationToken.None)
                .SelectMany(dtos => dtos.Select(d => d.ToTrade()).OrderBy(x => x.Time))
                .Distinct(x => x.Id);

            OrderBook = new TaskRepeatObservable().Create(() => _api.GetOrderbook(_currencyPair, true, 1), CancellationToken.None)
                .Select(ob => ob.ToOrderBook());


            movingAvarage = new Accord.Statistics.Moving.MovingNormalStatistics(250);
            MovingAverage = Trades.Select(t => t.Price).Buffer(10, 1).Select(l => l.Average());
        }
    }
}