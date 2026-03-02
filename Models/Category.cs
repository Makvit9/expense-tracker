using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Color Code")]
        [StringLength(7)]
        public string ColorCode { get; set; } // For UI visualization (e.g., #FF5733)

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

    }
}
