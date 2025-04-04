using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class ExpenseTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker";

    private string expensesFilePath;
    private string categoriesFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Expense Tracker Module is running...");

        expensesFilePath = Path.Combine(dataFolder, "expenses.json");
        categoriesFilePath = Path.Combine(dataFolder, "categories.json");

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
                    ViewMonthlyReport();
                    break;
                case "3":
                    ManageCategories();
                    break;
                case "4":
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
        if (!File.Exists(expensesFilePath))
        {
            File.WriteAllText(expensesFilePath, "[]");
        }

        if (!File.Exists(categoriesFilePath))
        {
            var defaultCategories = new List<string> { "Food", "Transport", "Utilities", "Entertainment", "Other" };
            File.WriteAllText(categoriesFilePath, JsonSerializer.Serialize(defaultCategories));
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nExpense Tracker Menu:");
        Console.WriteLine("1. Add Expense");
        Console.WriteLine("2. View Monthly Report");
        Console.WriteLine("3. Manage Categories");
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
        string description = Console.ReadLine();

        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(categoriesFilePath));
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

        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(expensesFilePath));
        expenses.Add(expense);
        File.WriteAllText(expensesFilePath, JsonSerializer.Serialize(expenses));

        Console.WriteLine("Expense added successfully.");
    }

    private void ViewMonthlyReport()
    {
        var expenses = JsonSerializer.Deserialize<List<Expense>>(File.ReadAllText(expensesFilePath));
        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(categoriesFilePath));

        Console.Write("Enter year and month (yyyy-MM): ");
        string yearMonthInput = Console.ReadLine();

        if (!DateTime.TryParse(yearMonthInput + "-01", out DateTime targetMonth))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM format.");
            return;
        }

        var monthlyExpenses = expenses.FindAll(e => e.Date.Year == targetMonth.Year && e.Date.Month == targetMonth.Month);

        Console.WriteLine("\nMonthly Report for " + targetMonth.ToString("yyyy-MM") + ":");
        Console.WriteLine("Total Expenses: " + monthlyExpenses.Sum(e => e.Amount).ToString("C"));

        Console.WriteLine("\nExpenses by Category:");
        foreach (var category in categories)
        {
            var categoryExpenses = monthlyExpenses.FindAll(e => e.Category == category);
            if (categoryExpenses.Count > 0)
            {
                Console.WriteLine(category + ": " + categoryExpenses.Sum(e => e.Amount).ToString("C") + " (" + categoryExpenses.Count + " expenses)");
            }
        }
    }

    private void ManageCategories()
    {
        var categories = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(categoriesFilePath));

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
                    File.WriteAllText(categoriesFilePath, JsonSerializer.Serialize(categories));
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
                    File.WriteAllText(categoriesFilePath, JsonSerializer.Serialize(categories));
                    Console.WriteLine("Category removed successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid category number.");
                }
                break;
            case "3":
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
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