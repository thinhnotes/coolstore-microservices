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
using N8T.Infrastructure.Swagger;
using N8T.Infrastructure.Tye;
using N8T.Infrastructure.Validator;
using ProductCatalogService;
using ProductCatalogService.Domain.Gateway;
using ProductCatalogService.Infrastructure.Data;
using ProductCatalogService.Infrastructure.Gateway;

var builder = WebApplication.CreateBuilder(args);
bool isRunOnTye = builder.Configuration.IsRunOnTye();

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor()
    .AddCustomMediatR<Anchor>()
    .AddCustomValidators<Anchor>()
    .AddControllers();

builder.AddCustomClientServices("redis");

builder.AddCustomDbContext<MainDbContext, Anchor>(builder.Configuration.GetConnectionString("postgres"));

builder.Services.AddCustomAuth<Anchor>(builder.Configuration);

builder.Services.AddOpenApi(builder.Configuration, new Dictionary<string, string>
{
    {"scope1", "Demo API - full access"}
});

builder.Services.AddScoped<IInventoryGateway, InventoryGateway>();

var app = builder.Build();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
