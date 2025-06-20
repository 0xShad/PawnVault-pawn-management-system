using System.ComponentModel.DataAnnotations;
namespace Pawn_Vault___OOP.Models
{
    public class LoanModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
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
        public DateOnly RetentionPeriod { get; set; }

        // === Foreign Key ===
        public int CustomerId { get; set; }

        // ====== Navigation Property, para ma link yung Loan at Customer Table at makuha yung Customer Name ======
        // public Customer Customer { get; set;} // di pa naiimplement ung table ng Customer kaya di muna siya isasama.

        // === sitll not sure kung need pa ba ideclare or not ===
        // public int EmployeeId { get; set; 

    }
}
