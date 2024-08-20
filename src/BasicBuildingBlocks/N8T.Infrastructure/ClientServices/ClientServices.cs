using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Newtonsoft.Json;
using StackExchange.Redis;
using static Google.Rpc.Context.AttributeContext.Types;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace N8T.Infrastructure.ClientServices
{
    public class ClientServices : IClientServices
    {
        private readonly DaprClient _daprClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnectionMultiplexer _redis;

        public ClientServices(DaprClient daprClient, IHttpClientFactory httpClientFactory, IConnectionMultiplexer redis)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
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
            var response = await httpClient.PostAsync($"{appId}/{methodName}", jsonContent, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return default(TResponse);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(content);
        }

        #endregion

        #region Event

        public Task PublishEventAsync<TData>(string pubsubName, string topicName, TData data, CancellationToken cancellationToken = default)
        {
            return _daprClient.PublishEventAsync(pubsubName, topicName, data, cancellationToken);
        }

        #endregion

        #region State

        public async Task<TValue> GetStateAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            string value = await db.StringGetAsync(key);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(value);
        }

        public Task<StateEntry<TValue>> GetStateEntryAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            return _daprClient.GetStateEntryAsync<TValue>(storeName, key, cancellationToken: cancellationToken);
        }

        public async Task SaveStateAsync<TValue>(string storeName, string key, TValue value, StateOptions stateOptions = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, JsonConvert.SerializeObject(value));
        }

        #endregion

    }
}
