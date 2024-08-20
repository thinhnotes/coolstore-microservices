using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;

class ClientServices : IClientServices
{
    private readonly DaprClient _daprClient;
    public ClientServices(DaprClient daprClient)
    {
        _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    }

    public Task<TValue> GetStateAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        return _daprClient.GetStateAsync<TValue>(storeName, key, cancellationToken: cancellationToken);
    }

    public Task<StateEntry<TValue>> GetStateEntryAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        return _daprClient.GetStateEntryAsync<TValue>(storeName, key, cancellationToken: cancellationToken);
    }

    public Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, TRequest data, CancellationToken cancellationToken = default)
    {
        return _daprClient.InvokeMethodAsync<TRequest, TResponse>(
                appId, methodName,
                data, cancellationToken: cancellationToken);
    }

    public Task PublishEventAsync<TData>(string pubsubName, string topicName, TData data, CancellationToken cancellationToken = default)
    {
        return _daprClient.PublishEventAsync(pubsubName, topicName, data, cancellationToken);
    }

    public Task SaveStateAsync<TValue>(string storeName, string key, TValue value, StateOptions stateOptions = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        return _daprClient.SaveStateAsync(storeName, key, value, cancellationToken: cancellationToken);
    }
}