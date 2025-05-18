using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quantra.Domain;
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
                    // Configuração do PostgreSQL (ou mude para InMemory/SQLite no dev)
                    services.AddDbContext<LedgerDbContext>(opt =>
                        opt.UseNpgsql(
                            ctx.Configuration.GetConnectionString("DB_CONN")
                            ?? "Host=localhost;Database=quantra;Username=postgres;Password=postgres"));

                    // Configura o barramento MassTransit
                    services.AddMessageBus(ctx.Configuration);

                    // Registra o serviço de domínio
                    services.AddScoped<ILedgerService, EfCoreLedgerService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}