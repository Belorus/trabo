using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trabo.History;

namespace Trabo
{
    public class CsvApi : ILiveCoinApi, IDisposable
    {
        private readonly string _path;
        private readonly HistoricalDataReader _reader;
        private readonly IEnumerator<HistoricalDataModel> _dataEnumerable;
        private HistoricalDataModel _lastData;

        private bool _lastTradeReaded = true;
        private bool _bidAskReaded = true;
        private long _id;
        
        private readonly object _lock = new object();
        
        public CsvApi(string path)
        {
            _path = path;
            
            _reader = new HistoricalDataReader();
            _reader.OpenData(path);
            _dataEnumerable = _reader.Data().GetEnumerator();
        }
        
        public Task<TradeDto[]> GetLastTrades(string currencyPair)
        {
            _lastTradeReaded = true;
            var data = GetData();

            if (data != null)
            {
                var dto = new TradeDto
                {
                    id = _id++,
                    price = data.Low + (data.Hight - data.Low) / 2,
                    quantity = data.BTCVolume,
                    time = data.Date * 1000,
                    type = "type"
                };
                return Task.FromResult(new TradeDto[] {dto});
            }
            else
            {
                var tcs = new TaskCompletionSource<TradeDto[]>();
                return tcs.Task;
            }
        }

        public Task<BidAskDto> GetBidAsk(string currencyPair)
        {
            _bidAskReaded = true;
            var data = GetData();

            if (data != null)
            {
                var dto = new BidAskDto
                {
                    maxBid = (data.Low + (data.Hight - data.Low) / 2) * 0.999,
                    minAsk = (data.Low + (data.Hight - data.Low) / 2) * 1.001,
                    symbol = "symbol"
                };

                return Task.FromResult(dto);
            }
            else
            {
                var tsc = new TaskCompletionSource<BidAskDto>();
                return tsc.Task;
            }
        }

        private HistoricalDataModel GetData()
        {
            lock (_lock)
            {
                if (_lastTradeReaded && _bidAskReaded)
                {
                    if (_dataEnumerable.MoveNext())
                    {
                        _lastData = _dataEnumerable.Current;
                    }
                    else
                    {
                        _lastData = null;
                    }

                    _lastTradeReaded = false;
                    _bidAskReaded = false;
                }

                return _lastData;
            }
        }
        
        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}