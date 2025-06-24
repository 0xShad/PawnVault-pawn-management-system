using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Pawn_Vault___OOP.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            return View(staffUsers); // expects Views/Employee/Index.cshtml
        }
    }
}
