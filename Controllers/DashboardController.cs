using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.ViewModels;
using System.Collections.Generic;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ExpenseTrackerContext _context;

        public DashboardController(ExpenseTrackerContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index(int? month, int? year)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var viewModel = await GetMonthlyDashboardData(currentMonth, currentYear);

            return View(viewModel);
        }

        // GET: Dashboard/MonthlyRecap
        public async Task<IActionResult> MonthlyRecap(int? month, int? year)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var viewModel = await GetMonthlyDashboardData(currentMonth, currentYear);

            return View(viewModel);
        }

        // Helper method to compile monthly dashboard data
        private async Task<MonthlyDashboardViewModel> GetMonthlyDashboardData(int month, int year)
        {
            var monthName = new DateTime(year, month, 1).ToString("MMMM yyyy");

            // Get or create monthly summary
            var summary = await _context.MonthlySummaries
                .FirstOrDefaultAsync(m => m.Month == month && m.Year == year);

            if (summary == null)
            {
                summary = new MonthlySummary
                {
                    Month = month,
                    Year = year,
                    Salary = 0,
                    AdditionalIncome = 0
                };
            }

            // Get all expenses for the month
            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.Month == month && e.Year == year)
                .OrderByDescending(e => e.Date)
                .ThenBy(e => e.WeekNumber)
                .ToListAsync();

            // Calculate total spending
            summary.TotalSpending = expenses.Sum(e => e.Amount);

            // Category breakdown
            var categoryBreakdown = expenses
                .GroupBy(e => e.Category.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(e => e.Amount)
                );

            // Weekly breakdown
            var weeklyBreakdown = expenses
                .GroupBy(e => e.WeekNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(e => e.Amount)
                );

            // Frequent items
            var frequentItems = expenses
                .GroupBy(e => e.ItemName)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList();

            var viewModel = new MonthlyDashboardViewModel
            {
                Month = month,
                Year = year,
                MonthName = monthName,
                Summary = summary,
                Expenses = expenses,
                CategoryBreakdown = categoryBreakdown,
                WeeklyBreakdown = weeklyBreakdown,
                FrequentItems = frequentItems
            };

            return viewModel;
        }

        // GET: Dashboard/EditSalary
        public async Task<IActionResult> EditSalary(int? month, int? year)
        {
            var currentMonth = month ?? DateTime.Now.Month;
            var currentYear = year ?? DateTime.Now.Year;

            var summary = await _context.MonthlySummaries
                .FirstOrDefaultAsync(m => m.Month == currentMonth && m.Year == currentYear);

            if (summary == null)
            {
                summary = new MonthlySummary
                {
                    Month = currentMonth,
                    Year = currentYear,
                    Salary = 0,
                    AdditionalIncome = 0
                };
            }

            ViewBag.MonthName = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");

            return View(summary);
        }

        // POST: Dashboard/EditSalary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSalary(MonthlySummary summary)
        {
            if (ModelState.IsValid)
            {
                var existingSummary = await _context.MonthlySummaries
                    .FirstOrDefaultAsync(m => m.Month == summary.Month && m.Year == summary.Year);

                if (existingSummary != null)
                {
                    existingSummary.Salary = summary.Salary;
                    existingSummary.AdditionalIncome = summary.AdditionalIncome;
                    existingSummary.Notes = summary.Notes;
                    existingSummary.UpdatedDate = DateTime.UtcNow;
                    
                    _context.Update(existingSummary);
                }
                else
                {
                    summary.CreatedDate = DateTime.UtcNow;
                    summary.UpdatedDate = DateTime.UtcNow;
                    _context.Add(summary);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { month = summary.Month, year = summary.Year });
            }

            ViewBag.MonthName = new DateTime(summary.Year, summary.Month, 1).ToString("MMMM yyyy");
            return View(summary);
        }


        // API endpoint for chart data
        [HttpGet]
        public async Task<JsonResult> GetCategoryChartData(int month, int year)
        {
            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.Month == month && e.Year == year)
                .ToListAsync();

            var chartData = expenses
                .GroupBy(e => e.Category.Name)
                .Select(g => new
                {
                    category = g.Key,
                    amount = g.Sum(e => e.Amount),
                    color = g.First().Category.ColorCode
                })
                .OrderByDescending(x => x.amount)
                .ToList();

            return Json(chartData);
        }

        // API endpoint for weekly trend data
        [HttpGet]
        public async Task<JsonResult> GetWeeklyTrendData(int month, int year)
        {
            var expenses = await _context.Expenses
                .Where(e => e.Month == month && e.Year == year)
                .ToListAsync();

            var weeklyData = expenses
                .GroupBy(e => e.WeekNumber)
                .Select(g => new
                {
                    week = g.Key,
                    amount = g.Sum(e => e.Amount)
                })
                .OrderBy(x => x.week)
                .ToList();

            return Json(weeklyData);
        }

        // Year-over-year comparison
        [HttpGet]
        public async Task<IActionResult> YearlyComparison(int? year)
        {
            var currentYear = year ?? DateTime.Now.Year;

            var yearlyData = new List<YearlyMonthData>();

            for (int month = 1; month <= 12; month++)
            {
                var expenses = await _context.Expenses
                    .Where(e => e.Month == month && e.Year == currentYear)
                    .ToListAsync();

                var summary = await _context.MonthlySummaries
                    .FirstOrDefaultAsync(m => m.Month == month && m.Year == currentYear);

                yearlyData.Add(new YearlyMonthData
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    MonthNumber = month,
                    TotalSpending = expenses.Sum(e => e.Amount),
                    Income = summary?.TotalIncome ?? 0,
                    Balance = (summary?.TotalIncome ?? 0) - expenses.Sum(e => e.Amount)
                });
            }

            ViewBag.Year = currentYear;
            return View(yearlyData);
        }
    }
}
