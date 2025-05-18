
namespace Quantra.Messaging.Events;

public record TransactionCreatedEvent(Guid TransactionId, DateTime Timestamp, string ExternalId);
