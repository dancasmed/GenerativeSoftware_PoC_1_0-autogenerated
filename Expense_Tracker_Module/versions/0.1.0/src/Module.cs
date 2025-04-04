using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ExpenseTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker Module";
    
    private List<Expense> _expenses;
    private string _dataFilePath;
    
    public ExpenseTrackerModule()
    {
        _expenses = new List<Expense>();
    }
    
    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Expense Tracker Module is running...");
            
            _dataFilePath = Path.Combine(dataFolder, "expenses.json");
            
            if (File.Exists(_dataFilePath))
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _expenses = JsonSerializer.Deserialize<List<Expense>>(jsonData) ?? new List<Expense>();
            }
            
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nExpense Tracker Menu:");
                Console.WriteLine("1. Add Expense");
                Console.WriteLine("2. View Monthly Report");
                Console.WriteLine("3. Exit Module");
                Console.Write("Select an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddExpense();
                        break;
                    case "2":
                        GenerateMonthlyReport();
                        break;
                    case "3":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveData();
            Console.WriteLine("Expense data saved. Module exiting...");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void AddExpense()
    {
        Console.Write("Enter expense amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return;
        }
        
        Console.Write("Enter expense category: ");
        string category = Console.ReadLine();
        
        Console.Write("Enter expense description: ");
        string description = Console.ReadLine();
        
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
            Console.WriteLine("No expenses recorded yet.");
            return;
        }
        
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        
        var monthlyExpenses = _expenses.FindAll(e => e.Date.Month == currentMonth && e.Date.Year == currentYear);
        
        if (monthlyExpenses.Count == 0)
        {
            Console.WriteLine("No expenses recorded for this month.");
            return;
        }
        
        Console.WriteLine("\nMonthly Expense Report:");
        Console.WriteLine("----------------------");
        
        var categories = new Dictionary<string, decimal>();
        decimal total = 0;
        
        foreach (var expense in monthlyExpenses)
        {
            total += expense.Amount;
            
            if (categories.ContainsKey(expense.Category))
            {
                categories[expense.Category] += expense.Amount;
            }
            else
            {
                categories[expense.Category] = expense.Amount;
            }
        }
        
        Console.WriteLine("Expenses by Category:");
        foreach (var category in categories)
        {
            Console.WriteLine(category.Key + ": " + category.Value.ToString("C"));
        }
        
        Console.WriteLine("\nTotal Expenses: " + total.ToString("C"));
    }
    
    private void SaveData()
    {
        string jsonData = JsonSerializer.Serialize(_expenses);
        File.WriteAllText(_dataFilePath, jsonData);
    }
}

public class Expense
{
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}