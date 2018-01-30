namespace Trabo
{
    
    public class CurrencyPairsDto<T>
    {
        public T[] currencyPairs;
    }
    
    public class BidAskDto
    {
        public string symbol;
        public double maxBid;
        public double minAsk;
    }

    public class OrderBookDto
    {
        public long timestamp;
        public double[][] asks;
        public double[][] bids;
    }

    public class ExchangeResultDto
    {
        public bool success;
        public bool added;
        public string exception;
        public string orderId;
    }
}