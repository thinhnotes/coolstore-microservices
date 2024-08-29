using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace N8T.Infrastructure.ClientServices
{
    public static class Extensions
    {
        public static IHostApplicationBuilder AddCustomClientServices(this IHostApplicationBuilder builder, string connName)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            builder.Services.AddSingleton(options);

            builder.Services.AddHttpClient();

            builder.AddRedisClient(connName);

            builder.Services.AddScoped<IClientServices, ClientServices>();

            return builder;
        }

        public static IServiceCollection AddCustomClientServices(this IServiceCollection services, string connStringRedis)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            services.AddSingleton(options);

            services.AddHttpClient();

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
