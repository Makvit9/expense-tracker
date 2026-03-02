using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class MonthlySummary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Month")]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Salary")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal Salary { get; set; }

        [Display(Name = "Additional Income")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99)]
        public decimal AdditionalIncome { get; set; } = 0;

        [Display(Name = "Notes")]
        [StringLength(500)]
        public string Notes { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // Computed properties (not stored in DB)
        [NotMapped]
        [Display(Name = "Total Income")]
        public decimal TotalIncome => Salary + AdditionalIncome;

        [NotMapped]
        [Display(Name = "Total Spending")]
        public decimal TotalSpending { get; set; }

        [NotMapped]
        [Display(Name = "Balance")]
        public decimal Balance => TotalIncome - TotalSpending;
    }
}
