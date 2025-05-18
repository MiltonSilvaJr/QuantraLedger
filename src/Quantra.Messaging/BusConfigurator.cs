using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;



namespace Quantra.Messaging;

public static class BusConfigurator
{
    public static void AddMessageBus(this IServiceCollection services, IConfiguration cfg)
    {
        var rabbitConn = cfg.GetConnectionString("Rabbit") 
            ?? Environment.GetEnvironmentVariable("RABBIT_CONN") 
            ?? "rabbitmq://localhost";
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((ctx, busCfg) =>
            {
                busCfg.Host(rabbitConn);
            });
        });
    }
}
