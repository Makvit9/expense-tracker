using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Item Name")]
        [StringLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        [Required]
        [Display(Name = "Amount")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Display(Name = "Week Number")]
        [Range(1, 5)]
        public int WeekNumber { get; set; }

        [Display(Name = "Month")]
        [Range(1, 12)]
        public int Month { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
