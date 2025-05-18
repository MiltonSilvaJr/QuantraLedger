
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantra.Domain.Models;

namespace Quantra.Persistence;

public class AssetRateConfiguration : IEntityTypeConfiguration<AssetRate>
{
    public void Configure(EntityTypeBuilder<AssetRate> builder)
    {
        builder.ToTable("asset_rates");
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.FromCurrency, a.ToCurrency, a.Timestamp });
        builder.Property(a => a.FromCurrency).HasMaxLength(3);
        builder.Property(a => a.ToCurrency).HasMaxLength(3);
        builder.Property(a => a.Rate).HasColumnType("numeric(18,6)");
    }
}
