using System;
using InventoryService;
using InventoryService.Infrastructure.Data;
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
using N8T.Infrastructure.Swagger;
using System.Collections.Generic;


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
    options.Audience = "inventory";
});

builder.Services.AddCustomOtelWithZipkin(builder.Configuration,
    o =>
    {
        o.Endpoint = isRunOnTye
            ? new Uri($"http://{builder.Configuration.GetServiceUri("zipkin")?.DnsSafeHost}:9411/api/v2/spans")
            : o.Endpoint;
    });

builder.Services.AddOpenApi("https://localhost:5001", new Dictionary<string, string>
            {
                {"scope1", "Demo API - full access"}
            });

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseOpenApi("inventory_api_swagger");

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
