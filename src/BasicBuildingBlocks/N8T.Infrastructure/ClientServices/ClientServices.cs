using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;

namespace N8T.Infrastructure.ClientServices
{
    public class ClientServices : IClientServices
    {
        private readonly DaprClient _daprClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientServices(DaprClient daprClient, IHttpClientFactory httpClientFactory)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
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

        public Task<TValue> GetStateAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            return _daprClient.GetStateAsync<TValue>(storeName, key, cancellationToken: cancellationToken);
        }

        public Task<StateEntry<TValue>> GetStateEntryAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            return _daprClient.GetStateEntryAsync<TValue>(storeName, key, cancellationToken: cancellationToken);
        }

        public Task SaveStateAsync<TValue>(string storeName, string key, TValue value, StateOptions stateOptions = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
        {
            return _daprClient.SaveStateAsync(storeName, key, value, cancellationToken: cancellationToken);
        }

        #endregion

    }
}
