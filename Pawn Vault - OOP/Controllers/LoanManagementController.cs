using Microsoft.AspNetCore.Mvc;

namespace Pawn_Vault___OOP.Controllers
{
    public class LoanManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
