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
        Console.WriteLine("Expense Tracker Module is running.");

        _expensesFilePath = Path.Combine(dataFolder, "expenses.json");
        _categoriesFilePath = Path.Combine(dataFolder, "categories.json");

        InitializeFiles();

        bool running = true;
        while (running)
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
                    GenerateMonthlyReport();
                    break;
                case "4":
                    ManageCategories();
                    break;
                case "5":
                    running = false;
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
            var defaultCategories = new List<string> { "Food", "Transport", "Utilities", "Entertainment", "Other" };
            File.WriteAllText(_categoriesFilePath, JsonSerializer.Serialize(defaultCategories));
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nExpense Tracker Menu:");
        Console.WriteLine("1. Add Expense");
        Console.WriteLine("2. View Expenses");
        Console.WriteLine("3. Generate Monthly Report");
        Console.WriteLine("4. Manage Categories");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddExpense()
    {
        Console.Write("Enter amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        Console.Write("Enter description: ");
        string description = Console.ReadLine();

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

        string category = categories[categoryIndex - 1];

        var expense = new Expense
        {
            Amount = amount,
            Description = description,
            Category = category,
            Date = DateTime.Now
        };

        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(_expensesFilePath));
        expenses.Add(expense);
        File.WriteAllText(_expensesFilePath, JsonSerializer.Serialize(expenses));

        Console.WriteLine("Expense added successfully.");
    }

    private void ViewExpenses()
    {
        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(_expensesFilePath));

        if (expenses.Count == 0)
        {
            Console.WriteLine("No expenses recorded yet.");
            return;
        }

        Console.WriteLine("\nAll Expenses:");
        foreach (var expense in expenses)
        {
            Console.WriteLine($"{expense.Date:yyyy-MM-dd} - {expense.Category}: {expense.Amount:C} - {expense.Description}");
        }
    }

    private void GenerateMonthlyReport()
    {
        Console.Write("Enter year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year.");
            return;
        }

        Console.Write("Enter month (1-12): ");
        if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
        {
            Console.WriteLine("Invalid month.");
            return;
        }

        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(_expensesFilePath));
        var monthlyExpenses = expenses.FindAll(e => e.Date.Year == year && e.Date.Month == month);

        if (monthlyExpenses.Count == 0)
        {
            Console.WriteLine("No expenses recorded for this month.");
            return;
        }

        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_categoriesFilePath));
        var report = new Dictionary<string, decimal>();

        foreach (var category in categories)
        {
            report[category] = 0;
        }

        decimal total = 0;
        foreach (var expense in monthlyExpenses)
        {
            report[expense.Category] += expense.Amount;
            total += expense.Amount;
        }

        Console.WriteLine("\nMonthly Expense Report:");
        Console.WriteLine($"Month: {month}/{year}");
        Console.WriteLine("----------------------------");

        foreach (var entry in report)
        {
            if (entry.Value > 0)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value:C}");
            }
        }

        Console.WriteLine("----------------------------");
        Console.WriteLine($"Total: {total:C}");
    }

    private void ManageCategories()
    {
        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_categoriesFilePath));

        bool managing = true;
        while (managing)
        {
            Console.WriteLine("\nCurrent Categories:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine(i + 1 + ". " + categories[i]);
            }

            Console.WriteLine("\n1. Add Category");
            Console.WriteLine("2. Remove Category");
            Console.WriteLine("3. Back to Main Menu");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.Write("Enter new category name: ");
                    string newCategory = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newCategory) && !categories.Contains(newCategory))
                    {
                        categories.Add(newCategory);
                        File.WriteAllText(_categoriesFilePath, JsonSerializer.Serialize(categories));
                        Console.WriteLine("Category added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid category name or category already exists.");
                    }
                    break;
                case "2":
                    Console.Write("Enter category number to remove: ");
                    if (int.TryParse(Console.ReadLine(), out int removeIndex) && removeIndex > 0 && removeIndex <= categories.Count)
                    {
                        categories.RemoveAt(removeIndex - 1);
                        File.WriteAllText(_categoriesFilePath, JsonSerializer.Serialize(categories));
                        Console.WriteLine("Category removed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid category number.");
                    }
                    break;
                case "3":
                    managing = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}

public class Expense
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public DateTime Date { get; set; }
}