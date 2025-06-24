using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Pawn_Vault___OOP.Models
{
    public class Loan
    {
        [Key]
        public int LoanID { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        
        [Required]
        //[Column(TypeName = "decimal(18,2)")] // allocation lang for precision
        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        public double InterestRate { get; set; }

        public int RetentionPeriod { get; set; }

        // Foreign key to IdentityUser (authenticated user who handled the loan)
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        
        // Navigation property to get user details
        public IdentityUser User { get; set; }

        // Foreign key to Customer
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        // Transaction code for the loan
        [Required]
        [StringLength(50)]
        public string TransactionCode { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Payment> Payments { get; set; }
        public ICollection<InventoryItem> InventoryItems { get; set; }
    }
}
