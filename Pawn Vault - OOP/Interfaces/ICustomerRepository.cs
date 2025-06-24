using Pawn_Vault___OOP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pawn_Vault___OOP.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddCustomerAsync(Customer customer);
        Task<List<Customer>> GetAllCustomersAsync();
        List<Customer> GetAllCustomers();
        Task SoftDeleteCustomerAsync(int customerId);
        Task UpdateCustomerAsync(Customer customer);
    }
}