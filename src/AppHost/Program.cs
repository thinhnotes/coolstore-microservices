using Aspirant.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgresQL = builder.AddPostgres("postgresQL");
var postgres = postgresQL.AddDatabase("postgres");

var inventory_api = builder.AddProject<Projects.InventoryService_Api>("inventoryapi")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithSwaggerUI();

builder.Build().Run();
