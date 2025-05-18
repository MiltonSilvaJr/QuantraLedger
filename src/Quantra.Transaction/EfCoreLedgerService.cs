
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quantra.Domain;
using Quantra.Domain.Models;
using Quantra.Messaging.Events;
using Quantra.Persistence;

namespace Quantra.Transaction;

public class EfCoreLedgerService : ILedgerService
{
    private readonly LedgerDbContext _db;
    private readonly IPublishEndpoint _bus;
    public EfCoreLedgerService(LedgerDbContext db, IPublishEndpoint bus)
    {
        _db = db;
        _bus = bus;
    }

    // existing methods
    public async Task<Transaction> PostAsync(string debitAcc, string creditAcc, decimal amount, string currency = "BRL", string? idempotencyKey = null)
    {
        var tx = await PostAsync(new[]
        {
            new LedgerInstruction(debitAcc, "debit", amount, currency),
            new LedgerInstruction(creditAcc, "credit", amount, currency)
        }, idempotencyKey);
        return tx;
    }

    public async Task<Transaction> PostAsync(IEnumerable<LedgerInstruction> instructions, string? idempotencyKey = null)
    {
        if (idempotencyKey != null)
        {
            var existing = await _db.Transactions.Include(t=>t.Entries)
                                .FirstOrDefaultAsync(t => t.ExternalId == idempotencyKey);
            if (existing is not null) return existing;
        }

        var entries = instructions.Select(i => new LedgerEntry(Guid.NewGuid(), i.Account,
            i.Direction == "debit" ? -i.Amount : i.Amount, i.Currency, i.Direction, DateTime.UtcNow)).ToList();

        var tx = new Transaction
        {
            ExternalId = idempotencyKey ?? Guid.NewGuid().ToString("N"),
            Entries = entries
        };
        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();

        await _bus.Publish(new TransactionCreatedEvent(tx.Id, tx.Timestamp, tx.ExternalId));

        return tx;
    }

    public async Task<decimal> GetBalanceAsync(string account, string currency = "BRL") =>
        await _db.LedgerEntries
                 .Where(e=>e.Account==account && e.Currency==currency)
                 .SumAsync(e=>e.Amount);
}
