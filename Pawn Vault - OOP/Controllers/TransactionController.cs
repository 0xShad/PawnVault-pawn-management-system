using Microsoft.AspNetCore.Mvc;

namespace Pawn_Vault___OOP.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        public IActionResult Index()
        {
            return View();
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            return View();
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

       
    }
}
