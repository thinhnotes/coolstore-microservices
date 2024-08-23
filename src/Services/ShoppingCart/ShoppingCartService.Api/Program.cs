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
using N8T.Infrastructure.OTel;
using N8T.Infrastructure.Swagger;
using N8T.Infrastructure.Tye;
using N8T.Infrastructure.Validator;
using ShoppingCartService;
using ShoppingCartService.Domain.Gateway;
using ShoppingCartService.Infrastructure.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor()
    .AddCustomMediatR<Anchor>()
    .AddCustomValidators<Anchor>()
    .AddCustomClientServices(builder.Configuration.GetConnectionString("redis"))
    .AddControllers();

builder.Services.AddHealthChecks();

builder.Services.AddCustomAuth<Anchor>(builder.Configuration);

builder.Services.AddScoped<ISecurityContextAccessor, SecurityContextAccessor>();
builder.Services.AddScoped<IProductCatalogGateway, ProductCatalogGateway>();
builder.Services.AddScoped<IPromoGateway, PromoGateway>();
builder.Services.AddScoped<IShippingGateway, ShippingGateway>();

//check zipkin
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

app.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => true });
app.MapHealthChecks("/liveness",
    new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });

app.MapControllers();


app.Run();
