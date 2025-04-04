using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ExpenseTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker Module";

    private string _expensesFilePath;
    private string _categoriesFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Expense Tracker Module is running...");
        
        _expensesFilePath = Path.Combine(dataFolder, "expenses.json");
        _categoriesFilePath = Path.Combine(dataFolder, "categories.json");

        InitializeFiles();

        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddExpense();
                    break;
                case "2":
                    AddCategory();
                    break;
                case "3":
                    GenerateMonthlyReport();
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        return true;
    }

    private void InitializeFiles()
    {
        if (!Directory.Exists(Path.GetDirectoryName(_expensesFilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(_expensesFilePath));

        if (!File.Exists(_expensesFilePath))
            File.WriteAllText(_expensesFilePath, "[]");

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
        Console.WriteLine("3. Generate Monthly Report");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private void AddExpense()
    {
        Console.Write("Enter expense amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount format.");
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

    private void GenerateMonthlyReport()
    {
        Console.Write("Enter year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year format.");
            return;
        }

        Console.Write("Enter month (1-12): ");
        if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
        {
            Console.WriteLine("Invalid month format.");
            return;
        }

        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(_expensesFilePath));
        var monthlyExpenses = expenses.FindAll(e => e.Date.Year == year && e.Date.Month == month);

        if (monthlyExpenses.Count == 0)
        {
            Console.WriteLine("No expenses found for the selected month.");
            return;
        }

        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_categoriesFilePath));
        var report = new Dictionary<string, decimal>();

        foreach (var category in categories)
        {
            report[category] = 0;
        }

        foreach (var expense in monthlyExpenses)
        {
            report[expense.Category] += expense.Amount;
        }

        Console.WriteLine("\nMonthly Expense Report:");
        Console.WriteLine($"{new DateTime(year, month, 1):MMMM yyyy}");
        Console.WriteLine("----------------------------");

        decimal total = 0;
        foreach (var entry in report)
        {
            if (entry.Value > 0)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value:C}");
                total += entry.Value;
            }
        }

        Console.WriteLine("----------------------------");
        Console.WriteLine($"Total: {total:C}");
    }

    private class Expense
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }
}