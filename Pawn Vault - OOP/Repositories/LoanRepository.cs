using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Data;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<LoanModel>> GetAllLoansAsync()
        {
            return await _context.LoanModels.ToListAsync(); // fetching all record atsaka irereturn siya as list
        }
        public async Task<LoanModel> GetLoanbyIdAsync(int id) {
            return await _context.LoanModels.FindAsync(id); // fetching the specific record base sa pinasang id
        }

        public async Task<LoanModel> AddLoanAsync(LoanModel loan)
        {
            var loan1 = new LoanModel
            {
                ItemName = "Test",
                Description = "Test Description",
                Amount = 3000,
                Status = "Active",
                IssuedDate = DateTime.UtcNow,
                InterestRate = 3,
                RetentionPeriod = DateTime.UtcNow,
                CustomerID = 1
            };

            //loan.IssuedDate = DateTime.UtcNow; // set the attribute IssuedDate sa time kung kailan nacreate ito
            _context.LoanModels.Add(loan1); // paglagay ng loan sa db context

            _context.SaveChanges();
            
            //await _context.SaveChangesAsync(); // pagsasave ng loan na nakalagay sa db context sa database
            return loan;
        }

        public async Task<LoanModel> UpdateLoanAsync(LoanModel loan)
        {
            _context.Entry(loan).State = EntityState.Modified; // marking as modified para alam ni EF Core na inupdate ito ng user
            await _context.SaveChangesAsync(); // pagsasave ng updated na row sa database
            return loan;
        }

        public async Task DeleteLoanAsync(int id)
        {
            var loan = await _context.LoanModels.FindAsync(id);
            if (loan != null)
            {
                _context.LoanModels.Remove(loan);
                await _context.SaveChangesAsync();
            }
        }

    }
}
