using System.ComponentModel.DataAnnotations;

namespace Pawn_Vault___OOP.Models
{
    public class LoanModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } 
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public decimal InterestRate { get; set; }
        public DateOnly RetentionPeriod { get; set; }

        // === Foreign Keys, tho sitll not sure kung need pa ba ideclare or not ===
        /*  
        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }
        */

    }
}
