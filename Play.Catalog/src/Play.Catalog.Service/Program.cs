using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddMongo()
        .AddMongoRepository<Entity>("items");

services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
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
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();