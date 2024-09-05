using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using N8T.Domain;
using N8T.Infrastructure.App.Dtos;
using N8T.Infrastructure.App.Requests.ProductCatalog;
using N8T.Infrastructure.ClientServices;
using ShoppingCartService.Domain.Gateway;

namespace ShoppingCartService.Infrastructure.Gateway
{
    public class ProductCatalogGateway : IProductCatalogGateway
    {
        private readonly IClientServices _client;
        private readonly ILogger<ProductCatalogGateway> _logger;

        public ProductCatalogGateway(IClientServices client, ILogger<ProductCatalogGateway> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{Prefix}: GetProductByIdAsync by id={Id}", nameof(ProductCatalogGateway), id);

            var product = await _client.GetStateAsync<ProductDto>("statestore", $"product-{id}", cancellationToken: cancellationToken);
            if (product is not null)
                return product;

            var requestData = new ProductByIdRequest { Id = id };
            product = await _client.InvokeMethodAsync<ProductByIdRequest, ProductDto>(
                "productcatalogapp", "get-product-by-id", requestData, cancellationToken: cancellationToken);

            if (product is null)
            {
                throw new CoreException($"Couldn't find out product with id={id}");
            }

            await _client.SaveStateAsync("statestore", $"product-{id}", product, cancellationToken: cancellationToken);

            return product;
        }
    }
}
