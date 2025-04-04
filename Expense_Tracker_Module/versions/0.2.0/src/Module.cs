using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ExpenseTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Expense Tracker Module";

    private class Expense
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    private class ExpenseData
    {
        public List<Expense> Expenses { get; set; } = new List<Expense>();
    }

    private string GetDataFilePath(string dataFolder)
    {
        return Path.Combine(dataFolder, "expenses.json");
    }

    private ExpenseData LoadExpenses(string dataFolder)
    {
        string filePath = GetDataFilePath(dataFolder);
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<ExpenseData>(jsonData) ?? new ExpenseData();
        }
        return new ExpenseData();
    }

    private void SaveExpenses(string dataFolder, ExpenseData data)
    {
        string filePath = GetDataFilePath(dataFolder);
        string jsonData = JsonSerializer.Serialize(data);
        File.WriteAllText(filePath, jsonData);
    }

    private void DisplayMonthlyReport(ExpenseData data, DateTime month)
    {
        var monthlyExpenses = data.Expenses
            .Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) });

        Console.WriteLine("Monthly Expense Report for " + month.ToString("MMMM yyyy") + ":");
        Console.WriteLine("--------------------------------");
        foreach (var category in monthlyExpenses)
        {
            Console.WriteLine(category.Category + ": " + category.Total.ToString("C"));
        }
        Console.WriteLine("--------------------------------");
        Console.WriteLine("Total: " + monthlyExpenses.Sum(c => c.Total).ToString("C"));
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Expense Tracker Module is running...");
        Console.WriteLine("Initializing expense tracking system...");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        ExpenseData expenseData = LoadExpenses(dataFolder);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Expense");
            Console.WriteLine("2. View Monthly Report");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddExpense(expenseData);
                    SaveExpenses(dataFolder, expenseData);
                    break;
                case "2":
                    Console.Write("Enter month and year (MM/YYYY): ");
                    if (DateTime.TryParseExact(Console.ReadLine(), "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime reportMonth))
                    {
                        DisplayMonthlyReport(expenseData, reportMonth);
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please use MM/YYYY.");
                    }
                    break;
                case "3":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Saving expense data...");
        SaveExpenses(dataFolder, expenseData);
        Console.WriteLine("Expense Tracker Module completed successfully.");
        return true;
    }

    private void AddExpense(ExpenseData data)
    {
        Console.Write("Enter expense date (MM/DD/YYYY): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime expenseDate))
        {
            Console.WriteLine("Invalid date format. Using today's date.");
            expenseDate = DateTime.Today;
        }

        Console.Write("Enter category: ");
        string category = Console.ReadLine();

        Console.Write("Enter description: ");
        string description = Console.ReadLine();

        Console.Write("Enter amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Setting to 0.");
            amount = 0;
        }

        data.Expenses.Add(new Expense
        {
            Date = expenseDate,
            Category = category,
            Description = description,
            Amount = amount
        });

        Console.WriteLine("Expense added successfully.");
    }
}