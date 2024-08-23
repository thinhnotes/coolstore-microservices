using System;
using System.Collections.Generic;
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
using N8T.Infrastructure.Swagger;
using N8T.Infrastructure.Tye;
using N8T.Infrastructure.Validator;
using SaleService;
using SaleService.Domain.Gateway;
using SaleService.Domain.Model;
using SaleService.Infrastructure.Data;
using SaleService.Infrastructure.Gateway;
using SaleService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor()
    .AddCustomMediatR<Anchor>()
    .AddCustomValidators<Anchor>()
    .AddCustomDbContext<MainDbContext, Anchor>(builder.Configuration.GetConnectionString("postgres"))
    .AddCustomClientServices(builder.Configuration.GetConnectionString("redis"))
    .AddControllers();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("postgres"));

builder.Services.AddCustomAuth<Anchor>(builder.Configuration);

builder.Services.AddScoped<ISecurityContextAccessor, SecurityContextAccessor>();
builder.Services.AddScoped<IUserGateway, UserGateway>();
builder.Services.AddScoped<IInventoryGateway, InventoryGateway>();
builder.Services.AddScoped<IProductCatalogGateway, ProductCatalogGateway>();
builder.Services.AddScoped<IOrderValidationService, OrderValidationService>();

//Need check zipkin
builder.Services.AddCustomOtelWithZipkin(builder.Configuration);

builder.Services.AddOpenApi(builder.Configuration, new Dictionary<string, string>
{
    {"scope1", "Demo API - full access"}
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => true });
    endpoints.MapHealthChecks("/liveness",
        new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });

    endpoints.MapControllers();
});

//app.ApplicationServices.CreateLoggerConfiguration(IsRunOnTye);

app.Run();
