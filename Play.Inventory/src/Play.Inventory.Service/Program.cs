using MassTransit;
using Play.Common.MongoDB;
using Play.Common.MassTransit;
using Play.Inventory.Service.Consumers;
using Play.Inventory.Service.Entitites;
using Play.Inventory.Service.HttpClients;
using Polly;
using Polly.Timeout;

const string AllowedOriginSettings = "AllowedOrigin";

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services
    .AddMongo()
    .AddMongoRepository<InventoryEntity>("inventory_items")
    .AddMongoRepository<CatalogEntity>("catalog_items");

services.AddRabbitMQ();

// Explicit adding of RabbitMQ consumers
//services.AddMassTransit(busConfigurator =>
//{ 
//    busConfigurator.AddConsumer<CatalogItemCreatedConsumer>();
//    busConfigurator.AddConsumer<CatalogItemUpdatedConsumer>();
//    busConfigurator.AddConsumer<CatalogItemDeletedConsumer>();
//    busConfigurator.UsingRabbitMq((context, rabbitMQConfigurator) =>
//    {
//        rabbitMQConfigurator.ConfigureEndpoints(context);
//    });
//});

// Synchronous REST request to Play.Catalog.Service
// AddCatalogServiceClient(services);

services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddCatalogServiceClient(IServiceCollection serviceCollection)
{
    var jitter = new Random();

    // Typed HttpClient pattern is used. Every HttpClient isntance will be created by HttpClientFactory
    serviceCollection
        .AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7054");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Play.Inventory.Service");
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                   + TimeSpan.FromMilliseconds(jitter.Next(10, 1000)),
            onRetry: (_, timeSpan, retryAttempt) => Console.WriteLine($"Delaying for {timeSpan.TotalSeconds} seconds")
        ))
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking:3, 
            durationOfBreak: TimeSpan.FromSeconds(10),
            onBreak:(_, timeSpan) => Console.WriteLine($"Opening the circuit for {timeSpan.TotalSeconds} seconds"),
            onReset: () => Console.WriteLine("Closing the circuit")
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}
