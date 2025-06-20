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
        public string CustomerFN { get; set; }

        [Required]
        public string CustomerLN { get; set; }

        [Required]
        public string TelephoneNo { get; set; }

        [AllowNull] //wala pa kasing add customer or sa loan
        public string Street { get; set; }

        [AllowNull]
        public string Municipality { get; set; }

        [AllowNull]
        public string ZipCode { get; set; }

        // Navigation property
        public ICollection<Loan> Loans { get; set; }
    }
}
