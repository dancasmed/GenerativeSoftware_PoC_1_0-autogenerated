using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class HomeRenovationBudgetManager : IGeneratedModule
{
    public string Name { get; set; } = "Home Renovation Budget Manager";
    
    private string budgetFilePath;
    
    public HomeRenovationBudgetManager()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Renovation Budget Manager...");
        
        budgetFilePath = Path.Combine(dataFolder, "renovation_budget.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        var budget = LoadBudget();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nHome Renovation Budget Manager");
            Console.WriteLine("1. View Current Budget");
            Console.WriteLine("2. Add Expense");
            Console.WriteLine("3. Update Budget");
            Console.WriteLine("4. Save and Exit");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    DisplayBudget(budget);
                    break;
                case "2":
                    budget = AddExpense(budget);
                    break;
                case "3":
                    budget = UpdateBudget(budget);
                    break;
                case "4":
                    SaveBudget(budget);
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Budget saved successfully. Exiting...");
        return true;
    }
    
    private RenovationBudget LoadBudget()
    {
        if (File.Exists(budgetFilePath))
        {
            try
            {
                var json = File.ReadAllText(budgetFilePath);
                return JsonSerializer.Deserialize<RenovationBudget>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading budget: " + ex.Message);
            }
        }
        
        return new RenovationBudget
        {
            TotalBudget = 0,
            Expenses = new List<Expense>(),
            RemainingBudget = 0
        };
    }
    
    private void SaveBudget(RenovationBudget budget)
    {
        try
        {
            budget.RemainingBudget = budget.TotalBudget - budget.Expenses.Sum(e => e.Amount);
            var json = JsonSerializer.Serialize(budget, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(budgetFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving budget: " + ex.Message);
        }
    }
    
    private void DisplayBudget(RenovationBudget budget)
    {
        Console.WriteLine("\nCurrent Budget Details:");
        Console.WriteLine("Total Budget: " + budget.TotalBudget.ToString("C"));
        Console.WriteLine("Remaining Budget: " + budget.RemainingBudget.ToString("C"));
        Console.WriteLine("\nExpenses:");
        
        if (budget.Expenses.Count == 0)
        {
            Console.WriteLine("No expenses recorded.");
        }
        else
        {
            foreach (var expense in budget.Expenses)
            {
                Console.WriteLine($"{expense.Category}: {expense.Description} - {expense.Amount.ToString("C")} on {expense.Date.ToShortDateString()}");
            }
        }
    }
    
    private RenovationBudget AddExpense(RenovationBudget budget)
    {
        Console.WriteLine("\nAdd New Expense");
        
        Console.Write("Category: ");
        var category = Console.ReadLine();
        
        Console.Write("Description: ");
        var description = Console.ReadLine();
        
        Console.Write("Amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Expense not added.");
            return budget;
        }
        
        var expense = new Expense
        {
            Category = category,
            Description = description,
            Amount = amount,
            Date = DateTime.Now
        };
        
        budget.Expenses.Add(expense);
        budget.RemainingBudget -= amount;
        
        Console.WriteLine("Expense added successfully.");
        return budget;
    }
    
    private RenovationBudget UpdateBudget(RenovationBudget budget)
    {
        Console.WriteLine("\nUpdate Budget");
        
        Console.Write("New Total Budget: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal newTotal))
        {
            Console.WriteLine("Invalid amount. Budget not updated.");
            return budget;
        }
        
        budget.TotalBudget = newTotal;
        budget.RemainingBudget = newTotal - budget.Expenses.Sum(e => e.Amount);
        
        Console.WriteLine("Budget updated successfully.");
        return budget;
    }
}

public class RenovationBudget
{
    public decimal TotalBudget { get; set; }
    public decimal RemainingBudget { get; set; }
    public List<Expense> Expenses { get; set; }
}

public class Expense
{
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}