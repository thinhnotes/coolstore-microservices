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
using ProductCatalogService;
using ProductCatalogService.Domain.Gateway;
using ProductCatalogService.Infrastructure.Data;
using ProductCatalogService.Infrastructure.Gateway;

var builder = WebApplication.CreateBuilder(args);
bool isRunOnTye = builder.Configuration.IsRunOnTye();

builder.Services.AddServiceDiscovery();

builder.Services.ConfigureHttpClientDefaults(http =>
{
    // Turn on service discovery by default
    http.AddServiceDiscovery();
});

builder.Services.AddHttpContextAccessor()
    .AddCustomMediatR<Anchor>()
    .AddCustomValidators<Anchor>()
    .AddCustomDbContext<MainDbContext, Anchor>(builder.Configuration.GetConnectionString("postgres"))
    .AddCustomClientServices(builder.Configuration.GetConnectionString("redis"))
    .AddControllers();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("postgres"));

builder.Services.AddCustomAuth<Anchor>(builder.Configuration);

builder.Services.AddOpenApi(builder.Configuration, new Dictionary<string, string>
{
    {"scope1", "Demo API - full access"}
});

builder.Services.AddScoped<IInventoryGateway, InventoryGateway>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapControllers();

app.Run();
