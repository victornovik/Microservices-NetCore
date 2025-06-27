using Play.Common.MongoDB;
using Play.Inventory.Service.Entitites;
using Play.Inventory.Service.HttpClients;
using Polly;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddMongo().AddMongoRepository<InventoryEntity>("inventory_items");

services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7054");
})
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

services.AddControllers();
services.AddOpenApi();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
