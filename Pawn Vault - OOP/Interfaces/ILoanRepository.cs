using Pawn_Vault___OOP.Data.Migrations;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Interfaces
{
    public interface ILoanRepository
    {
        // ==== declaration ng mga methods for querying sa database, check the LoanRepository for logic ===
        Task<IEnumerable<Loan>> GetAllLoansAsync (); // pagkuha ng lahat ng records o rows sa table
        Task<Loan> GetLoanbyIdAsync(int id); // method pag ikiclick ng user ung view details ng loan
        Task<Loan> AddLoanAsync(Loan loan); // method pag magdadagdag ng loan
        Task<Loan> UpdateLoanAsync(Loan loan); // method pag maguupdate ng existing loan
        Task DeleteLoanAsync(int id); // method pag magdedelete ng loan
    }
}
