using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quantra.Domain;
using Quantra.Domain.Models;
using Quantra.Messaging;
using Quantra.Messaging.Events;
using Quantra.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantra.Transaction
{
    public class EfCoreLedgerService : ILedgerService
    {
        private readonly LedgerDbContext _db;
        private readonly IPublishEndpoint _bus;

        public EfCoreLedgerService(LedgerDbContext db, IPublishEndpoint bus)
        {
            _db = db;
            _bus = bus;
        }

        public async Task<Transaction> PostAsync(LedgerInstruction input, string? correlationId = null)
        {
            var entity = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = input.AccountId,
                Counterparty = input.Counterparty,
                Amount = input.Amount,
                Timestamp = DateTime.UtcNow,
                CorrelationId = correlationId
            };

            await _db.Transactions.AddAsync(entity);
            await _db.SaveChangesAsync();

            await _bus.Publish(new TransactionCreatedEvent(
                entity.Id,
                entity.Amount,
                entity.Timestamp,
                correlationId));

            return entity;
        }

        public async Task<Transaction> PostAsync(string accountId, string counterparty, decimal amount, string idempotencyKey, string? correlationId = null)
        {
            var instruction = new LedgerInstruction
            {
                AccountId = accountId,
                Counterparty = counterparty,
                Amount = amount,
                IdempotencyKey = idempotencyKey
            };
            return await PostAsync(instruction, correlationId);
        }

        public async Task<Transaction> PostAsync(IEnumerable<LedgerInstruction> inputs, string? correlationId = null)
        {
            Transaction? last = null;
            foreach (var input in inputs)
            {
                last = await PostAsync(input, correlationId);
            }
            return last!;
        }

        public async Task<decimal> GetBalanceAsync(string accountId, string? correlationId = null)
        {
            return await _db.Transactions
                            .Where(t => t.AccountId == accountId)
                            .SumAsync(t => t.Amount);
        }
    }
}