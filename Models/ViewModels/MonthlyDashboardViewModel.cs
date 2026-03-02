using System.Collections.Generic;

namespace ExpenseTracker.Models.ViewModels
{
    public class MonthlyDashboardViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        
        public MonthlySummary Summary { get; set; }
        
        public List<Expense> Expenses { get; set; }
        
        public Dictionary<string, decimal> CategoryBreakdown { get; set; }
        
        public Dictionary<int, decimal> WeeklyBreakdown { get; set; }
        
        public List<string> FrequentItems { get; set; }
    }
}
