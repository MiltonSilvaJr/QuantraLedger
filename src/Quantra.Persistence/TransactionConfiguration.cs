
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantra.Domain.Models;

namespace Quantra.Persistence;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.ExternalId).IsRequired();
        builder.HasIndex(t => t.ExternalId).IsUnique();

        builder.HasMany(t => t.Entries)
               .WithOne()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
