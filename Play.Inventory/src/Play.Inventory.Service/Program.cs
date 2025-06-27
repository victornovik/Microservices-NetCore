using Play.Common.MongoDB;
using Play.Inventory.Service.Entitites;
using Play.Inventory.Service.HttpClients;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddMongo().AddMongoRepository<InventoryEntity>("inventory_items");

var jitter = new Random();

services
    .AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7054");
    })
    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        retryCount: 5,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                 + TimeSpan.FromMilliseconds(jitter.Next(10, 1000)),
        onRetry: (outcome, timeSpan, retryAttempt) => Console.WriteLine($"Delaying for {timeSpan.TotalSeconds} seconds")
    ))
    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking:3, 
        durationOfBreak: TimeSpan.FromSeconds(10),
        onBreak:(outcome, timeSpan) => Console.WriteLine($"Opening the circuit for {timeSpan.TotalSeconds} seconds"),
        onReset: () => Console.WriteLine("Closing the circuit")
    ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

services.AddControllers();
services.AddOpenApi();
services.AddSwaggerGen();

var app = builder.Build();

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
