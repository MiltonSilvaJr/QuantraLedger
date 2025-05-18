using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quantra.Persistence;

var root = new RootCommand("QuantraLedger CLI");
var migrate = new Command("migrate", "Applies EF Core migrations");

migrate.Handler = CommandHandler.Create(() =>
{
    var services = new ServiceCollection()
        .AddDbContext<LedgerDbContext>(opt =>
            opt.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONN")
                           ?? "Host=localhost;Database=quantra;Username=postgres;Password=postgres"));
    var sp = services.BuildServiceProvider();
    sp.GetRequiredService<LedgerDbContext>().Database.Migrate();
    Console.WriteLine("Migrations applied.");
    return 0;
});

root.AddCommand(migrate);
return await root.InvokeAsync(args);