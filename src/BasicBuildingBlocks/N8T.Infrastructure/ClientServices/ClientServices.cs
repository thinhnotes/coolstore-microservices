using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace N8T.Infrastructure.ClientServices
{
    public class ClientServices : IClientServices
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnectionMultiplexer _redis;

        public ClientServices(IHttpClientFactory httpClientFactory, IConnectionMultiplexer redis)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        }

        #region Request Reponse

        public async Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, TRequest data, CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");
            var response = await httpClient.PostAsync($"http://{appId}/{methodName}", jsonContent, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return default(TResponse);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(content);
        }

        #endregion

        #region Event

        public Task PublishEventAsync<TData>(string pubsubName, string topicName, TData data, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //return _daprClient.PublishEventAsync(pubsubName, topicName, data, cancellationToken);
        }

        #endregion

        #region State

        public async Task<TValue> GetStateAsync<TValue>(string storeName, string key, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            string value = await db.StringGetAsync(key);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(value);
        }

        public async Task SaveStateAsync<TValue>(string storeName, string key, TValue value, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, JsonConvert.SerializeObject(value));
        }

        #endregion

    }
}
