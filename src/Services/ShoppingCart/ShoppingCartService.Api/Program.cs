using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N8T.Infrastructure;
using N8T.Infrastructure.Auth;
using N8T.Infrastructure.Dapr;
using N8T.Infrastructure.OTel;
using N8T.Infrastructure.Tye;
using N8T.Infrastructure.Validator;
using ShoppingCartService;
using ShoppingCartService.Domain.Gateway;
using ShoppingCartService.Infrastructure.Gateway;

var builder = WebApplication.CreateBuilder(args);
bool isRunOnTye = builder.Configuration.IsRunOnTye();

builder.Services.AddHttpContextAccessor()
    .AddCustomMediatR<Anchor>()
    .AddCustomValidators<Anchor>()
    .AddCustomClientServices()
    .AddControllers();

builder.Services.AddHealthChecks();

builder.Services.AddCustomAuth<Anchor>(builder.Configuration, options =>
{
    options.Authority = isRunOnTye
        ? builder.Configuration.GetServiceUri("identityapp")?.AbsoluteUri
        : options.Authority;

    options.Audience = isRunOnTye
        ? $"{builder.Configuration.GetServiceUri("identityapp")?.AbsoluteUri.TrimEnd('/')}/resources"
    : options.Audience;
});

builder.Services.AddScoped<ISecurityContextAccessor, SecurityContextAccessor>();
builder.Services.AddScoped<IProductCatalogGateway, ProductCatalogGateway>();
builder.Services.AddScoped<IPromoGateway, PromoGateway>();
builder.Services.AddScoped<IShippingGateway, ShippingGateway>();

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

app.UseRouting();

app.UseCloudEvents();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => true });
    endpoints.MapHealthChecks("/liveness",
        new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });

    endpoints.MapControllers();
    endpoints.MapSubscribeHandler();
});

//app.ApplicationServices.CreateLoggerConfiguration(IsRunOnTye);

app.Run();
