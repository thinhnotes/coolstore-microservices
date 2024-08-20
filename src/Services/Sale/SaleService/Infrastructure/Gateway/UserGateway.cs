using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using N8T.Infrastructure.App.Dtos;
using N8T.Infrastructure.App.Requests.Identity;
using N8T.Infrastructure.ClientServices;
using SaleService.Domain.Gateway;

namespace SaleService.Infrastructure.Gateway
{
    public class UserGateway : IUserGateway
    {
        private readonly IClientServices _client;
        private readonly ILogger<UserGateway> _logger;

        public UserGateway(IClientServices client, ILogger<UserGateway> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> GetUserInfo(string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("{Prefix}: GetUserInfo by id={Id}", nameof(UserGateway), userId);

            var requestData = new UserByIdRequest { UserId = userId };
            return await _client.InvokeMethodAsync<UserByIdRequest, UserDto>(
                "identityapp", "get-user-by-id", requestData, cancellationToken: cancellationToken);
        }
    }
}
