using Microsoft.AspNetCore.Mvc;

namespace Pawn_Vault___OOP.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
