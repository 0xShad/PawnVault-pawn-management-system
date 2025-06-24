using System.ComponentModel.DataAnnotations;

namespace Pawn_Vault___OOP.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        //[Required]
        [StringLength(20)]
        public string Condition { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal EstimatedValue { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal LoanAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public int? LoanID { get; set; } // Link to Loan if item is from a loan

        public bool IsDeleted { get; set; } = false;
    }
}