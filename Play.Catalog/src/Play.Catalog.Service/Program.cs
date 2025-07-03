using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDB;

const string AllowedOriginSettings = "AllowedOrigin";

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddMongo().AddMongoRepository<Entity>("catalog_items");
services.AddRabbitMQ();

services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
services.AddOpenApi();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(configurePolicy =>
    {
        configurePolicy.WithOrigins(app.Configuration[AllowedOriginSettings]!)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();