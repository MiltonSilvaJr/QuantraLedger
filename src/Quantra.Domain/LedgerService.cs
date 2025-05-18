
using Quantra.Domain.Models;

namespace Quantra.Domain;

public interface ILedgerService
{
    Task<Transaction> PostAsync(string debitAcc, string creditAcc, decimal amount, string currency = "BRL", string? idempotencyKey = null);
    Task<decimal> GetBalanceAsync(string account, string currency = "BRL");
    Task<Transaction> PostAsync(IEnumerable<LedgerInstruction> instructions, string? idempotencyKey = null);
}
