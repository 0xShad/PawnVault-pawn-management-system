using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pawn_Vault___OOP.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        public string EmpFN { get; set; }

        [Required]
        public string EmpLN { get; set; }

        [Required]
        [RegularExpression("Admin|Employee", ErrorMessage = "Role must be either Admin or Employee")]
        public string Role { get; set; }

        [Required]
        [DefaultValue("Active")]
        public string Status { get; set; } // Active, Inactive

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Navigation
        public ICollection<Loan> Loans { get; set; }
    }
}
