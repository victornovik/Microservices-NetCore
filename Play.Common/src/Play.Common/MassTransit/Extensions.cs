using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        services.AddMassTransit(massTransitConfigurator =>
        {
            // Register all MassTransit consumers inhereted from IConsumer<> found in the current assembly
            massTransitConfigurator.AddConsumers(Assembly.GetEntryAssembly());

            massTransitConfigurator.UsingRabbitMq((context, rabbitMQConfigurator) =>
            {
                var cfg = context.GetService<IConfiguration>();
                var rabbitMqSettings = cfg.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                var serviceSettings = cfg.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                rabbitMQConfigurator.Host(rabbitMqSettings?.Host);
                rabbitMQConfigurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings?.ServiceName, false));
                rabbitMQConfigurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                });
            });
        });

        return services;
    }
}