using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pawn_Vault___OOP.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        public string EmpFN { get; set; }

        public string EmpLN { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }

        // Navigation
        public ICollection<Loan> Loans { get; set; }
    }
}
