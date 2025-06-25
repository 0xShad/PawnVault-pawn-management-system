using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Data;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryItem>> GetAllItemsAsync()
        {
            // Show items that are not deleted and not redeemed, paid, or overdue (forfeited items are now included)
            return await _context.InventoryItems
                .Where(i => !i.IsDeleted && i.Status.ToLower() != "redeemed" && i.Status.ToLower() != "paid" && i.Status.ToLower() != "overdue")
                .ToListAsync();
        }

        public async Task<InventoryItem?> GetItemByIdAsync(int id)
        {
            return await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }

        public async Task<InventoryItem> AddItemAsync(InventoryItem item)
        {
            item.DateAdded = DateTime.UtcNow;
            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InventoryItem> UpdateItemAsync(InventoryItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalItemsCountAsync()
        {
            return await _context.InventoryItems.CountAsync();
        }

        public async Task<int> GetOnLoanItemsCountAsync()
        {
            return await _context.InventoryItems.CountAsync(i => i.Status.ToLower() == "on loan");
        }

        public async Task<decimal> GetTotalValueAsync()
        {
            return await _context.InventoryItems.SumAsync(i => i.EstimatedValue);
        }
    }
}