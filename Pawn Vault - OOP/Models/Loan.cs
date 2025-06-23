using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public string Status { get; set; }

        public DateTime IssuedDate { get; set; }

        public double InterestRate { get; set; }

        public int RetentionPeriod { get; set; }

        // Foreign keys
        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<InventoryItem> InventoryItems { get; set; }

    }
}
