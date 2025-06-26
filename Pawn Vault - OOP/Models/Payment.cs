using System;
using System.ComponentModel.DataAnnotations;

namespace Pawn_Vault___OOP.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public int LoanID { get; set; }

        // Navigation
        public Loan Loan { get; set; }

        public string TransactionType { get; set; } = string.Empty; // redemption, renewal, forfeit, overdue, etc.
        public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, etc.
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string HandledBy { get; set; } = string.Empty; // User full name or user id

        public bool IsDeleted { get; set; } = false;
    }
}
