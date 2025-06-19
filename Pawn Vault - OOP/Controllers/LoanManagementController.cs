using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanModel loan)
        {
            if (ModelState.IsValid)
            {
                await _loanRepository.AddLoanAsync(loan);
                return RedirectToAction(nameof(Index));
            }
            return View(loan);
        }

        
        public async Task <IActionResult> Edit(int id)
        {
             var loan = await _loanRepository.GetLoanbyIdAsync(id);
            if (loan == null)
            {
                return NotFound();
            }            
            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoanModel loan)
        {
            if (id != loan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid) 
            {
                await _loanRepository.UpdateLoanAsync(loan);
                return RedirectToAction(nameof(Index));
            }
            return View(loan);
        }

        public async Task<IActionResult> ViewLoan(int id)
        {
            var loan = await _loanRepository.GetLoanbyIdAsync(id);
            
            if (loan == null)
                {
                    return NotFound();
                }

            return View(loan);
        }

        public async Task<IActionResult> Delete (int id) 
        {
            await _loanRepository.DeleteLoanAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
