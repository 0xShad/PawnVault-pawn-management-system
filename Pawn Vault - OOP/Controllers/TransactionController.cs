using Microsoft.AspNetCore.Identity;
using Pawn_Vault___OOP.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Models;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Pawn_Vault___OOP.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionController(ICustomerRepository customerRepository, ILoanRepository loanRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _customerRepository = customerRepository;
            _loanRepository = loanRepository;
            _context = context;
            _userManager = userManager;
        }

        // GET: Transaction
        public IActionResult Index()
        {
            var payments = _context.Payments
                .Where(p => !p.IsDeleted)
                .Select(p => new {
                    p.PaymentID,
                    p.PaymentDate,
                    p.TransactionType,
                    CustomerName = p.Loan != null && p.Loan.Customer != null ? p.Loan.Customer.FullName : "",
                    ItemName = p.Loan != null ? p.Loan.ItemName : "",
                    Status = p.Loan != null ? p.Loan.Status : "",
                    p.Amount
                }).ToList();
            ViewBag.Payments = payments;
            return View();
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            ViewBag.Customers = _customerRepository.GetAllCustomers();
            // Get current user's full name
            string fullName = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var userManager = HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
                var user = userManager?.GetUserAsync(User).Result;
                if (user != null)
                {
                    fullName = $"{user.FirstName} {user.LastName}";
                }
            }
            ViewBag.HandledByName = fullName;
            return View();
        }

        [HttpGet]
        public JsonResult GetLoansByCustomer(int customerId)
        {
            var loans = _loanRepository.GetLoansByCustomerId(customerId)
                .Where(l => l.Status.ToLower() != "redeemed" && l.Status.ToLower() != "paid" && l.Status.ToLower() != "forfeited")
                .ToList();
            var result = new List<object>();
            foreach (var loan in loans)
            {
                result.Add(new { loanId = loan.LoanID, display = loan.ItemName + " (Due: " + loan.DueDate.ToString("yyyy-MM-dd") + ")" });
            }
            return Json(result);
        }

        public IActionResult ViewTransaction(int id)
        {
            var payment = _context.Payments
                .Where(p => p.PaymentID == id)
                .Select(p => new Payment {
                    PaymentID = p.PaymentID,
                    PaymentDate = p.PaymentDate,
                    TransactionType = p.TransactionType,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    ReferenceNumber = p.ReferenceNumber,
                    Notes = p.Notes,
                    HandledBy = p.HandledBy,
                    LoanID = p.LoanID,
                    Loan = p.Loan
                })
                .FirstOrDefault();
            if (payment == null)
                return NotFound();
            return View(payment);
        }

        // GET: Transaction/EditTransaction/5
        public IActionResult EditTransaction(int id)
        {
            var payment = _context.Payments
                .Include(p => p.Loan)
                .ThenInclude(l => l.Customer)
                .FirstOrDefault(p => p.PaymentID == id);
            if (payment == null)
                return NotFound();
            return View(payment);
        }

        [HttpGet]
        public JsonResult GetLoanDetails(int loanId)
        {
            var loan = _loanRepository.GetLoanbyIdAsync(loanId).Result;
            if (loan == null)
                return Json(null);

            var now = DateTime.Now;
            var isOverdue = now > loan.DueDate;
            var interest = loan.Amount * (decimal)(loan.InterestRate / 100.0);
            var paymentWithInterest = loan.Amount + interest;
            var overdueInterest = isOverdue ? interest : 0;
            var totalDue = loan.Amount + interest + overdueInterest;

            return Json(new {
                loanId = loan.LoanID,
                itemName = loan.ItemName,
                dueDate = loan.DueDate.ToString("yyyy-MM-dd"),
                amount = loan.Amount,
                interestRate = loan.InterestRate,
                paymentWithInterest = paymentWithInterest,
                paymentWithoutInterest = loan.Amount,
                isOverdue = isOverdue,
                overdueInterest = overdueInterest,
                totalDue = totalDue
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTransaction()
        {
            var form = Request.Form;
            // Validate required fields
            if (string.IsNullOrWhiteSpace(form["loanId"]))
            {
                ModelState.AddModelError("loanId", "Loan selection is required.");
            }
            if (string.IsNullOrWhiteSpace(form["amount"]))
            {
                ModelState.AddModelError("amount", "Amount is required.");
            }
            if (string.IsNullOrWhiteSpace(form["paymentMethod"]))
            {
                ModelState.AddModelError("paymentMethod", "Payment method is required.");
            }
            if (string.IsNullOrWhiteSpace(form["transactionType"]))
            {
                ModelState.AddModelError("transactionType", "Transaction type is required.");
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }

            int loanId = int.Parse(form["loanId"]);
            var loan = _context.Loans.FirstOrDefault(l => l.LoanID == loanId);
            if (loan == null)
            {
                ModelState.AddModelError("loanId", "Invalid loan selected.");
                return RedirectToAction("Create");
            }

            var user = _userManager.GetUserAsync(User).Result;
            string handledBy = user != null ? $"{user.FirstName} {user.LastName}" : "";

            var referenceNumber = form["referenceNumber"];
            if (string.IsNullOrWhiteSpace(referenceNumber))
            {
                referenceNumber = $"REF-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            }
            decimal amount = 0;
            if (!string.IsNullOrWhiteSpace(form["amount"]))
            {
                decimal.TryParse(form["amount"], out amount);
            }
            string paymentMethod = form["paymentMethod"].ToString() ?? string.Empty;
            string notes = form["notes"].ToString() ?? string.Empty;
            string transactionType = form["transactionType"].ToString() ?? string.Empty;
            DateTime paymentDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(form["transactionDate"]))
            {
                DateTime.TryParse(form["transactionDate"], out paymentDate);
            }

            // Validation for amount based on transaction type
            var interest = loan.Amount * (decimal)(loan.InterestRate / 100.0);
            if (transactionType == "redemption")
            {
                var expectedAmount = loan.Amount + interest;
                if (amount != expectedAmount)
                {
                    ModelState.AddModelError("amount", $"Amount must be equal to principal + interest: {expectedAmount:N2}");
                    return RedirectToAction("Create");
                }
            }
            else if (transactionType == "forfeit")
            {
                if (amount != 0)
                {
                    ModelState.AddModelError("amount", "Amount for forfeited transactions must be 0.");
                    return RedirectToAction("Create");
                }
            }

            var payment = new Payment
            {
                LoanID = loanId,
                Amount = amount,
                PaymentDate = paymentDate,
                PaymentMethod = paymentMethod,
                ReferenceNumber = referenceNumber.ToString(),
                Notes = notes,
                HandledBy = handledBy,
                TransactionType = transactionType,
                Balance = 0 // Set as needed
            };

            _context.Payments.Add(payment);

            // Update loan and inventory status only in their respective tables
            if (transactionType == "redemption")
            {
                loan.Status = "Paid";
                var inventoryItem = _context.InventoryItems.FirstOrDefault(i => i.LoanID == loan.LoanID);
                if (inventoryItem != null)
                {
                    inventoryItem.IsDeleted = true;
                    _context.InventoryItems.Update(inventoryItem);
                }
            }
            else if (transactionType == "forfeit")
            {
                loan.Status = "Forfeited";
                var inventoryItem = _context.InventoryItems.FirstOrDefault(i => i.LoanID == loan.LoanID);
                if (inventoryItem != null)
                {
                    // Do NOT set IsDeleted for forfeited items; just update status if needed
                    inventoryItem.Status = "Forfeited";
                    _context.InventoryItems.Update(inventoryItem);
                }
            }
            _context.Loans.Update(loan);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTransaction(Payment model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Invalid transaction data.");
                return View(model);
            }

            var payment = _context.Payments
                .Include(p => p.Loan)
                .ThenInclude(l => l.Customer)
                .FirstOrDefault(p => p.PaymentID == model.PaymentID);
            if (payment == null)
            {
                ModelState.AddModelError("", "Transaction not found.");
                return View(model);
            }

            var loan = _context.Loans.FirstOrDefault(l => l.LoanID == payment.LoanID);
            if (loan == null)
            {
                ModelState.AddModelError("", "Associated loan not found.");
                return View(model);
            }

            // Validation for amount based on transaction type
            var interest = loan.Amount * (decimal)(loan.InterestRate / 100.0);
            if (model.TransactionType == "redemption")
            {
                var expectedAmount = loan.Amount + interest;
                if (model.Amount != expectedAmount)
                {
                    ModelState.AddModelError("Amount", $"Amount must be equal to principal + interest: {expectedAmount:N2}");
                    return View(model);
                }
            }
            else if (model.TransactionType == "forfeit")
            {
                if (model.Amount != 0)
                {
                    ModelState.AddModelError("Amount", "Amount for forfeited transactions must be 0.");
                    return View(model);
                }
            }

            // Update payment fields
            payment.TransactionType = model.TransactionType;
            payment.Amount = model.Amount;
            payment.PaymentDate = model.PaymentDate;
            payment.PaymentMethod = model.PaymentMethod;
            payment.ReferenceNumber = model.ReferenceNumber;
            payment.Notes = model.Notes;
            payment.HandledBy = model.HandledBy;

            // Update loan and inventory status
            if (model.TransactionType == "redemption")
            {
                loan.Status = "Paid";
                var inventoryItem = _context.InventoryItems.FirstOrDefault(i => i.LoanID == loan.LoanID);
                if (inventoryItem != null)
                {
                    inventoryItem.IsDeleted = true;
                    _context.InventoryItems.Update(inventoryItem);
                }
            }
            else if (model.TransactionType == "forfeit")
            {
                loan.Status = "Forfeited";
                var inventoryItem = _context.InventoryItems.FirstOrDefault(i => i.LoanID == loan.LoanID);
                if (inventoryItem != null)
                {
                    inventoryItem.Status = "Forfeited";
                    inventoryItem.IsDeleted = false;
                    _context.InventoryItems.Update(inventoryItem);
                }
            }
            _context.Loans.Update(loan);
            _context.Payments.Update(payment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var payment = _context.Payments
                .Include(p => p.Loan)
                .ThenInclude(l => l.Payments)
                .FirstOrDefault(p => p.PaymentID == id);
            if (payment != null)
            {
                payment.IsDeleted = true;
                payment.Notes += " (Soft Deleted by Admin on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + ")";
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
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Transaction/ValidateAmount")]
        public JsonResult ValidateAmount(int loanId, string transactionType, decimal amount)
        {
            var loan = _context.Loans.FirstOrDefault(l => l.LoanID == loanId);
            if (loan == null)
                return Json(new { isValid = false });

            var interest = loan.Amount * (decimal)(loan.InterestRate / 100.0);
            if (transactionType == "redemption")
            {
                var expectedAmount = loan.Amount + interest;
                return Json(new { isValid = amount == expectedAmount });
            }
            else if (transactionType == "forfeit")
            {
                return Json(new { isValid = amount == 0 });
            }
            return Json(new { isValid = true });
        }
    }
}
