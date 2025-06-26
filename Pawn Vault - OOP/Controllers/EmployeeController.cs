using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Areas.Identity.Data;
using Pawn_Vault___OOP.Models.ViewModels;

namespace Pawn_Vault___OOP.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.GetUsersInRoleAsync("Staff");
            var model = new List<StaffViewModel>();

            foreach (var user in users)
            {
                model.Add(new StaffViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active",
                    EmpFN = user.FirstName,
                    EmpLN = user.LastName
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new StaffViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active",
                EmpFN = user.FirstName,
                EmpLN = user.LastName
            };

            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StaffViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return NotFound();
            }
            
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.EmpFN;
            user.LastName = model.EmpLN;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user.");
                return View("Edit", model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!resetResult.Succeeded)
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
            if (user == null) return NotFound();

            var model = new StaffViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active",
                EmpFN = user.FirstName,
                EmpLN = user.LastName
            };

            return View("Delete", model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to delete user.");
                return View("Delete", new StaffViewModel { Id = id });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already taken.");
                return View(model);
            }

            var newUser = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var createResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, "Staff");
            if (!roleResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to assign Staff role to user.");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Employee {model.FirstName} {model.LastName} has been successfully created.";
            return RedirectToAction("Index");
        }
    }
}
