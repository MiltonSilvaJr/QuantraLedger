# apply-sprint5.ps1
$ErrorActionPreference = 'Stop'
Write-Host "Aplicando patch Sprint 5 no Quantra.Transaction..."

function Write-File($Path, $Text) {
  $dir = Split-Path $Path
  if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }
  $Text.Trim() | Out-File -FilePath $Path -Encoding UTF8
}

# 1) EfCoreLedgerService.cs
Write-File "src\Quantra.Transaction\EfCoreLedgerService.cs" @'
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quantra.Persistence;

namespace Quantra.Transaction;

public class EfCoreLedgerService : ILedgerService
{
    private readonly LedgerDbContext _db;
    private readonly IPublishEndpoint   _bus;

    public EfCoreLedgerService(LedgerDbContext db, IPublishEndpoint bus)
    {
        _db  = db;
        _bus = bus;
    }

    public async Task<Transaction> PostAsync(LedgerInstruction input, string? correlationId = null)
    {
        // Mapear input para entidade domain Transaction
        var entity = new Transaction
        {
            /* Id = Guid.NewGuid(), 
               Amount = input.Amount, 
               Timestamp = DateTime.UtcNow, etc. */
        };

        await _db.Transactions.AddAsync(entity);
        await _db.SaveChangesAsync();

        await _bus.Publish(new TransactionCreatedEvent(entity.Id, entity.Amount, entity.Timestamp));
        return entity;
    }

    public async Task<Transaction> PostAsync(IEnumerable<LedgerInstruction> inputs, string? correlationId = null)
    {
        // Implementar lógica de múltiplas instruções
        throw new NotImplementedException();
    }
}
'@

# 2) Program.cs
Write-File "src\Quantra.Transaction\Program.cs" @'
using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Quantra.Persistence;
using Quantra.Transaction;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddDbContext<LedgerDbContext>(opt =>
            opt.UseNpgsql(
              ctx.Configuration
                 .GetConnectionString("DB_CONN")
              ?? "Host=localhost;Database=quantra;Username=postgres;Password=postgres"
            ));

        // Registrando MassTransit e nosso service
        services.AddMessageBus(ctx.Configuration);
        services.AddScoped<ILedgerService, EfCoreLedgerService>();
    })
    .Build();

await host.RunAsync();
'@

Write-Host "✔ Sprint 5 aplicado com sucesso!"
