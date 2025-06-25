using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawn_Vault___OOP.Models.ViewModels;
using System.Security.Claims;

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
            var model = new List<StaffViewModel>();

            foreach (var user in users)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var empFN = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                var empLN = claims.FirstOrDefault(c => c.Type == "LastName")?.Value;

                model.Add(new StaffViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active",
                    EmpFN = empFN,
                    EmpLN = empLN
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var claims = await _userManager.GetClaimsAsync(user);
            var empFN = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var empLN = claims.FirstOrDefault(c => c.Type == "LastName")?.Value;

            var model = new StaffViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active",
                EmpFN = empFN,
                EmpLN = empLN
            };

            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StaffViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;

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

            var claims = await _userManager.GetClaimsAsync(user);

            var existingFN = claims.FirstOrDefault(c => c.Type == "FirstName");
            if (existingFN != null) await _userManager.RemoveClaimAsync(user, existingFN);
            await _userManager.AddClaimAsync(user, new Claim("FirstName", model.EmpFN ?? ""));

            var existingLN = claims.FirstOrDefault(c => c.Type == "LastName");
            if (existingLN != null) await _userManager.RemoveClaimAsync(user, existingLN);
            await _userManager.AddClaimAsync(user, new Claim("LastName", model.EmpLN ?? ""));

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var claims = await _userManager.GetClaimsAsync(user);
            var empFN = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
            var empLN = claims.FirstOrDefault(c => c.Type == "LastName")?.Value;

            var model = new StaffViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now ? "Inactive" : "Active",
                EmpFN = empFN,
                EmpLN = empLN
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
        public async Task<IActionResult> Create(StaffViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already taken.");
                return View(model);
            }

            var newUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email
            };

            var createResult = await _userManager.CreateAsync(newUser, model.NewPassword ?? "#Temp123");
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await _userManager.AddToRoleAsync(newUser, "Staff");

            if (!string.IsNullOrEmpty(model.EmpFN))
                await _userManager.AddClaimAsync(newUser, new Claim("FirstName", model.EmpFN));
            if (!string.IsNullOrEmpty(model.EmpLN))
                await _userManager.AddClaimAsync(newUser, new Claim("LastName", model.EmpLN));

            return RedirectToAction("Index");
        }
    }
}
