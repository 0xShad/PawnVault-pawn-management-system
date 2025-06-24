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

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Customer) // pag include ng Customer para makuha ang related na customer data sa LoanModel
                .Where(l => !l.IsDeleted)
                .ToListAsync(); // fetching all record atsaka irereturn siya as list
        }
        public async Task<Loan> GetLoanbyIdAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.Customer) 
                .FirstOrDefaultAsync(l => l.LoanID == id && !l.IsDeleted);
        }

        public async Task<Loan> AddLoanAsync(Loan loan)
        {

            loan.IssuedDate = DateTime.UtcNow; // set the attribute IssuedDate sa time kung kailan nacreate ito
            _context.Loans.Add(loan); // paglagay ng loan sa db context
            
            await _context.SaveChangesAsync(); // pagsasave ng loan na nakalagay sa db context sa database
            return loan;
        }

        public async Task<Loan> UpdateLoanAsync(Loan loan)
        {
            _context.Entry(loan).State = EntityState.Modified; // marking as modified para alam ni EF Core na inupdate ito ng user
            await _context.SaveChangesAsync(); // pagsasave ng updated na row sa database
            return loan;
        }

        public async Task DeleteLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                loan.IsDeleted = true;
                _context.Entry(loan).State = EntityState.Modified;
                // Soft delete all related inventory items
                var relatedItems = _context.InventoryItems.Where(i => i.LoanID == loan.LoanID);
                foreach (var item in relatedItems)
                {
                    item.IsDeleted = true;
                    _context.Entry(item).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }
        }

    }
}
