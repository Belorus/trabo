using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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

        public IObservable<BidAsk> Bets { get; set; }
        public IObservable<decimal> MovingAverage { get; set; }
        public IObservable<Trade> Trades { get; private set; }
        public IObservable<(decimal, decimal)> Delta { get; set; }

        public void Start()
        {
            Trades = Observable.Interval(TimeSpan.FromSeconds(3))
                .SelectMany(time => _api.GetLastTrades(_currencyPair).ToObservable())
                .SelectMany(dtos => dtos.Select(d => d.ToTrade()).OrderBy(x => x.Time))
                .Distinct(x => x.Id);

            Bets = Observable.Interval(TimeSpan.FromSeconds(3))
                .SelectMany(time => _api.GetBidAsk(_currencyPair).ToObservable())
                .Select(dto => dto.ToBidAsk());

            MovingAverage = Trades.Select(t => t.Price)
                .Buffer(250, 1)
                .Select(o => o.Average());

            Delta = Bets.Zip(MovingAverage,
                (ask, avg) => ((avg - ask.MinAsk) / avg, (ask.MaxBid - avg) / avg));
        }
    }
}