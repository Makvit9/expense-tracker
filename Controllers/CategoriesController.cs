using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ExpenseTrackerContext _context;

        public CategoriesController(ExpenseTrackerContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
            
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            var maxDisplayOrder = _context.Categories.Any() 
                ? _context.Categories.Max(c => c.DisplayOrder) 
                : 0;

            var category = new Category
            {
                DisplayOrder = maxDisplayOrder + 1,
                IsActive = true,
                ColorCode = "#" + System.DateTime.Now.Ticks.ToString("X6").Substring(0, 6)
            };

            return View(category);
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            ModelState.Remove("Expenses");

            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Expenses");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (category == null)
            {
                return NotFound();
            }

            // Check if category has expenses
            var hasExpenses = await _context.Expenses.AnyAsync(e => e.CategoryId == id);
            ViewBag.HasExpenses = hasExpenses;

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            if (category != null)
            {
                // Check if category has expenses
                var hasExpenses = await _context.Expenses.AnyAsync(e => e.CategoryId == id);
                
                if (hasExpenses)
                {
                    // Instead of deleting, deactivate the category
                    category.IsActive = false;
                    _context.Update(category);
                }
                else
                {
                    // Safe to delete if no expenses
                    _context.Categories.Remove(category);
                }
                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Categories/Reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int id, string direction)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var currentOrder = category.DisplayOrder;
            Category? swapCategory = null;

            if (direction == "up" && currentOrder > 1)
            {
                swapCategory = await _context.Categories
                    .Where(c => c.DisplayOrder == currentOrder - 1)
                    .FirstOrDefaultAsync();
            }
            else if (direction == "down")
            {
                swapCategory = await _context.Categories
                    .Where(c => c.DisplayOrder == currentOrder + 1)
                    .FirstOrDefaultAsync();
            }

            if (swapCategory != null)
            {
                category.DisplayOrder = swapCategory.DisplayOrder;
                swapCategory.DisplayOrder = currentOrder;

                _context.Update(category);
                _context.Update(swapCategory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
