using System.ComponentModel.DataAnnotations;
namespace Pawn_Vault___OOP.Models
{
    public class LoanModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        // [Required] // removed kase wala pang text field sa create loan view page
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [Required]
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public DateTime RetentionPeriod { get; set; }

        // === Foreign Key ===
        public int CustomerID { get; set; }

        // ====== Navigation Property, para ma link yung Customer at Inventory Table sa Loan Table ======
        public Customer? Customer { get; set;}
        // public inventoryItem Inventory { get; set; } // di pa naiimplement ung table ng Inventory kaya di muna siya isasama.
    }
}
