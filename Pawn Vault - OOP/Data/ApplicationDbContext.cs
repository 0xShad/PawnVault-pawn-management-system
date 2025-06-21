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

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<LoanModel> LoanModels { get; set; } //  Still thinking whether the "LoanModels" should be change to just "Loans" for clarity or readability
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure InventoryItem
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
