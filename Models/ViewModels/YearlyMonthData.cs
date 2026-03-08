using System;

namespace ExpenseTracker.Models.ViewModels
{
    public class YearlyMonthData
    {
        public string Month { get; set; } = string.Empty;
        public int MonthNumber { get; set; }
        public decimal TotalSpending { get; set; }
        public decimal Income { get; set; }
        public decimal Balance { get; set; }
    }
}