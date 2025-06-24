using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Models;
using Pawn_Vault___OOP.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Pawn_Vault___OOP.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private const int PageSize = 10;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IActionResult> Index(string search = "", string sort = "az", string status = "", int page = 1)
        {
            var customers = await _customerRepository.GetAllCustomersAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                customers = customers.Where(c =>
                    (c.CustomerFN ?? "").ToLower().Contains(lower) ||
                    (c.CustomerLN ?? "").ToLower().Contains(lower) ||
                    c.TelephoneNo.ToString().Contains(lower) ||
                    (c.Street ?? "").ToLower().Contains(lower) ||
                    (c.Municipality ?? "").ToLower().Contains(lower)
                ).ToList();
            }

            // Status Filter
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Active")
                    customers = customers.Where(c => c.Loans.Any(l => l.Status == "Active")).ToList();
                else if (status == "Inactive")
                    customers = customers.Where(c => c.Loans.Any() && !c.Loans.Any(l => l.Status == "Active")).ToList();
                else if (status == "NoLoans")
                    customers = customers.Where(c => !c.Loans.Any()).ToList();
            }

            // Sort
            customers = sort == "za"
                ? customers.OrderByDescending(c => c.CustomerFN).ThenByDescending(c => c.CustomerLN).ToList()
                : customers.OrderBy(c => c.CustomerFN).ThenBy(c => c.CustomerLN).ToList();

            // Remove soft-deleted loans from each customer for correct loan count
            foreach (var c in customers)
            {
                c.Loans = c.Loans?.Where(l => !l.IsDeleted).ToList() ?? new List<Loan>();
            }

            // Pagination
            int total = customers.Count;
            int totalPages = (int)System.Math.Ceiling(total / (double)PageSize);
            customers = customers.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Status = status;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Total = total;

            return View(customers);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate by first and last name (string only)
                var allCustomers = await _customerRepository.GetAllCustomersAsync();
                bool exists = allCustomers.Any(c =>
                    (c.CustomerFN ?? "").Trim().ToLower() == (customer.CustomerFN ?? "").Trim().ToLower() &&
                    (c.CustomerLN ?? "").Trim().ToLower() == (customer.CustomerLN ?? "").Trim().ToLower()
                );
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "A customer with this first and last name is already registered.");
                    return View(customer);
                }
                await _customerRepository.AddCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var customer = (await _customerRepository.GetAllCustomersAsync()).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["DeleteUrl"] = Url.Action("DeleteConfirmed", new { id });
            ViewData["Message"] = $"Are you sure you want to delete {customer.CustomerFN} {customer.CustomerLN}? This action cannot be undone.";
            return PartialView("~/Views/Shared/_DeleteModal.cshtml", customer);
        }

        // POST: Customer/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerRepository.SoftDeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var customer = (await _customerRepository.GetAllCustomersAsync()).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }
            // Filter out soft-deleted loans and inventory items
            customer.Loans = customer.Loans?.Where(l => !l.IsDeleted).ToList() ?? new List<Loan>();
            foreach (var loan in customer.Loans)
            {
                loan.InventoryItems = loan.InventoryItems?.Where(i => !i.IsDeleted).ToList() ?? new List<InventoryItem>();
            }
            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customer = (await _customerRepository.GetAllCustomersAsync()).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                // Only update if not deleted
                var existing = (await _customerRepository.GetAllCustomersAsync()).FirstOrDefault(c => c.CustomerID == id);
                if (existing == null)
                {
                    return NotFound();
                }
                // Update fields
                existing.CustomerFN = customer.CustomerFN;
                existing.CustomerLN = customer.CustomerLN;
                existing.TelephoneNo = customer.TelephoneNo;
                existing.Street = customer.Street;
                existing.Municipality = customer.Municipality;
                existing.ZipCode = customer.ZipCode;
                await _customerRepository.UpdateCustomerAsync(existing);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
    }
}
