using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Data;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pawn_Vault___OOP.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Where(p => !p.IsDeleted)
                .Include(p => p.Loan).ThenInclude(l => l.Customer)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Loan).ThenInclude(l => l.Customer)
                .FirstOrDefaultAsync(p => p.PaymentID == id && !p.IsDeleted);
        }

        public async Task<Payment> AddPaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.Include(p => p.Loan).ThenInclude(l => l.Payments).FirstOrDefaultAsync(p => p.PaymentID == id);
            if (payment != null)
            {
                payment.IsDeleted = true;
                _context.Entry(payment).State = EntityState.Modified;

                // Soft delete related loan if exists
                if (payment.Loan != null)
                {
                    payment.Loan.IsDeleted = true;
                    _context.Entry(payment.Loan).State = EntityState.Modified;

                    // Soft delete all payments for this loan
                    foreach (var pay in payment.Loan.Payments)
                    {
                        pay.IsDeleted = true;
                        _context.Entry(pay).State = EntityState.Modified;
                    }

                    // Soft delete all related inventory items
                    var relatedItems = _context.InventoryItems.Where(i => i.LoanID == payment.Loan.LoanID);
                    foreach (var item in relatedItems)
                    {
                        item.IsDeleted = true;
                        _context.Entry(item).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
