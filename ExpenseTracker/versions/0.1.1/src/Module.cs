using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class ExpenseTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker Module";
    
    private string _expensesPath;
    private string _categoriesPath;
    private string _reportsPath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Expense Tracker Module...");
        
        _expensesPath = Path.Combine(dataFolder, "expenses.json");
        _categoriesPath = Path.Combine(dataFolder, "categories.json");
        _reportsPath = Path.Combine(dataFolder, "reports.json");
        
        var expenseRepo = new ExpenseRepository(_expensesPath);
        var categoryRepo = new CategoryRepository(_categoriesPath);
        var reportRepo = new ReportRepository(_reportsPath);
        
        var categoryService = new CategoryService(categoryRepo, _expensesPath);
        var expenseService = new ExpenseService(expenseRepo, categoryService);
        var reportService = new ReportService(expenseRepo, reportRepo);
        
        InitializeDefaultCategories(categoryService);
        
        while (ShowMainMenu(expenseService, categoryService, reportService)) { }
        
        return true;
    }
    
    private void InitializeDefaultCategories(CategoryService service)
    {
        if (!service.GetAllCategories().Any())
        {
            service.AddCategory(new Category { Name = "Food", BudgetLimit = 0 });
            service.AddCategory(new Category { Name = "Transportation", BudgetLimit = 0 });
            service.AddCategory(new Category { Name = "Housing", BudgetLimit = 0 });
        }
    }
    
    private bool ShowMainMenu(ExpenseService expenseService, CategoryService categoryService, ReportService reportService)
    {
        Console.WriteLine("\nMain Menu:");
        Console.WriteLine("1. Add Expense");
        Console.WriteLine("2. List Expenses");
        Console.WriteLine("3. Edit Expense");
        Console.WriteLine("4. Delete Expense");
        Console.WriteLine("5. Manage Categories");
        Console.WriteLine("6. Generate Report");
        Console.WriteLine("7. Exit");
        
        switch (Console.ReadLine())
        {
            case "1": AddExpense(expenseService, categoryService); return true;
            case "2": ListExpenses(expenseService); return true;
            case "3": EditExpense(expenseService, categoryService); return true;
            case "4": DeleteExpense(expenseService); return true;
            case "5": ManageCategories(categoryService); return true;
            case "6": GenerateReport(reportService, expenseService); return true;
            case "7": return false;
            default: Console.WriteLine("Invalid option"); return true;
        }
    }
    
    private void AddExpense(ExpenseService service, CategoryService categoryService)
    {
        try
        {
            Console.WriteLine("Enter amount:");
            var amount = decimal.Parse(Console.ReadLine());
            
            Console.WriteLine("Enter date (yyyy-MM-dd):");
            var date = DateTime.Parse(Console.ReadLine());
            
            Console.WriteLine("Available categories:");
            categoryService.GetAllCategories().ToList().ForEach(c => Console.WriteLine(c.Name));
            Console.WriteLine("Enter category name:");
            var categoryName = Console.ReadLine();
            
            Console.WriteLine("Enter description:");
            var description = Console.ReadLine();
            
            var expense = new Expense
            {
                Amount = amount,
                Date = date,
                CategoryId = categoryService.GetCategoryByName(categoryName).Id,
                Description = description
            };
            
            service.AddExpense(expense);
            Console.WriteLine("Expense added successfully");
            
            CheckBudgetAlert(expense, categoryService);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding expense: " + ex.Message);
        }
    }
    
    private void EditExpense(ExpenseService service, CategoryService categoryService)
    {
        try
        {
            Console.WriteLine("Enter expense ID to edit:");
            var id = Console.ReadLine();
            var expense = service.GetExpense(id);
            
            if (expense == null)
            {
                Console.WriteLine("Expense not found!");
                return;
            }
            
            Console.WriteLine("Enter new amount (current: " + expense.Amount + "):");
            var amountInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(amountInput))
                expense.Amount = decimal.Parse(amountInput);
            
            Console.WriteLine("Enter new date (yyyy-MM-dd) (current: " + expense.Date.ToString("yyyy-MM-dd") + "):");
            var dateInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(dateInput))
                expense.Date = DateTime.Parse(dateInput);
            
            Console.WriteLine("Available categories:");
            categoryService.GetAllCategories().ToList().ForEach(c => Console.WriteLine(c.Name));
            Console.WriteLine("Enter new category name (current: " + categoryService.GetCategory(expense.CategoryId).Name + "):");
            var categoryName = Console.ReadLine();
            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = categoryService.GetCategoryByName(categoryName);
                if (category != null)
                    expense.CategoryId = category.Id;
                else
                    Console.WriteLine("Category not found, keeping current category.");
            }
            
            Console.WriteLine("Enter new description (current: " + expense.Description + "):");
            var description = Console.ReadLine();
            if (!string.IsNullOrEmpty(description))
                expense.Description = description;
            
            service.UpdateExpense(expense);
            Console.WriteLine("Expense updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error editing expense: " + ex.Message);
        }
    }
    
    private void DeleteExpense(ExpenseService service)
    {
        try
        {
            Console.WriteLine("Enter expense ID to delete:");
            var id = Console.ReadLine();
            var expense = service.GetExpense(id);
            
            if (expense == null)
            {
                Console.WriteLine("Expense not found!");
                return;
            }
            
            service.DeleteExpense(id);
            Console.WriteLine("Expense deleted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting expense: " + ex.Message);
        }
    }
    
    private void CheckBudgetAlert(Expense expense, CategoryService categoryService)
    {
        var category = categoryService.GetCategory(expense.CategoryId);
        if (category.BudgetLimit <= 0) return;
        
        var monthlyTotal = categoryService.GetMonthlyTotal(expense.CategoryId, DateTime.Now.Month);
        if (monthlyTotal >= category.BudgetLimit)
        {
            Console.WriteLine("ALERT: Budget limit exceeded for category " + category.Name);
        }
        else if (monthlyTotal >= category.BudgetLimit * 0.9m)
        {
            Console.WriteLine("WARNING: Approaching budget limit for category " + category.Name);
        }
    }
    
    private void ListExpenses(ExpenseService service)
    {
        Console.WriteLine("\nExpenses:");
        service.GetAllExpenses().ForEach(e =>
            Console.WriteLine($"{e.Id} - {e.Date:yyyy-MM-dd} - {e.Amount:C} - {e.Description}"));
    }
    
    private void ManageCategories(CategoryService service)
    {
        Console.WriteLine("\nCategory Management:");
        Console.WriteLine("1. Add Category");
        Console.WriteLine("2. Set Budget Limit");
        Console.WriteLine("3. List Categories");
        
        switch (Console.ReadLine())
        {
            case "1":
                Console.WriteLine("Enter category name:");
                var name = Console.ReadLine();
                service.AddCategory(new Category { Name = name });
                break;
            case "2":
                Console.WriteLine("Enter category name:");
                var categoryName = Console.ReadLine();
                Console.WriteLine("Enter budget limit:");
                var limit = decimal.Parse(Console.ReadLine());
                var category = service.GetCategoryByName(categoryName);
                category.BudgetLimit = limit;
                service.UpdateCategory(category);
                break;
            case "3":
                service.GetAllCategories().ForEach(c => 
                    Console.WriteLine($"{c.Name} - Budget: {c.BudgetLimit:C}"));
                break;
        }
    }
    
    private void GenerateReport(ReportService reportService, ExpenseService expenseService)
    {
        Console.WriteLine("Enter start date (yyyy-MM-dd):");
        var start = DateTime.Parse(Console.ReadLine());
        Console.WriteLine("Enter end date (yyyy-MM-dd):");
        var end = DateTime.Parse(Console.ReadLine());
        
        var report = reportService.GenerateReport(start, end);
        
        Console.WriteLine("\nMonthly Report:");
        Console.WriteLine("Total Expenses: " + report.TotalExpenses.ToString("C"));
        Console.WriteLine("By Category:");
        foreach (var item in report.ExpensesByCategory)
        {
            Console.WriteLine($"{item.CategoryName}: {item.Total.ToString("C")}");
        }
    }
}

public class Expense
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string CategoryId { get; set; }
    public string Description { get; set; }
}

public class Category
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public decimal BudgetLimit { get; set; }
}

public class Report
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalExpenses { get; set; }
    public List<CategoryTotal> ExpensesByCategory { get; set; } = new List<CategoryTotal>();
}

public class CategoryTotal
{
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public decimal Total { get; set; }
}

public class ExpenseRepository
{
    private readonly string _filePath;
    
    public ExpenseRepository(string filePath) => _filePath = filePath;
    
    public List<Expense> LoadExpenses() => File.Exists(_filePath) 
        ? JsonConvert.DeserializeObject<List<Expense>>(File.ReadAllText(_filePath)) 
        : new List<Expense>();
    
    public void SaveExpenses(List<Expense> expenses) => File.WriteAllText(_filePath, JsonConvert.SerializeObject(expenses));
}

public class CategoryRepository
{
    private readonly string _filePath;
    
    public CategoryRepository(string filePath) => _filePath = filePath;
    
    public List<Category> LoadCategories() => File.Exists(_filePath) 
        ? JsonConvert.DeserializeObject<List<Category>>(File.ReadAllText(_filePath)) 
        : new List<Category>();
    
    public void SaveCategories(List<Category> categories) => File.WriteAllText(_filePath, JsonConvert.SerializeObject(categories));
}

public class ReportRepository
{
    private readonly string _filePath;
    
    public ReportRepository(string filePath) => _filePath = filePath;
    
    public List<Report> LoadReports() => File.Exists(_filePath) 
        ? JsonConvert.DeserializeObject<List<Report>>(File.ReadAllText(_filePath)) 
        : new List<Report>();
    
    public void SaveReports(List<Report> reports) => File.WriteAllText(_filePath, JsonConvert.SerializeObject(reports));
}

public class ExpenseService
{
    private readonly ExpenseRepository _repository;
    private readonly CategoryService _categoryService;
    
    public ExpenseService(ExpenseRepository repository, CategoryService categoryService)
    {
        _repository = repository;
        _categoryService = categoryService;
    }
    
    public void AddExpense(Expense expense)
    {
        var expenses = _repository.LoadExpenses();
        expenses.Add(expense);
        _repository.SaveExpenses(expenses);
    }
    
    public List<Expense> GetAllExpenses() => _repository.LoadExpenses();
    
    public Expense GetExpense(string id) => _repository.LoadExpenses().FirstOrDefault(e => e.Id == id);
    
    public void UpdateExpense(Expense expense)
    {
        var expenses = _repository.LoadExpenses();
        var index = expenses.FindIndex(e => e.Id == expense.Id);
        if (index >= 0)
        {
            expenses[index] = expense;
            _repository.SaveExpenses(expenses);
        }
    }
    
    public void DeleteExpense(string expenseId)
    {
        var expenses = _repository.LoadExpenses();
        var expense = expenses.FirstOrDefault(e => e.Id == expenseId);
        if (expense != null)
        {
            expenses.Remove(expense);
            _repository.SaveExpenses(expenses);
        }
    }
}

public class CategoryService
{
    private readonly CategoryRepository _repository;
    private readonly string _expensesPath;
    
    public CategoryService(CategoryRepository repository, string expensesPath)
    {
        _repository = repository;
        _expensesPath = expensesPath;
    }
    
    public void AddCategory(Category category)
    {
        var categories = _repository.LoadCategories();
        categories.Add(category);
        _repository.SaveCategories(categories);
    }
    
    public List<Category> GetAllCategories() => _repository.LoadCategories();
    
    public Category GetCategory(string id) => _repository.LoadCategories().FirstOrDefault(c => c.Id == id);
    
    public Category GetCategoryByName(string name) => _repository.LoadCategories().FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    
    public void UpdateCategory(Category category)
    {
        var categories = _repository.LoadCategories();
        var index = categories.FindIndex(c => c.Id == category.Id);
        if (index >= 0) categories[index] = category;
        _repository.SaveCategories(categories);
    }
    
    public decimal GetMonthlyTotal(string categoryId, int month)
    {
        var expenses = new ExpenseRepository(_expensesPath).LoadExpenses();
        return expenses
            .Where(e => e.CategoryId == categoryId && e.Date.Month == month)
            .Sum(e => e.Amount);
    }
}

public class ReportService
{
    private readonly ExpenseRepository _expenseRepo;
    private readonly ReportRepository _reportRepo;
    
    public ReportService(ExpenseRepository expenseRepo, ReportRepository reportRepo)
    {
        _expenseRepo = expenseRepo;
        _reportRepo = reportRepo;
    }
    
    public Report GenerateReport(DateTime start, DateTime end)
    {
        var expenses = _expenseRepo.LoadExpenses()
            .Where(e => e.Date >= start && e.Date <= end)
            .ToList();
        
        var report = new Report
        {
            StartDate = start,
            EndDate = end,
            TotalExpenses = expenses.Sum(e => e.Amount),
            ExpensesByCategory = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new CategoryTotal
                {
                    CategoryId = g.Key,
                    Total = g.Sum(e => e.Amount)
                }).ToList()
        };
        
        var reports = _reportRepo.LoadReports();
        reports.Add(report);
        _reportRepo.SaveReports(reports);
        
        return report;
    }
}