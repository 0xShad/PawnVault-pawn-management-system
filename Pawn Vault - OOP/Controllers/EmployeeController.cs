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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.GetUsersInRoleAsync("Staff");
            var model = users.Select(u => new StaffViewModel
            {
                Id = u.Id,
                Email = u.Email,
                Status = u.LockoutEnd.HasValue && u.LockoutEnd > DateTime.Now ? "Inactive" : "Active"
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new StaffViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active"
            };

            return View("Edit", model); //  Use Edit.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StaffViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user.");
                return View("Edit", model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update password.");
                    return View("Edit", model);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new StaffViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active"
            };

            return View("Delete", model); // Use Delete.cshtml
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to delete user.");
                return View("Delete", new StaffViewModel { Id = id });
            }

            return RedirectToAction("Index");
        }

    }
}
