using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace Trabo
{
    public class LiveCoinApi
    {
        private readonly string _secretKey;
        private readonly string _apiKey;
        private readonly RestClient _client;

        public LiveCoinApi(
            string secretKey,
            string apiKey)
        {
            _secretKey = secretKey;
            _apiKey = apiKey;
            
            _client = new RestClient(@"https://api.livecoin.net/");
        }
        
        public async Task<ExchangeResultDto> SellLimit(string currencyPair, decimal amount, decimal price)
        {
            var dto = await MakeRequest<ExchangeResultDto>("/exchange/selllimit", Method.POST, new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair},
                {"quantity", amount.ToString(CultureInfo.InvariantCulture)},
                {"price", price.ToString(CultureInfo.InvariantCulture)}
            });

            return dto;
        }
        
        public async Task<ExchangeResultDto> BuyLimit(string currencyPair, decimal amount, decimal price)
        {
            var dto = await MakeRequest<ExchangeResultDto>("/exchange/buylimit", Method.POST, new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair},
                {"quantity", amount.ToString(CultureInfo.InvariantCulture)},
                {"price", price.ToString(CultureInfo.InvariantCulture)}
            });

            return dto;
        }
        
        public async Task<ExchangeResultDto> SellMarket(string currencyPair, decimal amount)
        {
            var dto = await MakeRequest<ExchangeResultDto>("/exchange/sellmarket", Method.POST, new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair},
                {"quantity", amount.ToString(CultureInfo.InvariantCulture)}
            });

            return dto;
        }
        
        public async Task<ExchangeResultDto> BuyMarket(string currencyPair, decimal amount)
        {
            var dto = await MakeRequest<ExchangeResultDto>("/exchange/buymarket", Method.POST, new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair},
                {"quantity", amount.ToString(CultureInfo.InvariantCulture)}
            });

            return dto;
        }
        
        public async Task<TradeDto[]> GetLastTrades(string currencyPair)
        {
            var dto = await MakeRequest<TradeDto[]>("/exchange/last_trades", Method.GET,new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair},
                {"minutesOrHour", "false"},
                {"type", "BUY"}
            });

            return dto;
        }

        public async Task<BidAskDto> GetBidAsk(string currencyPair)
        {
            var dto = await MakeRequest<CurrencyPairsDto<BidAskDto>>("/exchange/maxbid_minask", Method.GET,new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair}
            });

            return dto.currencyPairs[0];
        }
        
        public async Task<OrderBookDto> GetOrderbook(
            string currencyPair,
            bool groupByPrice,
            int depth)
        {
            var dto = await MakeRequest<OrderBookDto>("/exchange/order_book", Method.GET, new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair},
                {"groupByPrice", groupByPrice.ToString()},
                {"depth", depth.ToString()}
            });

            return dto;
        }

        private async Task<T> MakeRequest<T>(string method, Method httpMethod, Dictionary<string, string> data)
        {
            var sortedData = data.OrderBy(kv => kv.Key).ToArray();
            var hashString = CalculateHMAC(sortedData);

            var request = new RestRequest(method, httpMethod);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Api-key", _apiKey);
            request.AddHeader("Sign", hashString);

            foreach (var parameter in sortedData)
            {
                request.AddParameter(parameter.Key, parameter.Value, ParameterType.GetOrPost);
            }

            var response = await _client.ExecuteTaskAsync(request);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private string CalculateHMAC(KeyValuePair<string, string>[] sortedData)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));

            string queryString = string.Join("&", sortedData.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}")); 
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));

            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }
    }   
}