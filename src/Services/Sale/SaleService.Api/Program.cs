using System;
using Google.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N8T.Infrastructure;
using N8T.Infrastructure.Auth;
using N8T.Infrastructure.ClientServices;
using N8T.Infrastructure.EfCore;
using N8T.Infrastructure.OTel;
using N8T.Infrastructure.Tye;
using N8T.Infrastructure.Validator;
using SaleService;
using SaleService.Domain.Gateway;
using SaleService.Domain.Model;
using SaleService.Infrastructure.Data;
using SaleService.Infrastructure.Gateway;
using SaleService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
bool isRunOnTye = builder.Configuration.IsRunOnTye();

builder.Services.AddHttpContextAccessor()
    .AddCustomMediatR<Anchor>()
    .AddCustomValidators<Anchor>()
    .AddCustomDbContext<MainDbContext, Anchor>(builder.Configuration.GetConnectionString("postgres"))
    .AddCustomClientServices(builder.Configuration.GetConnectionString("redis"))
    .AddControllers();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("postgres"));

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
builder.Services.AddScoped<IUserGateway, UserGateway>();
builder.Services.AddScoped<IInventoryGateway, InventoryGateway>();
builder.Services.AddScoped<IProductCatalogGateway, ProductCatalogGateway>();
builder.Services.AddScoped<IOrderValidationService, OrderValidationService>();

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
