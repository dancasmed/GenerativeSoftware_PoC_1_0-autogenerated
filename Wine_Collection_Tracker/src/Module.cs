using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WineCollectionModule
{
    public string Name { get; set; } = "Wine Collection Tracker";

    private string _dataFilePath;
    private List<Wine> _wines;

    public WineCollectionModule()
    {
        _wines = new List<Wine>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Wine Collection Tracker...");
        
        try
        {
            Directory.CreateDirectory(dataFolder);
            _dataFilePath = Path.Combine(dataFolder, "winecollection.json");
            
            if (File.Exists(_dataFilePath))
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _wines = JsonSerializer.Deserialize<List<Wine>>(jsonData);
                Console.WriteLine("Loaded existing wine collection data.");
            }
            else
            {
                Console.WriteLine("No existing wine collection found. Starting new collection.");
            }
            
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nWine Collection Tracker");
                Console.WriteLine("1. Add Wine");
                Console.WriteLine("2. View Collection");
                Console.WriteLine("3. Search Wines");
                Console.WriteLine("4. Save and Exit");
                Console.Write("Choose an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddWine();
                        break;
                    case "2":
                        ViewCollection();
                        break;
                    case "3":
                        SearchWines();
                        break;
                    case "4":
                        running = false;
                        SaveCollection();
                        Console.WriteLine("Wine collection saved. Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void AddWine()
    {
        Console.WriteLine("\nAdd New Wine");
        
        Console.Write("Wine Name: ");
        string name = Console.ReadLine();
        
        Console.Write("Vineyard: ");
        string vineyard = Console.ReadLine();
        
        Console.Write("Vintage (year): ");
        if (!int.TryParse(Console.ReadLine(), out int vintage))
        {
            Console.WriteLine("Invalid year. Using current year.");
            vintage = DateTime.Now.Year;
        }
        
        Console.Write("Region: ");
        string region = Console.ReadLine();
        
        Console.Write("Notes: ");
        string notes = Console.ReadLine();
        
        _wines.Add(new Wine
        {
            Name = name,
            Vineyard = vineyard,
            Vintage = vintage,
            Region = region,
            Notes = notes,
            AddedDate = DateTime.Now
        });
        
        Console.WriteLine("Wine added successfully!");
    }
    
    private void ViewCollection()
    {
        Console.WriteLine("\nWine Collection (" + _wines.Count + " items)");
        Console.WriteLine("------------------------------------------------");
        
        if (_wines.Count == 0)
        {
            Console.WriteLine("No wines in collection yet.");
            return;
        }
        
        foreach (var wine in _wines)
        {
            Console.WriteLine($"{wine.Name} ({wine.Vintage})");
            Console.WriteLine($"Vineyard: {wine.Vineyard}");
            Console.WriteLine($"Region: {wine.Region}");
            Console.WriteLine($"Added: {wine.AddedDate:yyyy-MM-dd}");
            Console.WriteLine($"Notes: {wine.Notes}");
            Console.WriteLine("------------------------------------------------");
        }
    }
    
    private void SearchWines()
    {
        Console.Write("\nSearch term: ");
        string term = Console.ReadLine().ToLower();
        
        var results = _wines.FindAll(w => 
            w.Name.ToLower().Contains(term) || 
            w.Vineyard.ToLower().Contains(term) || 
            w.Region.ToLower().Contains(term) || 
            w.Notes.ToLower().Contains(term));
        
        Console.WriteLine("\nSearch Results (" + results.Count + " matches)");
        Console.WriteLine("------------------------------------------------");
        
        foreach (var wine in results)
        {
            Console.WriteLine($"{wine.Name} ({wine.Vintage})");
            Console.WriteLine($"Vineyard: {wine.Vineyard}");
            Console.WriteLine("------------------------------------------------");
        }
    }
    
    private void SaveCollection()
    {
        string jsonData = JsonSerializer.Serialize(_wines);
        File.WriteAllText(_dataFilePath, jsonData);
    }
}

public class Wine
{
    public string Name { get; set; }
    public string Vineyard { get; set; }
    public int Vintage { get; set; }
    public string Region { get; set; }
    public string Notes { get; set; }
    public DateTime AddedDate { get; set; }
}