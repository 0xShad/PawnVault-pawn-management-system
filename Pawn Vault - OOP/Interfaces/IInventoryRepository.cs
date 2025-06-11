using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryItem>> GetAllItemsAsync();
        Task<InventoryItem?> GetItemByIdAsync(int id);
        Task<InventoryItem> AddItemAsync(InventoryItem item);
        Task<InventoryItem> UpdateItemAsync(InventoryItem item);
        Task DeleteItemAsync(int id);
        Task<int> GetTotalItemsCountAsync();
        Task<int> GetOnLoanItemsCountAsync();
        Task<decimal> GetTotalValueAsync();
    }
} 