﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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

        public IObservable<decimal> MovingAverage { get; set; }
        public IObservable<Trade> Trades { get; private set; }
        public IObservable<OrderBook> OrderBook { get; private set; }
        public void Start()
        {
            Trades = new TaskRepeatObservable().Create(() => _api.GetLastTrades(_currencyPair), CancellationToken.None)
                .SelectMany(dtos => dtos.Select(d => d.ToTrade()).OrderBy(x => x.Time))
                .Distinct(x => x.Id);

            OrderBook = new TaskRepeatObservable().Create(() => _api.GetOrderbook(_currencyPair, true, 1), CancellationToken.None)
                .Select(ob => ob.ToOrderBook());


            var movingAvarage = new Accord.Statistics.Moving.MovingNormalStatistics(250);
            MovingAverage = Trades.Select(t => {
                double p = (double)t.Price;
                movingAvarage.Push(p);
                return (decimal)movingAvarage.Mean;
                }
            );
                //.Buffer(250, 1)
                //.Select(o => o.Average());

            Delta = Bets.Zip(MovingAverage,
                (ask, avg) => ((avg - ask.MinAsk) / avg, (ask.MaxBid - avg) / avg));
        }
    }
}