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
            massTransitConfigurator.AddConsumers(Assembly.GetEntryAssembly());

            massTransitConfigurator.UsingRabbitMq((context, rabbitMQConfigurator) =>
            {
                var cfg = context.GetService<IConfiguration>();

                var rabbitMqSettings = cfg.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                rabbitMQConfigurator.Host(rabbitMqSettings?.Host);

                var serviceSettings = cfg.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                rabbitMQConfigurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings?.ServiceName, false));
            });
        });

        return services;
    }
}