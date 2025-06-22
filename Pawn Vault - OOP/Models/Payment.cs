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
    }
}
