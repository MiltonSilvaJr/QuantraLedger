
using System.Collections.Generic;

namespace Quantra.Domain.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ExternalId { get; set; } = string.Empty; // idempotency key
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ICollection<LedgerEntry> Entries { get; set; } = new List<LedgerEntry>();
}
