using Microsoft.EntityFrameworkCore;
using TransactionCompliance.Domain.Models;

namespace TransactionCompliance.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<Status> Statuses { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}