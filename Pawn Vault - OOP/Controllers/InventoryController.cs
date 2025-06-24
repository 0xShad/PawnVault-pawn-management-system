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

        public async Task<IActionResult> Index(string search = "", string sort = "az", string status = "", string source = "", string category = "", string condition = "", int page = 1)
        {
            var items = await _inventoryRepository.GetAllItemsAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                items = items.Where(i =>
                    (i.Name ?? "").ToLower().Contains(lower) ||
                    (i.Category ?? "").ToLower().Contains(lower) ||
                    (i.Description ?? "").ToLower().Contains(lower)
                ).ToList();
            }

            // Status filter
            if (!string.IsNullOrEmpty(status))
            {
                items = items.Where(i => i.Status.ToLower() == status.ToLower()).ToList();
            }

            // Source filter (from loan or admin)
            if (!string.IsNullOrEmpty(source))
            {
                if (source == "loan")
                    items = items.Where(i => i.LoanID != null).ToList();
                else if (source == "admin")
                    items = items.Where(i => i.LoanID == null).ToList();
            }

            // Category filter
            if (!string.IsNullOrEmpty(category))
            {
                items = items.Where(i => (i.Category ?? "").ToLower() == category.ToLower()).ToList();
            }

            // Condition filter
            if (!string.IsNullOrEmpty(condition))
            {
                items = items.Where(i => (i.Condition ?? "").ToLower() == condition.ToLower()).ToList();
            }

            // Sort
            items = sort == "za"
                ? items.OrderByDescending(i => i.Name).ToList()
                : items.OrderBy(i => i.Name).ToList();

            // Pagination
            int pageSize = 10;
            int total = items.Count();
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Status = status;
            ViewBag.Source = source;
            ViewBag.Category = category;
            ViewBag.Condition = condition;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Total = total;

            // For dropdowns, always use all items for unique values, not filtered items
            var allItems = await _inventoryRepository.GetAllItemsAsync();
            ViewBag.AllCategories = allItems.Select(i => i.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToList();
            ViewBag.AllConditions = allItems.Select(i => i.Condition)
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToList();

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

        public async Task<IActionResult> View(int id)
        {
            var item = await _inventoryRepository.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
    }
}
