using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N8T.Infrastructure.App.Dtos;
using N8T.Infrastructure.App.Events.ShoppingCart;
using N8T.Infrastructure.Auth;
using N8T.Infrastructure.ClientServices;

namespace ShoppingCartService.Application.Checkout
{
    public class CheckOutHandler : IRequestHandler<CheckOutQuery, CartDto>
    {
        private readonly IClientServices _client;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public CheckOutHandler(IClientServices client, ISecurityContextAccessor securityContextAccessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _securityContextAccessor = securityContextAccessor ?? throw new ArgumentNullException(nameof(securityContextAccessor));
        }

        public async Task<CartDto> Handle(CheckOutQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _securityContextAccessor.UserId;

            var cart = await _client.GetStateAsync<CartDto>("statestore", $"shopping-cart-{currentUserId}",
                cancellationToken: cancellationToken);

            cart.UserId = currentUserId;
            var @event = new ShoppingCartCheckedOut { Cart = cart };
            await _client.PublishEventAsync("pubsub", "processing-order", @event, cancellationToken);

            cart = new CartDto();
            await _client.SaveStateAsync("statestore", "shopping-cart-{currentUserId}", cart,
                cancellationToken: cancellationToken);

            return cart;
        }
    }
}
