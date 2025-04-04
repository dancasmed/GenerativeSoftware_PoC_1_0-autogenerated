using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class ExpenseTracker : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker";
    
    private string _expensesFilePath;
    private List<Expense> _expenses;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Expense Tracker module is running.");
        
        _expensesFilePath = Path.Combine(dataFolder, "expenses.json");
        
        try
        {
            LoadExpenses();
            
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nExpense Tracker Menu:");
                Console.WriteLine("1. Add Expense");
                Console.WriteLine("2. View Monthly Report");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");
                
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddExpense();
                        break;
                    case "2":
                        GenerateMonthlyReport();
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveExpenses();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LoadExpenses()
    {
        if (File.Exists(_expensesFilePath))
        {
            var json = File.ReadAllText(_expensesFilePath);
            _expenses = JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
        }
        else
        {
            _expenses = new List<Expense>();
        }
    }
    
    private void SaveExpenses()
    {
        var json = JsonSerializer.Serialize(_expenses);
        File.WriteAllText(_expensesFilePath, json);
    }
    
    private void AddExpense()
    {
        Console.Write("Enter expense amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }
        
        Console.Write("Enter expense category: ");
        var category = Console.ReadLine();
        
        Console.Write("Enter expense description: ");
        var description = Console.ReadLine();
        
        var expense = new Expense
        {
            Amount = amount,
            Category = category,
            Description = description,
            Date = DateTime.Now
        };
        
        _expenses.Add(expense);
        Console.WriteLine("Expense added successfully.");
    }
    
    private void GenerateMonthlyReport()
    {
        if (_expenses.Count == 0)
        {
            Console.WriteLine("No expenses to report.");
            return;
        }
        
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        
        var monthlyExpenses = _expenses
            .Where(e => e.Date.Month == currentMonth && e.Date.Year == currentYear)
            .GroupBy(e => e.Category)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(e => e.Amount),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToList();
        
        Console.WriteLine("\nMonthly Expense Report:");
        Console.WriteLine($"{currentMonth}/{currentYear}");
        Console.WriteLine("----------------------------");
        
        foreach (var category in monthlyExpenses)
        {
            Console.WriteLine($"Category: {category.Category}");
            Console.WriteLine($"Total: {category.Total:C}");
            Console.WriteLine($"Number of expenses: {category.Count}");
            Console.WriteLine("----------------------------");
        }
        
        var totalMonthlyExpenses = monthlyExpenses.Sum(x => x.Total);
        Console.WriteLine($"Total Monthly Expenses: {totalMonthlyExpenses:C}");
    }
}

public class Expense
{
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}