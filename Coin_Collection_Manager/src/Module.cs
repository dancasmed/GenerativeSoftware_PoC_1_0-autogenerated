using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CoinCollectionManager : IGeneratedModule
{
    public string Name { get; set; } = "Coin Collection Manager";
    
    private List<Coin> _coins = new List<Coin>();
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Coin Collection Manager...");
        
        _dataFilePath = Path.Combine(dataFolder, "coin_collection.json");
        
        LoadCollection();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddCoin();
                    break;
                case "2":
                    ViewCollection();
                    break;
                case "3":
                    SearchByRarity();
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveCollection();
        Console.WriteLine("Coin collection saved. Exiting...");
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nCoin Collection Manager");
        Console.WriteLine("1. Add a new coin");
        Console.WriteLine("2. View collection");
        Console.WriteLine("3. Search by rarity");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddCoin()
    {
        Console.Write("Enter coin name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter coin year: ");
        string year = Console.ReadLine();
        
        Console.Write("Enter coin country: ");
        string country = Console.ReadLine();
        
        Console.Write("Enter rarity (Common, Uncommon, Rare, Very Rare, Extremely Rare): ");
        string rarity = Console.ReadLine();
        
        _coins.Add(new Coin
        {
            Name = name,
            Year = year,
            Country = country,
            Rarity = rarity
        });
        
        Console.WriteLine("Coin added successfully!");
    }
    
    private void ViewCollection()
    {
        if (_coins.Count == 0)
        {
            Console.WriteLine("Your collection is empty.");
            return;
        }
        
        Console.WriteLine("\nYour Coin Collection:");
        Console.WriteLine("-------------------");
        
        foreach (var coin in _coins)
        {
            Console.WriteLine("Name: " + coin.Name);
            Console.WriteLine("Year: " + coin.Year);
            Console.WriteLine("Country: " + coin.Country);
            Console.WriteLine("Rarity: " + coin.Rarity);
            Console.WriteLine("-------------------");
        }
    }
    
    private void SearchByRarity()
    {
        Console.Write("Enter rarity to search for: ");
        string rarity = Console.ReadLine();
        
        var results = _coins.FindAll(c => c.Rarity.Equals(rarity, StringComparison.OrdinalIgnoreCase));
        
        if (results.Count == 0)
        {
            Console.WriteLine("No coins found with that rarity.");
            return;
        }
        
        Console.WriteLine("\nCoins with rarity " + rarity + ":");
        Console.WriteLine("-------------------");
        
        foreach (var coin in results)
        {
            Console.WriteLine("Name: " + coin.Name);
            Console.WriteLine("Year: " + coin.Year);
            Console.WriteLine("Country: " + coin.Country);
            Console.WriteLine("-------------------");
        }
    }
    
    private void LoadCollection()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _coins = JsonSerializer.Deserialize<List<Coin>>(json);
                Console.WriteLine("Collection loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading collection: " + ex.Message);
            }
        }
    }
    
    private void SaveCollection()
    {
        try
        {
            string json = JsonSerializer.Serialize(_coins);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving collection: " + ex.Message);
        }
    }
}

public class Coin
{
    public string Name { get; set; }
    public string Year { get; set; }
    public string Country { get; set; }
    public string Rarity { get; set; }
}