using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantra.Domain.Models;

namespace Quantra.Persistence;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("ledger_entries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Account).IsRequired().HasMaxLength(120);
        builder.Property(e => e.Currency).IsRequired().HasMaxLength(3);
        builder.Property(e => e.Direction).IsRequired().HasMaxLength(6);
        builder.Property(e => e.Amount).HasColumnType("numeric(38,10)");
        builder.Property(e => e.Timestamp).HasDefaultValueSql("now()");
        builder.HasIndex(e => e.Account);
    }
}