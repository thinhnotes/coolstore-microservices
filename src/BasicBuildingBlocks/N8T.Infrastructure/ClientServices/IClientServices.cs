using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
namespace N8T.Infrastructure.ClientServices
{
    public interface IClientServices
    {
        Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, TRequest data, CancellationToken cancellationToken = default(CancellationToken));

        Task<TValue> GetStateAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<StateEntry<TValue>> GetStateEntryAsync<TValue>(string storeName, string key, ConsistencyMode? consistencyMode = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveStateAsync<TValue>(string storeName, string key, TValue value, StateOptions stateOptions = null, IReadOnlyDictionary<string, string> metadata = null, CancellationToken cancellationToken = default(CancellationToken));
        Task PublishEventAsync<TData>(string pubsubName, string topicName, TData data, CancellationToken cancellationToken = default(CancellationToken));
    }
}
