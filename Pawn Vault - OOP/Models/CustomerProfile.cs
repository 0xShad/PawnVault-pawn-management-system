using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pawn_Vault___OOP.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        public string CustomerFN { get; set; }

        [Required]
        public string CustomerLN { get; set; }

        [Required]
        public string TelephoneNo { get; set; }

        public string Street { get; set; }

        public string Municipality { get; set; }

        public string ZipCode { get; set; }

        // Navigation property
        public ICollection<Loan> Loans { get; set; }
    }
}
