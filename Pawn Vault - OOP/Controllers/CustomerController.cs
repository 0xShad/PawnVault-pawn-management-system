using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Models;
using System;

namespace Pawn_Vault___OOP.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Include(c => c.Loans)
                .ThenInclude(l => l.Payments)
                .ToListAsync();

            return View(customers);
        }
    }
}
