
using MassTransit;
using Quantra.Audit;
using Quantra.Messaging;
using Quantra.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string conn = builder.Configuration.GetConnectionString("LedgerDb") 
              ?? Environment.GetEnvironmentVariable("LEDGER_CONNECTION") 
              ?? "Host=localhost;Database=midaz;Username=midaz;Password=midaz";

builder.Services.AddDbContext<LedgerDbContext>(opt => opt.UseNpgsql(conn));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TransactionCreatedConsumer>();
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbit = builder.Configuration.GetConnectionString("Rabbit") ?? "rabbitmq://localhost";
        cfg.Host(rabbit);
        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();
app.MapGet("/", () => "Audit service running");
app.Run();
