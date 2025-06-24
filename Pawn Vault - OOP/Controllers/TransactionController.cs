using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Models;
using Pawn_Vault___OOP.Interfaces;

namespace Pawn_Vault___OOP.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILoanRepository _loanRepository;

        public TransactionController(ICustomerRepository customerRepository, ILoanRepository loanRepository)
        {
            _customerRepository = customerRepository;
            _loanRepository = loanRepository;
        }

        // GET: Transaction
        public IActionResult Index()
        {
            return View();
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            ViewBag.Customers = _customerRepository.GetAllCustomers();
            // Optionally: ViewBag.StaffList, ViewBag.GeneratedReference
            return View();
        }

        [HttpGet]
        public JsonResult GetLoansByCustomer(int customerId)
        {
            var loans = _loanRepository.GetLoansByCustomerId(customerId)
                .Select(l => new { loanId = l.LoanID, display = l.ItemName + " (Due: " + l.DueDate.ToString("yyyy-MM-dd") + ")" });
            return Json(loans);
        }

        public IActionResult ViewTransaction()
        {
            // TODO: Fetch transaction details from database
            return View();
        }

        // GET: Transaction/EditTransaction/5
        public IActionResult EditTransaction()
        {
            // TODO: Fetch transaction details from database
            return View();
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
    }
}
