using AppHost.Health;
using Aspirant.Hosting;
using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresQL = builder.AddPostgres("postgresQL").WithHealthCheck();
var postgres = postgresQL.AddDatabase("postgres");

var redis = builder.AddRedis("redis").WithHealthCheck();

var hostInfra = "localhost";

var identityApi = builder.AddProject<Projects.IdentityService>("identity-app");

var inventoryApi = builder.AddProject<Projects.InventoryService_Api>("inventory-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WaitFor(redis)
    // .WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
    // .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377")
    .WithSwaggerUI();

var productApi = builder.AddProject<Projects.ProductCatalogService_Api>("productcatalogapp")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WaitFor(redis)
    // .WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
    // .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377")
    .WithReference(inventoryApi)
    .WithSwaggerUI();

var saleApi = builder.AddProject<Projects.SaleService_Api>("sale-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WaitFor(redis)
    // .WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
    // .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377")
    .WithSwaggerUI();

var shoppingCartApi = builder.AddProject<Projects.ShoppingCartService_Api>("shoppingcart-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(identityApi)
    .WithReference(productApi) 
    .WaitFor(postgres)
    .WaitFor(redis)
    // .WithEnvironment("ConnectionStrings__postgres", $"Server={hostInfra};Port=5434;Database=postgres;User Id=postgres;Password=P@ssw0rd")
    // .WithEnvironment("ConnectionStrings__redis", $"{hostInfra}:6377")
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

builder.AddProject<Projects.BlazorWeb>("webUI")
       .WaitFor(identityApi)
       .WaitFor(webapigatewayApi)
       .WithReference(identityApi)
       .WithReference(webapigatewayApi);
var identityUrl = identityApi.GetEndpoint("http");
var webapiUrl = webapigatewayApi.GetEndpoint("http");

builder.AddProject<Projects.BlazorWeb_Client>("webUI-client")
       .WaitFor(identityApi)
       .WaitFor(webapigatewayApi)
       .WithReference(identityApi)
       .WithReference(webapigatewayApi);

//it need run node 10.16.3 and run npm install before run the projects
//builder.AddNpmApp("web", "../web")
//    .WithEnvironment("PORT", "3000")
//    .WithEnvironment("REACT_APP_AUTHORITY", identityUrl)
//    .WithEnvironment("REACT_APP_API", webapiUrl)
//    //.WithHttpEndpoint(3000)
//    .WaitFor(identityApi)
//    .WaitFor(webapigatewayApi)
//    .PublishAsDockerFile();

builder.Build().Run();
