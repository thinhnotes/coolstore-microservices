using Microsoft.AspNetCore.Routing;

namespace N8T.Infrastructure.Endpoint
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
