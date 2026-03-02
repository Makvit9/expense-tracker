using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class ExpenseTrackerContext : DbContext
    {
        public ExpenseTrackerContext(DbContextOptions<ExpenseTrackerContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Expense> Expenses { get; set; }
        public DbSet<Models.Category> Categories { get; set; }
        public DbSet<Models.MonthlySummary> MonthlySummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial categories
            modelBuilder.Entity<Models.Category>().HasData(
                new Models.Category { Id = 1, Name = "Bills", ColorCode = "#FF6B6B", DisplayOrder = 1 },
                new Models.Category { Id = 2, Name = "Personal - Smoking", ColorCode = "#4ECDC4", DisplayOrder = 2 },
                new Models.Category { Id = 3, Name = "Personal - Food", ColorCode = "#45B7D1", DisplayOrder = 3 },
                new Models.Category { Id = 4, Name = "Personal - Drinks", ColorCode = "#96CEB4", DisplayOrder = 4 },
                new Models.Category { Id = 5, Name = "Transportation", ColorCode = "#FFEAA7", DisplayOrder = 5 }
            );

            // Create unique index for Month/Year in MonthlySummary
            modelBuilder.Entity<Models.MonthlySummary>()
                .HasIndex(m => new { m.Month, m.Year })
                .IsUnique();
        }
    }
}
