using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
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

        public IObservable<Trade> Trades { get; private set; }
        public IObservable<OrderBook> OrderBook { get; private set; }

        public void Start()
        {
            Trades = new TaskRepeatObservable().Create(() => _api.GetLastTrades(_currencyPair), CancellationToken.None)
                .SelectMany(dtos => dtos.Select(d => d.ToTrade()).OrderBy(x => x.Time))
                .Distinct(x => x.Id)
                .Publish()
                .RefCount();

            OrderBook = new TaskRepeatObservable()
                .Create(() => _api.GetOrderbook(_currencyPair, true, 1), CancellationToken.None)
                .Select(ob => ob.ToOrderBook())
                .Publish()
                .RefCount();
        }
    }
}