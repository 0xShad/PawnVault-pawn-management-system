using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Data
{
    public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
       
            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Balance)
                .HasPrecision(18, 2);

            builder.Entity<InventoryItem>()
                .Property(i => i.EstimatedValue)
                .HasPrecision(18, 2);

            builder.Entity<InventoryItem>()
                .Property(i => i.LoanAmount)
                .HasPrecision(18, 2);

            // Configure LoanModel
            builder.Entity<LoanModel>()
                .Property(l => l.Amount)
                .HasPrecision(18, 2);

            builder.Entity<LoanModel>()
                .Property(l => l.InterestRate)
                .HasPrecision(18, 2);
        }

    }

}
