using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using N8T.Infrastructure.ClientServices;

namespace ShoppingCartService.Api.Subscriber
{
    [ApiController]
    [Route("")]
    public class ProductSubscriber : ControllerBase
    {
        private readonly IClientServices _client;
        private readonly ILogger<ProductSubscriber> _logger;

        public ProductSubscriber(IClientServices client, ILogger<ProductSubscriber> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
