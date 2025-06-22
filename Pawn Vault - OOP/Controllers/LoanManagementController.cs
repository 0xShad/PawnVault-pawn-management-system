using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Controllers
{
    [Authorize] // sinisguradong mga naka log in lang yung makakaaccess
    public class LoanManagementController : Controller
    {
        private readonly ILoanRepository _loanRepository; 

        public LoanManagementController(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }


        // Pagpapakita ng lahat ng loan sa Index View
        public async Task<IActionResult> Index()
        {
            var loans = await _loanRepository.GetAllLoansAsync();
            return View(loans);
        }

        // ===== Create Logic =====
        public IActionResult Create() // pagpapakita lang ng Create Page
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanModel loan) // Logic sa pagcreate ng loan
        {
            if (ModelState.IsValid)
            {
                loan.Status = "Active"; // default status ng loan kapag na create
                loan.IssuedDate = DateTime.UtcNow;
                await _loanRepository.AddLoanAsync(loan);
            }
            // return View(loan);
            return RedirectToAction("Index");
        }

        // ===== EDIT LOGIC =====
        public async Task <IActionResult> Edit(int id) // pagpapakita ng edit loan page
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
        public async Task<IActionResult> Edit(int id, LoanModel loan) // Pag-uupdate ng edited row sa database
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
    }
}
