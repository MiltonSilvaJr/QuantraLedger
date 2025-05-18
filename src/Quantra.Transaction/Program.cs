using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quantra.Domain;
using Quantra.Messaging;
using Quantra.Persistence;
using Quantra.Transaction;
using System;
using System.Threading.Tasks;

namespace Quantra.Transaction
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services.AddDbContext<LedgerDbContext>(opt =>
                        opt.UseNpgsql(
                            ctx.Configuration.GetConnectionString("DB_CONN")
                            ?? "Host=localhost;Database=quantra;Username=postgres;Password=postgres"));

                    services.AddMessageBus(ctx.Configuration);
                    services.AddScoped<ILedgerService, EfCoreLedgerService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}