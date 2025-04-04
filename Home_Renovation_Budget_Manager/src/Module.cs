using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HomeRenovationBudgetManager : IGeneratedModule
{
    public string Name { get; set; } = "Home Renovation Budget Manager";
    
    private string _expensesFilePath;
    private string _budgetFilePath;
    
    public bool Main(string dataFolder)
    {
        _expensesFilePath = Path.Combine(dataFolder, "expenses.json");
        _budgetFilePath = Path.Combine(dataFolder, "budget.json");
        
        Console.WriteLine("Home Renovation Budget Manager started.");
        Console.WriteLine("Data will be saved in: " + dataFolder);
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        Budget budget = LoadBudget();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. View Budget");
            Console.WriteLine("2. Add Expense");
            Console.WriteLine("3. View Expenses");
            Console.WriteLine("4. Update Budget");
            Console.WriteLine("5. Exit");
            
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    ViewBudget(budget);
                    break;
                case "2":
                    AddExpense(budget);
                    break;
                case "3":
                    ViewExpenses();
                    break;
                case "4":
                    UpdateBudget(ref budget);
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Saving data and exiting Home Renovation Budget Manager.");
        return true;
    }
    
    private Budget LoadBudget()
    {
        if (File.Exists(_budgetFilePath))
        {
            string json = File.ReadAllText(_budgetFilePath);
            return JsonSerializer.Deserialize<Budget>(json);
        }
        else
        {
            Console.Write("Enter your total renovation budget: ");
            decimal totalBudget;
            while (!decimal.TryParse(Console.ReadLine(), out totalBudget) || totalBudget <= 0)
            {
                Console.Write("Invalid amount. Please enter a positive number: ");
            }
            
            var newBudget = new Budget { TotalAmount = totalBudget, RemainingAmount = totalBudget };
            SaveBudget(newBudget);
            return newBudget;
        }
    }
    
    private void SaveBudget(Budget budget)
    {
        string json = JsonSerializer.Serialize(budget);
        File.WriteAllText(_budgetFilePath, json);
    }
    
    private void ViewBudget(Budget budget)
    {
        Console.WriteLine("\nCurrent Budget:");
        Console.WriteLine("Total Budget: " + budget.TotalAmount.ToString("C"));
        Console.WriteLine("Remaining Budget: " + budget.RemainingAmount.ToString("C"));
        Console.WriteLine("Spent: " + (budget.TotalAmount - budget.RemainingAmount).ToString("C"));
    }
    
    private void AddExpense(Budget budget)
    {
        Console.Write("Enter expense description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter expense amount: ");
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
        {
            Console.Write("Invalid amount. Please enter a positive number: ");
        }
        
        if (amount > budget.RemainingAmount)
        {
            Console.WriteLine("Warning: This expense exceeds your remaining budget!");
            Console.Write("Do you want to proceed? (y/n): ");
            if (Console.ReadLine().ToLower() != "y")
            {
                return;
            }
        }
        
        var expense = new Expense
        {
            Description = description,
            Amount = amount,
            Date = DateTime.Now
        };
        
        List<Expense> expenses = LoadExpenses();
        expenses.Add(expense);
        
        budget.RemainingAmount -= amount;
        SaveBudget(budget);
        
        string json = JsonSerializer.Serialize(expenses);
        File.WriteAllText(_expensesFilePath, json);
        
        Console.WriteLine("Expense added successfully.");
    }
    
    private List<Expense> LoadExpenses()
    {
        if (File.Exists(_expensesFilePath))
        {
            string json = File.ReadAllText(_expensesFilePath);
            return JsonSerializer.Deserialize<List<Expense>>(json);
        }
        return new List<Expense>();
    }
    
    private void ViewExpenses()
    {
        List<Expense> expenses = LoadExpenses();
        
        if (expenses.Count == 0)
        {
            Console.WriteLine("No expenses recorded yet.");
            return;
        }
        
        Console.WriteLine("\nExpense History:");
        foreach (var expense in expenses)
        {
            Console.WriteLine($"{expense.Date:yyyy-MM-dd}: {expense.Description} - {expense.Amount.ToString("C")}");
        }
        
        decimal totalSpent = 0;
        foreach (var expense in expenses)
        {
            totalSpent += expense.Amount;
        }
        
        Console.WriteLine("\nTotal Spent: " + totalSpent.ToString("C"));
    }
    
    private void UpdateBudget(ref Budget budget)
    {
        Console.Write("Enter new total budget amount: ");
        decimal newAmount;
        while (!decimal.TryParse(Console.ReadLine(), out newAmount) || newAmount <= 0)
        {
            Console.Write("Invalid amount. Please enter a positive number: ");
        }
        
        decimal difference = newAmount - budget.TotalAmount;
        budget.TotalAmount = newAmount;
        budget.RemainingAmount += difference;
        
        SaveBudget(budget);
        Console.WriteLine("Budget updated successfully.");
    }
}

public class Budget
{
    public decimal TotalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
}

public class Expense
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}