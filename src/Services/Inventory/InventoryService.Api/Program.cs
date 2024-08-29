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
using N8T.Infrastructure.Validator;
using N8T.Infrastructure.Swagger;
using System.Collections.Generic;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHttpContextAccessor()
        .AddCustomMediatR<Anchor>()
        .AddCustomValidators<Anchor>()
        .AddControllers();

builder.AddCustomClientServices("redis");
builder.AddCustomDbContext<MainDbContext, Anchor>(builder.Configuration.GetConnectionString("postgres"));

builder.Services.AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("postgres"));

builder.Services.AddCustomAuth<Anchor>(builder.Configuration);

builder.Services.AddOpenApi(builder.Configuration, new Dictionary<string, string>
{
    {"scope1", "Demo API - full access"}
});

var app = builder.Build();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapEndpoints(versionedGroup);
app.MapControllers();

app.Run();
