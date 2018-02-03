using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Trabo
{
    public interface ILiveCoinApi
    {
        Task<TradeDto[]> GetLastTrades(string currencyPair);
        Task<BidAskDto> GetBidAsk(string currencyPair);
        Task<OrderBookDto> GetOrderbook(string currencyPair, bool groupByPrice, int depth);
    }
}