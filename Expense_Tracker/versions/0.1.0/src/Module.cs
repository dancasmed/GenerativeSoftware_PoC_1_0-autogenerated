using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ExpenseTracker : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker";

    private string _expensesFilePath;
    private string _categoriesFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Expense Tracker module is running...");
        
        _expensesFilePath = Path.Combine(dataFolder, "expenses.json");
        _categoriesFilePath = Path.Combine(dataFolder, "categories.json");

        InitializeFiles();

        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddExpense();
                    break;
                case "2":
                    AddCategory();
                    break;
                case "3":
                    ViewMonthlyReport();
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        return true;
    }

    private void InitializeFiles()
    {
        if (!File.Exists(_expensesFilePath))
        {
            File.WriteAllText(_expensesFilePath, "[]");
        }

        if (!File.Exists(_categoriesFilePath))
        {
            var defaultCategories = new List<string> { "Food", "Transport", "Utilities", "Entertainment" };
            File.WriteAllText(_categoriesFilePath, JsonSerializer.Serialize(defaultCategories));
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nExpense Tracker Menu:");
        Console.WriteLine("1. Add Expense");
        Console.WriteLine("2. Add Category");
        Console.WriteLine("3. View Monthly Report");
        Console.WriteLine("4. Exit");
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

        Console.Write("Enter expense description: ");
        var description = Console.ReadLine();

        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_categoriesFilePath));
        Console.WriteLine("Available categories:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + categories[i]);
        }

        Console.Write("Select category (number): ");
        if (!int.TryParse(Console.ReadLine(), out int categoryIndex) || categoryIndex < 1 || categoryIndex > categories.Count)
        {
            Console.WriteLine("Invalid category selection.");
            return;
        }

        var expense = new Expense
        {
            Amount = amount,
            Description = description,
            Category = categories[categoryIndex - 1],
            Date = DateTime.Now
        };

        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(_expensesFilePath));
        expenses.Add(expense);
        File.WriteAllText(_expensesFilePath, JsonSerializer.Serialize(expenses));

        Console.WriteLine("Expense added successfully.");
    }

    private void AddCategory()
    {
        Console.Write("Enter new category name: ");
        var categoryName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            Console.WriteLine("Category name cannot be empty.");
            return;
        }

        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_categoriesFilePath));
        if (categories.Contains(categoryName))
        {
            Console.WriteLine("Category already exists.");
            return;
        }

        categories.Add(categoryName);
        File.WriteAllText(_categoriesFilePath, JsonSerializer.Serialize(categories));

        Console.WriteLine("Category added successfully.");
    }

    private void ViewMonthlyReport()
    {
        Console.Write("Enter month (1-12): ");
        if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
        {
            Console.WriteLine("Invalid month.");
            return;
        }

        Console.Write("Enter year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year.");
            return;
        }

        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(_expensesFilePath));
        var monthlyExpenses = expenses.FindAll(e => e.Date.Month == month && e.Date.Year == year);

        if (monthlyExpenses.Count == 0)
        {
            Console.WriteLine("No expenses found for the selected month.");
            return;
        }

        var report = new Dictionary<string, decimal>();
        decimal total = 0;

        foreach (var expense in monthlyExpenses)
        {
            total += expense.Amount;
            if (report.ContainsKey(expense.Category))
            {
                report[expense.Category] += expense.Amount;
            }
            else
            {
                report[expense.Category] = expense.Amount;
            }
        }

        Console.WriteLine("\nMonthly Expense Report:");
        Console.WriteLine("----------------------");
        foreach (var entry in report)
        {
            Console.WriteLine(entry.Key + ": " + entry.Value.ToString("C"));
        }
        Console.WriteLine("----------------------");
        Console.WriteLine("Total: " + total.ToString("C"));
    }

    private class Expense
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }
}