using AppHost.Health;
using Aspirant.Hosting;
using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresQL = builder.AddPostgres("postgresQL").WithHealthCheck();
var postgres = postgresQL.AddDatabase("postgres");

var redis = builder.AddRedis("redis").WithHealthCheck();

bool includeInfra = false;

var identityApi = builder.AddProject<Projects.IdentityService>("identity-app");

var inventoryApi = builder.AddProject<Projects.InventoryService_Api>("inventory-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithSwaggerUI();

var productApi = builder.AddProject<Projects.ProductCatalogService_Api>("productcatalogapp")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithReference(inventoryApi)
    .WithSwaggerUI();

var saleApi = builder.AddProject<Projects.SaleService_Api>("sale-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithSwaggerUI();

var shoppingCartApi = builder.AddProject<Projects.ShoppingCartService_Api>("shoppingcart-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WithReference(productApi)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithSwaggerUI();

var webapigatewayApi = builder.AddProject<Projects.WebApiGateway>("webapigateway-api")
    .WaitFor(inventoryApi)
    .WaitFor(productApi)
    .WaitFor(saleApi)
    .WaitFor(shoppingCartApi)
    .WithReference(inventoryApi)
    .WithReference(productApi)
    .WithReference(saleApi)
    .WithReference(shoppingCartApi);

var blazorClient = builder.AddProject<Projects.BlazorWeb_Client>("webUI")
    .WaitFor(identityApi)
    .WaitFor(webapigatewayApi);
var identityUrl = identityApi.GetEndpoint("http");
var webapiUrl = webapigatewayApi.GetEndpoint("http");
var blazorClientUrl = blazorClient.GetEndpoint("http");

//it need run node 10.16.3 and run npm install before run the projects
builder.AddNpmApp("web", "../web", "dev")
    //.WithEnvironment("PORT", "3000")
    .WithEnvironment("VITE_REACT_APP_AUTHORITY", identityUrl)
    .WithEnvironment("VITE_REACT_APP_API", webapiUrl)
    .WithEnvironment("VITE_REACT_APP_BLAZOR", blazorClientUrl)
    //.WithHttpEndpoint(3000)
    .WaitFor(identityApi)
    .WaitFor(webapigatewayApi)
    .PublishAsDockerFile();

if (!includeInfra)
{
    var hostInfra = "localhost";
    inventoryApi.WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
                .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377");

    productApi.WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
                .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377");

    saleApi.WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
                .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377");

    shoppingCartApi.WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
                .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377");

}

builder.Build().Run();
