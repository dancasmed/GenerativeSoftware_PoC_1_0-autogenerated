using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class InvestmentPortfolioTracker : IGeneratedModule
{
    public string Name { get; set; } = "Investment Portfolio Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Investment Portfolio Tracker module is running.");
        
        try
        {
            _dataFilePath = Path.Combine(dataFolder, "portfolio_data.json");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            var portfolio = LoadPortfolio();
            
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nPortfolio Tracker Menu:");
                Console.WriteLine("1. Add Investment");
                Console.WriteLine("2. View Portfolio");
                Console.WriteLine("3. Calculate Performance");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");
                
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddInvestment(portfolio);
                        break;
                    case "2":
                        ViewPortfolio(portfolio);
                        break;
                    case "3":
                        CalculatePerformance(portfolio);
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SavePortfolio(portfolio);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private List<Investment> LoadPortfolio()
    {
        if (File.Exists(_dataFilePath))
        {
            var json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<Investment>>(json) ?? new List<Investment>();
        }
        return new List<Investment>();
    }
    
    private void SavePortfolio(List<Investment> portfolio)
    {
        var json = JsonSerializer.Serialize(portfolio);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddInvestment(List<Investment> portfolio)
    {
        Console.Write("Enter investment name: ");
        var name = Console.ReadLine();
        
        Console.Write("Enter initial amount invested: ");
        if (!decimal.TryParse(Console.ReadLine(), out var amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }
        
        Console.Write("Enter current value: ");
        if (!decimal.TryParse(Console.ReadLine(), out var currentValue))
        {
            Console.WriteLine("Invalid current value.");
            return;
        }
        
        portfolio.Add(new Investment
        {
            Name = name,
            InitialAmount = amount,
            CurrentValue = currentValue,
            PurchaseDate = DateTime.Now
        });
        
        Console.WriteLine("Investment added successfully.");
    }
    
    private void ViewPortfolio(List<Investment> portfolio)
    {
        if (portfolio.Count == 0)
        {
            Console.WriteLine("No investments in portfolio.");
            return;
        }
        
        Console.WriteLine("\nCurrent Portfolio:");
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("| Name           | Initial Amt | Current Val | Gain/Loss |");
        Console.WriteLine("-------------------------------------------------");
        
        foreach (var investment in portfolio)
        {
            var gainLoss = investment.CurrentValue - investment.InitialAmount;
            Console.WriteLine(string.Format("| {0,-15} | {1,11:C2} | {2,11:C2} | {3,9:C2} |", 
                investment.Name, investment.InitialAmount, investment.CurrentValue, gainLoss));
        }
        
        Console.WriteLine("-------------------------------------------------");
    }
    
    private void CalculatePerformance(List<Investment> portfolio)
    {
        if (portfolio.Count == 0)
        {
            Console.WriteLine("No investments in portfolio.");
            return;
        }
        
        decimal totalInitial = 0;
        decimal totalCurrent = 0;
        
        foreach (var investment in portfolio)
        {
            totalInitial += investment.InitialAmount;
            totalCurrent += investment.CurrentValue;
        }
        
        var totalGainLoss = totalCurrent - totalInitial;
        var percentageChange = (totalGainLoss / totalInitial) * 100;
        
        Console.WriteLine("\nPortfolio Performance Summary:");
        Console.WriteLine("Total Initial Investment: " + totalInitial.ToString("C2"));
        Console.WriteLine("Total Current Value: " + totalCurrent.ToString("C2"));
        Console.WriteLine("Total Gain/Loss: " + totalGainLoss.ToString("C2"));
        Console.WriteLine("Percentage Change: " + percentageChange.ToString("F2") + "%");
    }
}

public class Investment
{
    public string Name { get; set; }
    public decimal InitialAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public DateTime PurchaseDate { get; set; }
}