using System.Collections.Generic;

namespace ExpenseTracker.Models.ViewModels
{
    public class MonthlyDashboardViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        
        public MonthlySummary Summary { get; set; } = new();
        
        public List<Expense> Expenses { get; set; } = new();
        
        public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
        
        public Dictionary<int, decimal> WeeklyBreakdown { get; set; } = new();
        
        public List<string> FrequentItems { get; set; } = new();
    }
}
