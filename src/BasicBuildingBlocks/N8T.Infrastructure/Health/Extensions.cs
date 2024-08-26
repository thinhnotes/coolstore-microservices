using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace N8T.Infrastructure.Health
{
    public static class Extensions
    {
        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddRequestTimeouts(
                configure: static timeouts =>
                    timeouts.AddPolicy("HealthChecks", TimeSpan.FromSeconds(5)));

            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            app.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => true });
            app.MapHealthChecks("/liveness",
                new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });
            return app;
        }
    }
}
