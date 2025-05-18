
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantra.Domain.Models;

namespace Quantra.Persistence;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Name).IsRequired().HasMaxLength(120);
        builder.HasMany(o => o.Accounts).WithOne(a => a.Organization).HasForeignKey(a => a.OrganizationId);
    }
}
