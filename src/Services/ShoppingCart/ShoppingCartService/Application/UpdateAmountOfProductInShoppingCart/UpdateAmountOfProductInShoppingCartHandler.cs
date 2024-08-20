using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N8T.Domain;
using N8T.Infrastructure.App.Dtos;
using N8T.Infrastructure.Auth;
using N8T.Infrastructure.ClientServices;
using ShoppingCartService.Domain.Gateway;
using ShoppingCartService.Infrastructure.Extensions;

namespace ShoppingCartService.Application.UpdateAmountOfProductInShoppingCart
{
    public class UpdateAmountOfProductInShoppingCartHandler : IRequestHandler<UpdateAmountOfProductInShoppingCartQuery, CartDto>
    {
        private readonly IClientServices _client;
        private readonly IProductCatalogGateway _productCatalogGateway;
        private readonly IPromoGateway _promoGateway;
        private readonly IShippingGateway _shippingGateway;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public UpdateAmountOfProductInShoppingCartHandler(IClientServices _client,
            IProductCatalogGateway productCatalogGateway,
            IPromoGateway promoGateway,
            IShippingGateway shippingGateway,
            ISecurityContextAccessor securityContextAccessor)
        {
            _client = _client ?? throw new ArgumentNullException(nameof(_client));
            _productCatalogGateway = productCatalogGateway ?? throw new ArgumentNullException(nameof(productCatalogGateway));
            _promoGateway = promoGateway ?? throw new ArgumentNullException(nameof(promoGateway));
            _shippingGateway = shippingGateway ?? throw new ArgumentNullException(nameof(shippingGateway));
            _securityContextAccessor = securityContextAccessor ?? throw new ArgumentNullException(nameof(securityContextAccessor));
        }

        public async Task<CartDto> Handle(UpdateAmountOfProductInShoppingCartQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _securityContextAccessor.UserId;

            var cart = await _client.GetStateAsync<CartDto>("statestore", $"shopping-cart-{currentUserId}",
                cancellationToken: cancellationToken);

            if (cart is null)
            {
                throw new CoreException($"Couldn't find cart for user_id={currentUserId}");
            }

            var cartItem = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

            // if not exists then it should be a new item
            if (cartItem is null)
            {
                await cart.InsertItemToCartAsync(request.Quantity, request.ProductId, _productCatalogGateway);
            }
            else
            {
                cartItem.Quantity += request.Quantity;
            }

            await cart.CalculateCartAsync(_productCatalogGateway, _shippingGateway, _promoGateway);

            await _client.SaveStateAsync<CartDto>("statestore", $"shopping-cart-{currentUserId}", cart,
                cancellationToken: cancellationToken);

            return cart;
        }
    }
}
