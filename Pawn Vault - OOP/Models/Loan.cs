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
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Estimated value must be greater than or equal to 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedValue { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        public double InterestRate { get; set; }

        public int RetentionPeriod { get; set; }

        // Foreign key to IdentityUser (authenticated user who handled the loan)
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        
        // Navigation property to get user details
        public IdentityUser? User { get; set; } // navigation property, nullable

        // Foreign key to Customer
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }
        public Customer? Customer { get; set; } // navigation property, nullable

        // Transaction code for the loan
        [Required]
        [StringLength(50)]
        public string TransactionCode { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Payment> Payments { get; set; } = new List<Payment>(); // navigation property, no [Required], initialize to avoid null
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>(); // navigation property, no [Required], initialize to avoid null

        public DateTime DueDate { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
