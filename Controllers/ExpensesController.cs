using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ExpenseTrackerContext _context;

        public ExpensesController(ExpenseTrackerContext context)
        {
            _context = context;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(int? month, int? year)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.Month == currentMonth && e.Year == currentYear)
                .OrderByDescending(e => e.Date)
                .ThenBy(e => e.WeekNumber)
                .ToListAsync();

            ViewBag.CurrentMonth = currentMonth;
            ViewBag.CurrentYear = currentYear;
            ViewBag.MonthName = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");

            return View(expenses);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create(int? month, int? year)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;
            var currentDate = DateTime.UtcNow;

            var expense = new Expense
            {
                Date = currentDate,
                Month = currentMonth,
                Year = currentYear,
                WeekNumber = GetWeekOfMonth(currentDate)
            };

            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder), "Id", "Name");
            ViewBag.FrequentItems = GetFrequentItems();
            
            return View(expense);
        }

[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            Console.WriteLine("========== CREATE EXPENSE POST CALLED ==========");
            Console.WriteLine($"Date: {expense.Date}");
            Console.WriteLine($"ItemName: {expense.ItemName}");
            Console.WriteLine($"CategoryId: {expense.CategoryId}");
            Console.WriteLine($"Amount: {expense.Amount}");
            
            // Remove navigation properties from validation
            ModelState.Remove("Category");
            
            if (ModelState.IsValid)
            {
                // Auto-calculate week number based on date
                expense.WeekNumber = GetWeekOfMonth(expense.Date);
                expense.Month = expense.Date.Month;
                expense.Year = expense.Date.Year;
                expense.CreatedDate = DateTime.UtcNow;
                expense.Date = DateTime.SpecifyKind(expense.Date, DateTimeKind.Utc);

                _context.Add(expense);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"✅ Expense saved successfully: {expense.ItemName}");
                return RedirectToAction(nameof(Index), new { month = expense.Month, year = expense.Year });
            }
            
            Console.WriteLine("❌ ModelState is INVALID. Errors:");
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"  {key}: {error.ErrorMessage}");
                    }
                }
            }
            
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder), "Id", "Name", expense.CategoryId);
            ViewBag.FrequentItems = GetFrequentItems();
            
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder), "Id", "Name", expense.CategoryId);
            ViewBag.FrequentItems = GetFrequentItems();
            
            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            // Remove navigation properties from validation
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalculate week number if date changed 

                    expense.WeekNumber = GetWeekOfMonth(expense.Date);
                    expense.Month = expense.Date.Month;
                    expense.Year = expense.Date.Year;
                    expense.Date = DateTime.SpecifyKind(expense.Date, DateTimeKind.Utc);    
                    expense.CreatedDate = DateTime.SpecifyKind(expense.CreatedDate, DateTimeKind.Utc);

                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { month = expense.Month, year = expense.Year });
            }
            
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder), "Id", "Name", expense.CategoryId);
            ViewBag.FrequentItems = GetFrequentItems();
            
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                var month = expense.Month;
                var year = expense.Year;
                
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index), new { month = month, year = year });
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method to get week number of a date within a month
        private int GetWeekOfMonth(DateTime date)
        {
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            int dayOfMonth = date.Day;
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            
            // Calculate week number (1-based)
            int weekNumber = (dayOfMonth + firstDayOfWeek - 1) / 7 + 1;
            
            return Math.Min(weekNumber, 5); // Cap at week 5
        }

        // Helper method to get frequently used items
        private object GetFrequentItems()
        {
            var frequentItems = _context.Expenses
                .GroupBy(e => e.ItemName)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList();

            return frequentItems;
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }

        // API endpoint for autocomplete
        [HttpGet]
        public JsonResult GetFrequentItemsJson(string term)
        {
            var items = _context.Expenses
                .Where(e => e.ItemName.Contains(term))
                .GroupBy(e => e.ItemName)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList();

            return Json(items);
        }
    }
}
