using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N8T.Infrastructure.App.Dtos;
using N8T.Infrastructure.Auth;
using N8T.Infrastructure.ClientServices;

namespace ShoppingCartService.Application.GetCartByUserId
{
    public class GetCartByUserIdHandler : IRequestHandler<GetCartByUserIdQuery, CartDto>
    {
        private readonly IClientServices _client;
        private readonly ISecurityContextAccessor _securityContextAccessor;

        public GetCartByUserIdHandler(IClientServices client, ISecurityContextAccessor securityContextAccessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _securityContextAccessor = securityContextAccessor ?? throw new ArgumentNullException(nameof(securityContextAccessor));
        }

        public async Task<CartDto> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _securityContextAccessor.UserId;

            var cart = await _client.GetStateAsync<CartDto>("statestore", $"shopping-cart-{currentUserId}",
                cancellationToken: cancellationToken);

            if (cart is not null)
                return cart;

            cart = new CartDto();
            await _client.SaveStateAsync<CartDto>("statestore", $"shopping-cart-{currentUserId}", cart,
                cancellationToken: cancellationToken);

            return cart;
        }
    }
}
