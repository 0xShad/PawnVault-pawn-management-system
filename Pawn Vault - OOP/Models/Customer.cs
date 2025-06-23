using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Pawn_Vault___OOP.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        public string CustomerFN { get; set; } = string.Empty;

        [Required]
        public string CustomerLN { get; set; } = string.Empty;

        public string TelephoneNo { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string Municipality { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;
        // public string email { get; set; } = string.Empty;
        // Navigation property

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}