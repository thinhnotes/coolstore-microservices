using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using N8T.Infrastructure.OTel;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("api", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
var inventoryUrl = "https+http://inventory-api";
var productCatalogUrl = "https+http://product-api";
var shoppingCartUrl = "https+http://shoppingcart-api";
var saleUrl = "https+http://sale-api";


builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy")).AddServiceDiscoveryDestinationResolver();

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{inventoryUrl}/healthz"),
        name: "inventoryapp-check", tags: new[] { "inventoryapp" })
    .AddUrlGroup(new Uri($"{productCatalogUrl}/healthz"),
        name: "productcatalogapp-check", tags: new[] { "productcatalogapp" })
    .AddUrlGroup(new Uri($"{shoppingCartUrl}/healthz"),
        name: "shoppingcartapp-check", tags: new[] { "shoppingcartapp" })
    .AddUrlGroup(new Uri($"{saleUrl}/healthz"),
        name: "saleapp-check", tags: new[] { "saleapp" });

builder.Services.AddCustomOtelWithZipkin(builder.Configuration,
    o =>
    {
        //o.Endpoint = isRunOnTye
        //    ? new Uri($"http://{builder.Configuration.GetServiceUri("zipkin")?.DnsSafeHost}:9411/api/v2/spans")
        //    : o.Endpoint;
    });


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCors("api");

app.UseRouting();

app.MapHealthChecks("/healthz",
        new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => true });

app.MapHealthChecks("/liveness",
        new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });

app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync("<h3>WebApiGateway</h3>");
    await context.Response.WriteAsync("<br>");
    await context.Response.WriteAsync("<a href='/inv/info'>Inventory&nbsp;|&nbsp;</a>");
    await context.Response.WriteAsync("<a href='/prod/info'>Product Catalog&nbsp;|&nbsp;</a>");
    await context.Response.WriteAsync("<a href='/cart/info'>Shopping Cart&nbsp;|&nbsp;</a>");
    await context.Response.WriteAsync("<a href='/sale/info'>Sale&nbsp;</a>");
});

app.MapReverseProxy();

app.Run();
