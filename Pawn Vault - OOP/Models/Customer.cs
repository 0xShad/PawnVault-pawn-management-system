using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        
        // Computed property for full name
        public string FullName => $"{CustomerFN} {CustomerLN}".Trim();

        [NotMapped]
        public string Status
        {
            get
            {
                if (Loans == null || !Loans.Any())
                    return "No Loans";
                if (Loans.Any(l => l.Status == "Overdue" || l.Status == "Forfeited"))
                    return "Delinquent";
                if (Loans.Any(l => l.Status == "Active"))
                    return "Active";
                if (Loans.All(l => l.Status == "Paid" || l.Status == "Redeemed"))
                    return "Cleared";
                return "Inactive";
            }
        }
    }
}