using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<BidAskDto> GetBidAsk(string currencyPair)
        {
            var response = await GetRequest("/exchange/maxbid_minask", new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair}
            });

            var dto = JsonConvert.DeserializeObject<CurrencyPairsDto<BidAskDto>>(response.Content);

            return dto.currencyPairs[0];
        }
        
        public async Task<OrderBookDto> GetOrderbook(
            string currencyPair,
            bool groupByPrice,
            int depth)
        {
            var response = await GetRequest("/exchange/order_book", new Dictionary<string, string>()
            {
                {"currencyPair", currencyPair},
                {"groupByPrice", groupByPrice.ToString()},
                {"depth", depth.ToString()}
            });

            var dto = JsonConvert.DeserializeObject<OrderBookDto>(response.Content);

            return dto;
        }

        private async Task<IRestResponse> GetRequest(string method, Dictionary<string, string> data)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(new byte[] { }); //TODO:
            var hashString = BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();

            var sortedData = data.OrderBy(kv => kv.Key).ToArray();

            var request = new RestRequest(method, Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Api-key", _apiKey);
            request.AddHeader("Sign", hashString);

            foreach (var parameter in sortedData)
            {
                request.AddParameter(parameter.Key, parameter.Value, ParameterType.QueryString);
            }

            return await _client.ExecuteTaskAsync(request);
        }
    }

    
}