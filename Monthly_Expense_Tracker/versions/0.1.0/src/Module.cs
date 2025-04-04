using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class ExpenseTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Monthly Expense Tracker";

    private class Expense
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    private class ExpenseReport
    {
        public Dictionary<string, decimal> CategoryTotals { get; set; } = new Dictionary<string, decimal>();
        public decimal TotalExpenses { get; set; }
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Monthly Expense Tracker...");

        try
        {
            string expensesFilePath = Path.Combine(dataFolder, "expenses.json");
            string reportFilePath = Path.Combine(dataFolder, "expense_report.json");

            List<Expense> expenses = LoadExpenses(expensesFilePath);

            Console.WriteLine("Enter new expenses (type 'done' to finish):");
            while (true)
            {
                Console.Write("Category: ");
                string category = Console.ReadLine();
                if (category.ToLower() == "done") break;

                Console.Write("Amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.WriteLine("Invalid amount. Please try again.");
                    continue;
                }

                Console.Write("Description (optional): ");
                string description = Console.ReadLine();

                expenses.Add(new Expense
                {
                    Category = category,
                    Amount = amount,
                    Date = DateTime.Now,
                    Description = description ?? string.Empty
                });
            }

            SaveExpenses(expenses, expensesFilePath);

            ExpenseReport report = GenerateReport(expenses);
            SaveReport(report, reportFilePath);

            DisplayReport(report);

            Console.WriteLine("Expense tracking completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private List<Expense> LoadExpenses(string filePath)
    {
        if (!File.Exists(filePath))
            return new List<Expense>();

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
    }

    private void SaveExpenses(List<Expense> expenses, string filePath)
    {
        string json = JsonSerializer.Serialize(expenses, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private ExpenseReport GenerateReport(List<Expense> expenses)
    {
        var report = new ExpenseReport();

        foreach (var expense in expenses)
        {
            if (report.CategoryTotals.ContainsKey(expense.Category))
                report.CategoryTotals[expense.Category] += expense.Amount;
            else
                report.CategoryTotals.Add(expense.Category, expense.Amount);

            report.TotalExpenses += expense.Amount;
        }

        return report;
    }

    private void SaveReport(ExpenseReport report, string filePath)
    {
        string json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private void DisplayReport(ExpenseReport report)
    {
        Console.WriteLine("\nMonthly Expense Report:");
        Console.WriteLine("----------------------");

        foreach (var category in report.CategoryTotals)
        {
            Console.WriteLine(category.Key + ": " + category.Value.ToString("C"));
        }

        Console.WriteLine("----------------------");
        Console.WriteLine("Total Expenses: " + report.TotalExpenses.ToString("C"));
    }
}