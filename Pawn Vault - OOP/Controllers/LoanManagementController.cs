using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Interfaces;

namespace Pawn_Vault___OOP.Controllers
{
    [Authorize]
    public class LoanManagementController : Controller
    {
        private readonly ILoanRepository _loanRepository;

        public LoanManagementController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _loanRepository.GetAllLoansAsync();
            return View(loans);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult ViewLoan()
        {
            return View();
        }
    }
}
