using Pawn_Vault___OOP.Data;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Pawn_Vault___OOP.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            if (customer.DateAdded == default)
            {
                customer.DateAdded = DateTime.UtcNow;
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Where(c => !c.IsDeleted)
                .Include(c => c.Loans)
                    .ThenInclude(l => l.Payments)
                .ToListAsync();
        }

        public List<Customer> GetAllCustomers()
        {
            return GetAllCustomersAsync().Result;
        }

        public async Task SoftDeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null && !customer.IsDeleted)
            {
                customer.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}