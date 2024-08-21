using AppHost.Health;
using Aspirant.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgresQL = builder.AddPostgres("postgresQL").WithHealthCheck();
var postgres = postgresQL.AddDatabase("postgres");

var identityApi = builder.AddProject<Projects.IdentityService>("identity-app")
    .WithReference(postgres)
    .WaitFor(postgres);

var inventoryApi = builder.AddProject<Projects.InventoryService_Api>("inventory-api")
    .WithReference(postgres)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WithSwaggerUI();

var productApi = builder.AddProject<Projects.ProductCatalogService_Api>("product-api")
    .WithReference(postgres)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WithSwaggerUI();

var saleApi = builder.AddProject<Projects.SaleService_Api>("sale-api")
    .WithReference(postgres)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WithSwaggerUI();

var shoppingCartApi = builder.AddProject<Projects.ShoppingCartService_Api>("shoppingcart-api")
    .WithReference(postgres)
    .WithReference(identityApi)
    .WaitFor(postgres)
    .WithSwaggerUI();

builder.AddProject<Projects.WebApiGateway>("webapigateway-api")
    .WaitFor(inventoryApi)
    .WaitFor(productApi)
    .WaitFor(saleApi)
    .WaitFor(shoppingCartApi);

builder.Build().Run();
