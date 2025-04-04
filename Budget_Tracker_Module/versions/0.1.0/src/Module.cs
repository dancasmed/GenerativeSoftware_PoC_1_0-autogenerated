using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BudgetTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Budget Tracker Module";
    
    private string _transactionsFilePath;
    private List<Transaction> _transactions;
    
    public BudgetTrackerModule()
    {
        _transactions = new List<Transaction>();
    }
    
    public bool Main(string dataFolder)
    {
        _transactionsFilePath = Path.Combine(dataFolder, "transactions.json");
        
        Console.WriteLine("Budget Tracker Module is running.");
        Console.WriteLine("Loading existing transactions...");
        
        LoadTransactions();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTransaction();
                    break;
                case "2":
                    ViewTransactions();
                    break;
                case "3":
                    ViewBalance();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Saving transactions before exiting...");
        SaveTransactions();
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nBudget Tracker Menu:");
        Console.WriteLine("1. Add Income/Expense");
        Console.WriteLine("2. View All Transactions");
        Console.WriteLine("3. View Current Balance");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddTransaction()
    {
        Console.WriteLine("\nAdd New Transaction");
        
        Console.Write("Enter description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter amount (positive for income, negative for expense): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Transaction not added.");
            return;
        }
        
        Console.Write("Enter date (YYYY-MM-DD) or leave empty for today: ");
        string dateInput = Console.ReadLine();
        
        DateTime date;
        if (string.IsNullOrWhiteSpace(dateInput))
        {
            date = DateTime.Today;
        }
        else if (!DateTime.TryParse(dateInput, out date))
        {
            Console.WriteLine("Invalid date format. Using today's date.");
            date = DateTime.Today;
        }
        
        var transaction = new Transaction
        {
            Description = description,
            Amount = amount,
            Date = date,
            TransactionType = amount >= 0 ? TransactionType.Income : TransactionType.Expense
        };
        
        _transactions.Add(transaction);
        Console.WriteLine("Transaction added successfully.");
    }
    
    private void ViewTransactions()
    {
        if (_transactions.Count == 0)
        {
            Console.WriteLine("No transactions found.");
            return;
        }
        
        Console.WriteLine("\nAll Transactions:");
        Console.WriteLine("Date\t\tType\t\tAmount\tDescription");
        Console.WriteLine(new string('-', 60));
        
        foreach (var transaction in _transactions)
        {
            Console.WriteLine(string.Format("{0:yyyy-MM-dd}\t{1}\t{2:C}\t{3}", 
                transaction.Date, 
                transaction.TransactionType, 
                transaction.Amount, 
                transaction.Description));
        }
    }
    
    private void ViewBalance()
    {
        decimal balance = 0;
        foreach (var transaction in _transactions)
        {
            balance += transaction.Amount;
        }
        
        Console.WriteLine(string.Format("\nCurrent Balance: {0:C}", balance));
    }
    
    private void LoadTransactions()
    {
        try
        {
            if (File.Exists(_transactionsFilePath))
            {
                string json = File.ReadAllText(_transactionsFilePath);
                _transactions = JsonSerializer.Deserialize<List<Transaction>>(json);
                Console.WriteLine(string.Format("Loaded {0} transactions.", _transactions.Count));
            }
            else
            {
                Console.WriteLine("No existing transaction file found. Starting with empty list.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("Error loading transactions: {0}", ex.Message));
        }
    }
    
    private void SaveTransactions()
    {
        try
        {
            string json = JsonSerializer.Serialize(_transactions);
            File.WriteAllText(_transactionsFilePath, json);
            Console.WriteLine(string.Format("Saved {0} transactions.", _transactions.Count));
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("Error saving transactions: {0}", ex.Message));
        }
    }
}

public class Transaction
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType TransactionType { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
}