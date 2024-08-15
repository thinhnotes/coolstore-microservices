using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N8T.Infrastructure.OTel.MediatR;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace N8T.Infrastructure.OTel
{
    public static class Extensions
    {
        public static IServiceCollection AddCustomOtelWithZipkin(this IServiceCollection services,
            IConfiguration config, Action<ZipkinExporterOptions> configureZipkin = null)
        {
            services.AddOpenTelemetry()
                .WithTracing(services => services
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
                    .AddSource(OTelMediatROptions.OTelMediatRName)
                    .SetSampler(new AlwaysOnSampler())
                    .AddZipkinExporter(o =>
                    {
                        config.Bind("OtelZipkin", o);
                        configureZipkin?.Invoke(o);
                    })
                );

            return services;
        }
    }
}
