using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using N8T.Infrastructure.App.Dtos;
using N8T.Infrastructure.App.Requests.Inventory;
using SaleService.Domain.Gateway;

namespace SaleService.Infrastructure.Gateway
{
    public class InventoryGateway : IInventoryGateway
    {
        private readonly IClientServices _client;

        public InventoryGateway(IClientServices _client)
        {
            _client = _client ?? throw new ArgumentNullException(nameof(_client));
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryListAsync(IEnumerable<Guid>? ids = null,
            CancellationToken cancellationToken = default)
        {
            ids ??= new List<Guid>();
            var data = new InventoryByIdsRequest {InventoryIds = ids};

            var inventories = await _client.InvokeMethodAsync<InventoryByIdsRequest, List<InventoryDto>>(
                "inventoryapp", "get-inventories-by-ids",
                data, cancellationToken: cancellationToken);

            return inventories;
        }
    }
}
