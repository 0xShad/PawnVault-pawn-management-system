using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;

namespace Pawn_Vault___OOP.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _inventoryRepository.GetAllItemsAsync();
            return View(items);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                await _inventoryRepository.AddItemAsync(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inventoryRepository.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryItem item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _inventoryRepository.UpdateItemAsync(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _inventoryRepository.DeleteItemAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
