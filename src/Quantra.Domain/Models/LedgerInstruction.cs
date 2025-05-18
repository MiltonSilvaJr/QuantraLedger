
namespace Quantra.Domain.Models;

public record LedgerInstruction(string Account, string Direction, decimal Amount, string Currency = "BRL");
