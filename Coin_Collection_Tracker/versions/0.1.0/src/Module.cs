using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CoinCollectionTracker : IGeneratedModule
{
    public string Name { get; set; } = "Coin Collection Tracker";
    private string _dataFilePath;
    
    public CoinCollectionTracker()
    {
    }
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "coin_collection.json");
        
        Console.WriteLine("Coin Collection Tracker module is running.");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<Coin> coins = LoadCoinCollection();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddCoin(coins);
                    break;
                case "2":
                    ViewCollection(coins);
                    break;
                case "3":
                    CalculateTotalValue(coins);
                    break;
                case "4":
                    SaveCoinCollection(coins);
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private List<Coin> LoadCoinCollection()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<Coin>>(json);
        }
        return new List<Coin>();
    }
    
    private void SaveCoinCollection(List<Coin> coins)
    {
        string json = JsonSerializer.Serialize(coins);
        File.WriteAllText(_dataFilePath, json);
        Console.WriteLine("Coin collection saved successfully.");
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nCoin Collection Tracker");
        Console.WriteLine("1. Add a new coin");
        Console.WriteLine("2. View coin collection");
        Console.WriteLine("3. Calculate total value");
        Console.WriteLine("4. Save and exit");
        Console.Write("Select an option: ");
    }
    
    private void AddCoin(List<Coin> coins)
    {
        Console.Write("Enter coin name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter coin year: ");
        string yearInput = Console.ReadLine();
        
        Console.Write("Enter coin value: ");
        string valueInput = Console.ReadLine();
        
        if (int.TryParse(yearInput, out int year) && decimal.TryParse(valueInput, out decimal value))
        {
            coins.Add(new Coin { Name = name, Year = year, Value = value });
            Console.WriteLine("Coin added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid input. Coin not added.");
        }
    }
    
    private void ViewCollection(List<Coin> coins)
    {
        if (coins.Count == 0)
        {
            Console.WriteLine("No coins in the collection.");
            return;
        }
        
        Console.WriteLine("\nCoin Collection:");
        foreach (var coin in coins)
        {
            Console.WriteLine($"{coin.Name} ({coin.Year}) - {coin.Value:C}");
        }
    }
    
    private void CalculateTotalValue(List<Coin> coins)
    {
        decimal total = 0;
        foreach (var coin in coins)
        {
            total += coin.Value;
        }
        
        Console.WriteLine($"Total collection value: {total:C}");
    }
}

public class Coin
{
    public string Name { get; set; }
    public int Year { get; set; }
    public decimal Value { get; set; }
}