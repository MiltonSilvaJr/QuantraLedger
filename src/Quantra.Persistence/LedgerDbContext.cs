using Microsoft.EntityFrameworkCore;
using Quantra.Domain.Models;

namespace Quantra.Persistence;

public class LedgerDbContext : DbContext
{
    public LedgerDbContext(DbContextOptions<LedgerDbContext> options)
        : base(options)
    { }

    public DbSet<LedgerEntry> LedgerEntries   { get; set; }
    public DbSet<Transaction> Transactions    { get; set; }
    public DbSet<Organization> Organizations  { get; set; }
    public DbSet<Account> Accounts            { get; set; }
    public DbSet<AssetRate> AssetRates        { get; set; }
    public DbSet<AuditLog> AuditLogs          { get; set; }
    public DbSet<User> Users                  { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new AssetRateConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
