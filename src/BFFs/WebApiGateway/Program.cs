using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.ReverseProxy.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ReverseProxy.Service;
using WebApiGateway;
using N8T.Infrastructure.OTel;
using Microsoft.Extensions.Configuration;
using N8T.Infrastructure.Tye;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("api", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
bool isRunOnTye = builder.Configuration.IsRunOnTye();
// inventory
var inventoryUrl = isRunOnTye
    ? $"{builder.Configuration.GetServiceUri("inventoryapp")?.AbsoluteUri}"
    : builder.Configuration.GetValue<string>("Services:inventoryapp");

var invRoute = new ProxyRoute
{
    RouteId = "inv",
    ClusterId = "inv-svc-cluster",
    Match =
                {
                    Path = "/inv/{**catch-all}"
                },
    Transforms = new List<IDictionary<string, string>>()
};

invRoute.AddTransformXForwarded();
invRoute.AddTransformPathRemovePrefix("/inv");

var invCluster = new Cluster
{
    Id = "inv-svc-cluster",
    Destinations =
                {
                    {
                        "inv-svc-cluster/destination1", new Destination
                        {
                            Address = inventoryUrl
                        }
                    }
                }
};

// product catalog
var productCatalogUrl = isRunOnTye
    ? $"{builder.Configuration.GetServiceUri("productcatalogapp")?.AbsoluteUri}"
    : builder.Configuration.GetValue<string>("Services:productcatalogapp");

var prodRoute = new ProxyRoute
{
    RouteId = "prod",
    ClusterId = "prod-svc-cluster",
    Match =
                {
                    Path = "/prod/{**catch-all}"
                },
    Transforms = new List<IDictionary<string, string>>()
};

prodRoute.AddTransformXForwarded();
prodRoute.AddTransformPathRemovePrefix("/prod");

var prodCluster = new Cluster
{
    Id = "prod-svc-cluster",
    Destinations =
                {
                    {
                        "prod-svc-cluster/destination1", new Destination
                        {
                            Address = productCatalogUrl
                        }
                    }
                }
};

// shopping cart
var shoppingCartUrl = isRunOnTye
    ? $"{builder.Configuration.GetServiceUri("shoppingcartapp")?.AbsoluteUri}"
    : builder.Configuration.GetValue<string>("Services:shoppingcartapp");

var cartRoute = new ProxyRoute
{
    RouteId = "cart",
    ClusterId = "cart-svc-cluster",
    Match =
                {
                    Path = "/cart/{**catch-all}"
                },
    Transforms = new List<IDictionary<string, string>>()
};

cartRoute.AddTransformXForwarded();
cartRoute.AddTransformPathRemovePrefix("/cart");

var cartCluster = new Cluster
{
    Id = "cart-svc-cluster",
    Destinations =
                {
                    {
                        "cart-svc-cluster/destination1", new Destination
                        {
                            Address = shoppingCartUrl
                        }
                    }
                }
};


// sale
var saleUrl = isRunOnTye
    ? $"{builder.Configuration.GetServiceUri("saleapp")?.AbsoluteUri}"
    : builder.Configuration.GetValue<string>("Services:saleapp");

var saleRoute = new ProxyRoute
{
    RouteId = "sale",
    ClusterId = "sale-svc-cluster",
    Match =
                {
                    Path = "/sale/{**catch-all}"
                },
    Transforms = new List<IDictionary<string, string>>()
};

saleRoute.AddTransformXForwarded();
saleRoute.AddTransformPathRemovePrefix("/sale");

var saleCluster = new Cluster
{
    Id = "sale-svc-cluster",
    Destinations =
                {
                    {
                        "sale-svc-cluster/destination1", new Destination
                        {
                            Address = saleUrl
                        }
                    }
                }
};

// configure
var routes = new[]
{
                invRoute,
                prodRoute,
                cartRoute,
                saleRoute
            };

var clusters = new[]
{
                invCluster,
                prodCluster,
                cartCluster,
                saleCluster
            };

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters);

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(System.IO.Path.Combine(inventoryUrl, "healthz")),
        name: "inventoryapp-check", tags: new[] { "inventoryapp" })
    .AddUrlGroup(new Uri(System.IO.Path.Combine(productCatalogUrl, "healthz")),
        name: "productcatalogapp-check", tags: new[] { "productcatalogapp" })
    .AddUrlGroup(new Uri(System.IO.Path.Combine(shoppingCartUrl, "healthz")),
        name: "shoppingcartapp-check", tags: new[] { "shoppingcartapp" })
    .AddUrlGroup(new Uri(System.IO.Path.Combine(saleUrl, "healthz")),
        name: "saleapp-check", tags: new[] { "saleapp" });

builder.Services.AddCustomOtelWithZipkin(builder.Configuration,
    o =>
    {
        o.Endpoint = isRunOnTye
            ? new Uri($"http://{builder.Configuration.GetServiceUri("zipkin")?.DnsSafeHost}:9411/api/v2/spans")
            : o.Endpoint;
    });


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCors("api");

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz",
        new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => true });

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

app.Run();
