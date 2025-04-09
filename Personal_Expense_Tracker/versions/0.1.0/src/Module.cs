using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ExpenseTracker : IGeneratedModule
{
    public string Name { get; set; } = "Personal Expense Tracker";

    private string _expensesFilePath;
    private List<Expense> _expenses;

    public ExpenseTracker()
    {
        _expenses = new List<Expense>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Personal Expense Tracker...");
        _expensesFilePath = Path.Combine(dataFolder, "expenses.json");

        LoadExpenses();

        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddExpense();
                    break;
                case "2":
                    ViewExpenses();
                    break;
                case "3":
                    ViewExpensesByCategory();
                    break;
                case "4":
                    SaveExpenses();
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Expense tracker session ended.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPersonal Expense Tracker");
        Console.WriteLine("1. Add Expense");
        Console.WriteLine("2. View All Expenses");
        Console.WriteLine("3. View Expenses by Category");
        Console.WriteLine("4. Exit and Save");
        Console.Write("Select an option: ");
    }

    private void AddExpense()
    {
        Console.Write("Enter expense amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return;
        }

        Console.Write("Enter expense category (Food, Transportation, Entertainment): ");
        string category = Console.ReadLine().Trim();

        Console.Write("Enter expense description: ");
        string description = Console.ReadLine().Trim();

        _expenses.Add(new Expense
        {
            Amount = amount,
            Category = category,
            Description = description,
            Date = DateTime.Now
        });

        Console.WriteLine("Expense added successfully.");
    }

    private void ViewExpenses()
    {
        if (_expenses.Count == 0)
        {
            Console.WriteLine("No expenses recorded yet.");
            return;
        }

        Console.WriteLine("\nAll Expenses:");
        foreach (var expense in _expenses)
        {
            Console.WriteLine($"{expense.Date}: {expense.Category} - {expense.Description} - ${expense.Amount}");
        }
    }

    private void ViewExpensesByCategory()
    {
        Console.Write("Enter category to filter (Food, Transportation, Entertainment): ");
        string category = Console.ReadLine().Trim();

        var filteredExpenses = _expenses.FindAll(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (filteredExpenses.Count == 0)
        {
            Console.WriteLine("No expenses found for the specified category.");
            return;
        }

        Console.WriteLine($"\nExpenses for {category}:");
        foreach (var expense in filteredExpenses)
        {
            Console.WriteLine($"{expense.Date}: {expense.Description} - ${expense.Amount}");
        }
    }

    private void LoadExpenses()
    {
        if (File.Exists(_expensesFilePath))
        {
            try
            {
                string json = File.ReadAllText(_expensesFilePath);
                _expenses = JsonSerializer.Deserialize<List<Expense>>(json);
                Console.WriteLine("Previous expenses loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading expenses: " + ex.Message);
            }
        }
    }

    private void SaveExpenses()
    {
        try
        {
            string json = JsonSerializer.Serialize(_expenses);
            File.WriteAllText(_expensesFilePath, json);
            Console.WriteLine("Expenses saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving expenses: " + ex.Message);
        }
    }
}

public class Expense
{
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}