using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ReverseProxy.Abstractions;
using Microsoft.ReverseProxy.Service;
using N8T.Infrastructure.Logging;
using N8T.Infrastructure.OTel;
using N8T.Infrastructure.Tye;

namespace WebApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Config = config;
        }

        private IConfiguration Config { get; }
        private bool IsRunOnTye => Config.IsRunOnTye();

        public void ConfigureServices(IServiceCollection services)
        {
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("api");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz",
                    new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions {Predicate = _ => true});

                endpoints.MapHealthChecks("/liveness",
                    new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                    {
                        Predicate = r => r.Name.Contains("self")
                    });

                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<h3>WebApiGateway</h3>");
                    await context.Response.WriteAsync("<br>");
                    await context.Response.WriteAsync("<a href='/inv/info'>Inventory&nbsp;|&nbsp;</a>");
                    await context.Response.WriteAsync("<a href='/prod/info'>Product Catalog&nbsp;|&nbsp;</a>");
                    await context.Response.WriteAsync("<a href='/cart/info'>Shopping Cart&nbsp;|&nbsp;</a>");
                    await context.Response.WriteAsync("<a href='/sale/info'>Sale&nbsp;</a>");
                });

                endpoints.MapReverseProxy(proxyPipeline =>
                {
                    proxyPipeline.UseAffinitizedDestinationLookup();
                    proxyPipeline.UseProxyLoadBalancing();
                    proxyPipeline.UseRequestAffinitizer();
                });
            });

            app.ApplicationServices.CreateLoggerConfiguration(IsRunOnTye);
        }
    }
}
