# Expense Tracker Application

## Overview
This ASP.NET MVC application with Razor views provides a customizable experience that replaces typical excel spreadsheets

## Features

### Core Functionality
- **Monthly Expense Tracking**: Track expenses by week, date, item, category, and amount
- **Dynamic Categories**: Easily add, edit, and manage expense categories
- **Monthly Summary**: Set salary (including overtime), view total spending, and see what's left
- **Category Breakdown**: Automatic calculation of spending by category
- **Weekly Analysis**: Track spending patterns by week within each month

### Enhanced Features
- **Frequent Items Autocomplete**: Quickly add commonly purchased items (like Parliament cigarettes)
- **Auto Week Calculation**: Automatically determines week number based on date
- **Dashboard Analytics**: Visual charts and trends for your spending patterns
- **Yearly Comparison**: Compare spending across months and years
- **Color-Coded Categories**: Visual distinction between expense types

## Models

### Expense
Primary model for tracking individual expenses:
- Date, ItemName, Amount
- Category (foreign key relationship)
- Auto-calculated: WeekNumber, Month, Year
- Optional: Notes

### Category
Manages expense categories with evolution in mind:
- Name, Description
- ColorCode (for UI visualization)
- DisplayOrder (customizable sorting)
- IsActive (soft delete for categories with historical data)

### MonthlySummary
Tracks income and provides monthly overview:
- Salary, AdditionalIncome (overtime, bonuses)
- Computed: TotalIncome, TotalSpending, Balance
- Unique constraint on Month/Year combination

### ViewModels
**MonthlyDashboardViewModel**: Comprehensive monthly data including:
- Summary information
- List of expenses
- Category and weekly breakdowns
- Frequent items list

## Controllers

### ExpensesController
Handles all expense CRUD operations:
- `Index`: View expenses for a specific month/year
- `Create`: Add new expenses with autocomplete for frequent items
- `Edit/Delete`: Modify or remove expenses
- Helper methods for week calculation and frequent items

### DashboardController
Provides analytics and overview:
- `Index`: Main dashboard with monthly summary
- `MonthlyRecap`: Detailed monthly breakdown
- `EditSalary`: Set monthly income
- API endpoints for charts (GetCategoryChartData, GetWeeklyTrendData)
- `YearlyComparison`: Annual overview

### CategoriesController
Manages expense categories:
- Full CRUD operations
- `Reorder`: Change category display order
- Soft delete for categories with existing expenses
- Auto-generate color codes for new categories

## Database Context

**ExpenseTrackerContext**: Entity Framework Core DbContext with:
- Seed data for initial categories
- Unique constraint on MonthlySummary (Month, Year)
- Relationship configurations

## Setup Instructions

### Prerequisites
- .NET 10.0
- SQL Server or SQL Server Express
- Visual Studio 2022 or VS Code

### Installation Steps



1. **Install Required Packages**
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
   ```

3. **Register DbContext in Program.cs or Startup.cs**
   ```csharp
   builder.Services.AddDbContext<ExpenseTrackerContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

4. **Create and Apply Migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

## Current Workflows

1. **Monthly Setup**:
   - Navigate to Dashboard
   - Set your salary for the month (include overtime if applicable)

2. **Adding Expenses**:
   - Click "Add Expense"
   - Select date (week number auto-calculated)
   - Type item name (autocomplete suggests frequent items)
   - Select category
   - Enter amount
   - Save

3. **Monthly Review**:
   - View Dashboard for complete overview
   - See total spending vs. income
   - Review category breakdown
   - Check weekly spending patterns
   - Identify frequent purchases

4. **Category Management**:
   - Add new categories as your spending evolves
   - Reorder categories for better organization
   - Assign colors for visual clarity




