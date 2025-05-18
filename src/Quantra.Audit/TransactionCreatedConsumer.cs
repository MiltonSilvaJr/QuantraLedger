
using MassTransit;
using Quantra.Messaging.Events;
using Quantra.Persistence;
using Quantra.Domain.Models;

namespace Quantra.Audit;

public class TransactionCreatedConsumer : IConsumer<TransactionCreatedEvent>
{
    private readonly LedgerDbContext _db;
    public TransactionCreatedConsumer(LedgerDbContext db) => _db = db;

    public async Task Consume(ConsumeContext<TransactionCreatedEvent> context)
    {
        var evt = context.Message;
        var log = new AuditLog
        {
            EntityId = evt.TransactionId,
            EventType = nameof(TransactionCreatedEvent),
            Timestamp = evt.Timestamp,
            Payload = System.Text.Json.JsonSerializer.Serialize(evt)
        };
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
