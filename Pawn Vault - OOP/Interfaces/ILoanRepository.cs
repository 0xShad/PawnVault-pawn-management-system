using Pawn_Vault___OOP.Data.Migrations;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Interfaces
{
    public interface ILoanRepository
    {
        // ==== declaration ng mga methods for querying sa database, check the LoanRepository for logic ===
        Task<IEnumerable<LoanModel>> GetAllLoansAsync (); // pagkuha ng lahat ng records o rows sa table
        Task<LoanModel> GetLoanbyIdAsync(int id); // method pag ikiclick ng user ung view details ng loan
        Task<LoanModel> AddLoanAsync(LoanModel loan); // method pag magdadagdag ng loan
        Task<LoanModel> UpdateLoanAsync(LoanModel loan); // method pag maguupdate ng existing loan
        Task DeleteLoanAsync(int id); // method pag magdedelete ng loan
        

    }
}
