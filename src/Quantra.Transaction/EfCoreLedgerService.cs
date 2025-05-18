using MassTransit;
using Quantra.Domain;
using Quantra.Domain.Models;
using Quantra.Messaging;
using Quantra.Persistence;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Transaction>> PostAsync(IEnumerable<LedgerInstruction> inputs, string? correlationId = null)
        {
            var results = new List<Transaction>();
            foreach (var input in inputs)
            {
                var tx = await PostAsync(input, correlationId);
                results.Add(tx);
            }
            return results;
        }
    }
}