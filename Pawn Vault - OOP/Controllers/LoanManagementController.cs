using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Controllers
{
    [Authorize] 
    public class LoanManagementController : Controller
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public LoanManagementController(ILoanRepository loanRepository, ICustomerRepository customerRepository, IInventoryRepository inventoryRepository)
        {
            _loanRepository = loanRepository;
            _customerRepository = customerRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IActionResult> Index(string searchString, string sort = "az", string status = "", int pageNumber = 1, int pageSize = 10)
        {
            var loans = await _loanRepository.GetAllLoansAsync();

            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                loans = loans.Where(l =>
                    (l.ItemName != null && l.ItemName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (l.TransactionCode != null && l.TransactionCode.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (l.Customer != null && (
                        (l.Customer.CustomerFN != null && l.Customer.CustomerFN.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (l.Customer.CustomerLN != null && l.Customer.CustomerLN.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    ))
                );
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                loans = loans.Where(l => l.Status == status);
            }

            // Sort
            loans = sort == "za"
                ? loans.OrderByDescending(l => l.ItemName).ThenByDescending(l => l.LoanID)
                : loans.OrderBy(l => l.ItemName).ThenBy(l => l.LoanID);

            // Pagination
            int totalCount = loans.Count();
            int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            loans = loans.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageCount = pageCount;
            ViewBag.Sort = sort;
            ViewBag.Status = status;

            return View(loans.ToList());
        }

        // ===== Create Logic =====
        public async Task<IActionResult> Create() 
        {
            // Get all active customers for dropdown
            var customers = await _customerRepository.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers.Where(c => !c.IsDeleted), "CustomerID", "FullName");
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.TransactionCode = GenerateTransactionCode();
            
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loan loan) 
        {
            // Remove debug error message from ModelState
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "UserId is null or empty. User.Identity.IsAuthenticated: " + User.Identity?.IsAuthenticated);
            }
            else
            {
                loan.UserId = userId;
            }
            // Always set TransactionCode if missing
            if (string.IsNullOrWhiteSpace(loan.TransactionCode))
            {
                loan.TransactionCode = GenerateTransactionCode();
            }
            if (ModelState.IsValid)
            {
                loan.Status = "Active";
                loan.IssuedDate = DateTime.UtcNow;
                try
                {
                    await _loanRepository.AddLoanAsync(loan);
                    // Create InventoryItem for this loan
                    var inventoryItem = new InventoryItem
                    {
                        Name = loan.ItemName,
                        Description = loan.Description,
                        EstimatedValue = loan.EstimatedValue,
                        LoanAmount = loan.Amount,
                        Status = "on loan",
                        DateAdded = DateTime.UtcNow,
                        LoanID = loan.LoanID
                    };
                    await _inventoryRepository.AddItemAsync(inventoryItem);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error saving loan: {ex.Message}");
                }
            }
            else
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                if (allErrors.Any())
                {
                    ModelState.AddModelError(string.Empty, "Validation errors: " + string.Join(" | ", allErrors));
                }
            }
            var customers = await _customerRepository.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers.Where(c => !c.IsDeleted), "CustomerID", "FullName");
            return View(loan);
        }

        // ===== EDIT LOGIC =====
        public async Task <IActionResult> Edit(int id) 
        {
             var loan = await _loanRepository.GetLoanbyIdAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            
            // Get customers for dropdown
            var customers = await _customerRepository.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers.Where(c => !c.IsDeleted), "CustomerID", "FullName");
            
            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Loan loan) 
        {
            if (id != loan.LoanID)
            {
                return NotFound();
            }

            if (ModelState.IsValid) 
            {
                await _loanRepository.UpdateLoanAsync(loan);
                return RedirectToAction(nameof(Index));
            }
            
            // If validation fails, reload the customer dropdown
            var customers = await _customerRepository.GetAllCustomersAsync();
            ViewBag.Customers = new SelectList(customers.Where(c => !c.IsDeleted), "CustomerID", "FullName");
            
            return View(loan);
        }

        // ===== VIEW LOAN DETAILS LOGIC =====
        public async Task<IActionResult> ViewLoan(int id)
        {
            var loan = await _loanRepository.GetLoanbyIdAsync(id);
            
            if (loan == null)
                {
                    return NotFound();
                }

            return View(loan);
        }

        // ===== DELETE LOAN LOGIC ======
        public async Task<IActionResult> Delete (int id) 
        {
            await _loanRepository.DeleteLoanAsync(id);
            return RedirectToAction(nameof(Index));
        }
        
        // Helper method to generate transaction code
        private string GenerateTransactionCode()
        {
            var now = DateTime.Now;
            var random = new Random();
            var randomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"LOAN-{now:yyyy}-{randomString}";
        }
    }
}
