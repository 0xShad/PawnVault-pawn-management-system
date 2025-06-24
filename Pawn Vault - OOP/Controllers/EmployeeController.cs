using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Models.ViewModels;

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

            var model = staffUsers.Select(u => new StaffViewModel
            {
                Id = u.Id,
                Email = u.Email,
                Status = (u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow) ? "Inactive" : "Active" // if log-in or out
            }).ToList();

            return View(model);
        }
    }
}
