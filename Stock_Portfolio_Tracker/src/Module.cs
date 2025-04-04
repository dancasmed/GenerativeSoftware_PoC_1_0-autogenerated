using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class StockPortfolioTracker : IGeneratedModule
{
    public string Name { get; set; } = "Stock Portfolio Tracker";
    
    private string portfolioFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Stock Portfolio Tracker module is running.");
        
        portfolioFilePath = Path.Combine(dataFolder, "portfolio.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        Portfolio portfolio = LoadPortfolio();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nStock Portfolio Tracker");
            Console.WriteLine("1. View Portfolio");
            Console.WriteLine("2. Add Stock");
            Console.WriteLine("3. Remove Stock");
            Console.WriteLine("4. Update Stock");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ViewPortfolio(portfolio);
                    break;
                case "2":
                    AddStock(ref portfolio);
                    break;
                case "3":
                    RemoveStock(ref portfolio);
                    break;
                case "4":
                    UpdateStock(ref portfolio);
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SavePortfolio(portfolio);
        Console.WriteLine("Portfolio saved. Exiting Stock Portfolio Tracker.");
        
        return true;
    }
    
    private Portfolio LoadPortfolio()
    {
        if (File.Exists(portfolioFilePath))
        {
            string json = File.ReadAllText(portfolioFilePath);
            return JsonSerializer.Deserialize<Portfolio>(json) ?? new Portfolio();
        }
        
        return new Portfolio();
    }
    
    private void SavePortfolio(Portfolio portfolio)
    {
        string json = JsonSerializer.Serialize(portfolio, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(portfolioFilePath, json);
    }
    
    private void ViewPortfolio(Portfolio portfolio)
    {
        if (portfolio.Stocks.Count == 0)
        {
            Console.WriteLine("Your portfolio is empty.");
            return;
        }
        
        Console.WriteLine("\nYour Portfolio:");
        Console.WriteLine("Symbol\tShares\tAvg Price\tCurrent Price\tValue\tGain/Loss");
        
        decimal totalValue = 0;
        decimal totalGainLoss = 0;
        
        foreach (var stock in portfolio.Stocks)
        {
            decimal currentPrice = GetCurrentStockPrice(stock.Symbol);
            decimal value = stock.Shares * currentPrice;
            decimal gainLoss = value - (stock.Shares * stock.AveragePrice);
            
            Console.WriteLine($"{stock.Symbol}\t{stock.Shares}\t{stock.AveragePrice:C}\t{currentPrice:C}\t{value:C}\t{gainLoss:C}");
            
            totalValue += value;
            totalGainLoss += gainLoss;
        }
        
        Console.WriteLine("\nTotal Portfolio Value: {0:C}", totalValue);
        Console.WriteLine("Total Gain/Loss: {0:C}", totalGainLoss);
    }
    
    private void AddStock(ref Portfolio portfolio)
    {
        Console.Write("Enter stock symbol: ");
        string symbol = Console.ReadLine().ToUpper().Trim();
        
        Console.Write("Enter number of shares: ");
        if (!int.TryParse(Console.ReadLine(), out int shares) || shares <= 0)
        {
            Console.WriteLine("Invalid number of shares.");
            return;
        }
        
        Console.Write("Enter average purchase price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal avgPrice) || avgPrice <= 0)
        {
            Console.WriteLine("Invalid price.");
            return;
        }
        
        var existingStock = portfolio.Stocks.Find(s => s.Symbol == symbol);
        if (existingStock != null)
        {
            Console.WriteLine("Stock already exists in portfolio. Use update option instead.");
            return;
        }
        
        portfolio.Stocks.Add(new Stock { Symbol = symbol, Shares = shares, AveragePrice = avgPrice });
        Console.WriteLine("Stock added to portfolio.");
    }
    
    private void RemoveStock(ref Portfolio portfolio)
    {
        if (portfolio.Stocks.Count == 0)
        {
            Console.WriteLine("Portfolio is empty.");
            return;
        }
        
        Console.Write("Enter stock symbol to remove: ");
        string symbol = Console.ReadLine().ToUpper().Trim();
        
        var stock = portfolio.Stocks.Find(s => s.Symbol == symbol);
        if (stock == null)
        {
            Console.WriteLine("Stock not found in portfolio.");
            return;
        }
        
        portfolio.Stocks.Remove(stock);
        Console.WriteLine("Stock removed from portfolio.");
    }
    
    private void UpdateStock(ref Portfolio portfolio)
    {
        if (portfolio.Stocks.Count == 0)
        {
            Console.WriteLine("Portfolio is empty.");
            return;
        }
        
        Console.Write("Enter stock symbol to update: ");
        string symbol = Console.ReadLine().ToUpper().Trim();
        
        var stock = portfolio.Stocks.Find(s => s.Symbol == symbol);
        if (stock == null)
        {
            Console.WriteLine("Stock not found in portfolio.");
            return;
        }
        
        Console.Write("Enter new number of shares (current: {0}): ", stock.Shares);
        string sharesInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(sharesInput))
        {
            if (int.TryParse(sharesInput, out int newShares) && newShares > 0)
            {
                stock.Shares = newShares;
            }
            else
            {
                Console.WriteLine("Invalid number of shares. Shares not updated.");
            }
        }
        
        Console.Write("Enter new average price (current: {0:C}): ", stock.AveragePrice);
        string priceInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(priceInput))
        {
            if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice > 0)
            {
                stock.AveragePrice = newPrice;
            }
            else
            {
                Console.WriteLine("Invalid price. Price not updated.");
            }
        }
        
        Console.WriteLine("Stock updated.");
    }
    
    private decimal GetCurrentStockPrice(string symbol)
    {
        // Simulate fetching stock price - in a real app this would call an API
        Random random = new Random();
        return Math.Round((decimal)(random.NextDouble() * 100 + 50), 2);
    }
}

public class Portfolio
{
    public List<Stock> Stocks { get; set; } = new List<Stock>();
}

public class Stock
{
    public string Symbol { get; set; }
    public int Shares { get; set; }
    public decimal AveragePrice { get; set; }
}