
var builder = DistributedApplication.CreateBuilder(args);

//var catalogDb = builder.AddPostgres("catalog")
//    .WithDataVolume()
//    .AddDatabase("catalogdb");

//var basketCache = builder.AddRedis("basketcache")
//    .WithRedisCommander()
//    .WithDataVolume();

builder.AddProject<Projects.IdentityService>("IdentityService");
builder.AddProject<Projects.InventoryService_Api>("InventoryService");
builder.AddProject<Projects.ProductCatalogService_Api>("ProductCatalogService");
builder.AddProject<Projects.SaleService_Api>("SaleService");
builder.AddProject<Projects.ShoppingCartService_Api>("ShoppingCartService");
builder.AddProject<Projects.WebApiGateway>("WebApiGateway");

builder.Build().Run();
