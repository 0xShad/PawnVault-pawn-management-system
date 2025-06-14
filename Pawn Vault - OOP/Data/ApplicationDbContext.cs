﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        }
    }
}
