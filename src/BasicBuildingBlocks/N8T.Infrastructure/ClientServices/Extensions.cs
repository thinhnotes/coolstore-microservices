using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace N8T.Infrastructure.ClientServices
{
    public static class Extensions
    {
        public static IServiceCollection AddCustomClientServices(this IServiceCollection services, string connStringRedis)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            services.AddSingleton(options);

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(connStringRedis);
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddScoped<IClientServices, ClientServices>();

            return services;
        }
    }
}
