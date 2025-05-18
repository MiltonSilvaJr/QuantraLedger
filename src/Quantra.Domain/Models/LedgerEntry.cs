
namespace Quantra.Domain.Models;
public record LedgerEntry(Guid Id, string Account, decimal Amount, string Currency, string Direction, DateTime Timestamp);
