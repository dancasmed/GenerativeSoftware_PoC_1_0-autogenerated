using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WineCollectionModule : IGeneratedModule
{
    public string Name { get; set; } = "Wine Collection Manager";
    
    private string _dataFilePath;
    
    public WineCollectionModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Wine Collection Manager module...");
        
        _dataFilePath = Path.Combine(dataFolder, "winecollection.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<Wine> wines = LoadWines();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nWine Collection Manager");
            Console.WriteLine("1. Add new wine");
            Console.WriteLine("2. View all wines");
            Console.WriteLine("3. Search wines");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddWine(wines);
                    break;
                case "2":
                    DisplayAllWines(wines);
                    break;
                case "3":
                    SearchWines(wines);
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveWines(wines);
        Console.WriteLine("Wine collection saved. Exiting module...");
        return true;
    }
    
    private List<Wine> LoadWines()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<Wine>>(json) ?? new List<Wine>();
        }
        return new List<Wine>();
    }
    
    private void SaveWines(List<Wine> wines)
    {
        string json = JsonSerializer.Serialize(wines, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddWine(List<Wine> wines)
    {
        Console.WriteLine("\nAdd New Wine");
        
        Wine wine = new Wine();
        
        Console.Write("Name: ");
        wine.Name = Console.ReadLine();
        
        Console.Write("Type (Red/White/Rose/Sparkling): ");
        wine.Type = Console.ReadLine();
        
        Console.Write("Year: ");
        if (int.TryParse(Console.ReadLine(), out int year))
        {
            wine.Year = year;
        }
        
        Console.Write("Region: ");
        wine.Region = Console.ReadLine();
        
        Console.Write("Tasting Notes: ");
        wine.TastingNotes = Console.ReadLine();
        
        Console.Write("Rating (1-10): ");
        if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 10)
        {
            wine.Rating = rating;
        }
        else
        {
            wine.Rating = 5;
        }
        
        wines.Add(wine);
        Console.WriteLine("Wine added successfully!");
    }
    
    private void DisplayAllWines(List<Wine> wines)
    {
        Console.WriteLine("\nAll Wines in Collection:");
        
        if (wines.Count == 0)
        {
            Console.WriteLine("No wines in collection yet.");
            return;
        }
        
        foreach (var wine in wines)
        {
            DisplayWineDetails(wine);
        }
    }
    
    private void SearchWines(List<Wine> wines)
    {
        Console.WriteLine("\nSearch Wines");
        Console.WriteLine("1. By Name");
        Console.WriteLine("2. By Type");
        Console.WriteLine("3. By Region");
        Console.WriteLine("4. By Rating");
        Console.Write("Select search option: ");
        
        string input = Console.ReadLine();
        
        switch (input)
        {
            case "1":
                Console.Write("Enter name to search: ");
                string name = Console.ReadLine();
                SearchByName(wines, name);
                break;
            case "2":
                Console.Write("Enter type to search: ");
                string type = Console.ReadLine();
                SearchByType(wines, type);
                break;
            case "3":
                Console.Write("Enter region to search: ");
                string region = Console.ReadLine();
                SearchByRegion(wines, region);
                break;
            case "4":
                Console.Write("Enter minimum rating (1-10): ");
                if (int.TryParse(Console.ReadLine(), out int rating))
                {
                    SearchByRating(wines, rating);
                }
                else
                {
                    Console.WriteLine("Invalid rating value.");
                }
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    
    private void SearchByName(List<Wine> wines, string name)
    {
        var results = wines.FindAll(w => w.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        DisplaySearchResults(results, "Name", name);
    }
    
    private void SearchByType(List<Wine> wines, string type)
    {
        var results = wines.FindAll(w => w.Type.Contains(type, StringComparison.OrdinalIgnoreCase));
        DisplaySearchResults(results, "Type", type);
    }
    
    private void SearchByRegion(List<Wine> wines, string region)
    {
        var results = wines.FindAll(w => w.Region.Contains(region, StringComparison.OrdinalIgnoreCase));
        DisplaySearchResults(results, "Region", region);
    }
    
    private void SearchByRating(List<Wine> wines, int minRating)
    {
        var results = wines.FindAll(w => w.Rating >= minRating);
        DisplaySearchResults(results, "Rating", minRating.ToString());
    }
    
    private void DisplaySearchResults(List<Wine> results, string searchType, string searchValue)
    {
        Console.WriteLine("\nSearch Results ({0}: {1}):", searchType, searchValue);
        
        if (results.Count == 0)
        {
            Console.WriteLine("No wines found matching your criteria.");
            return;
        }
        
        foreach (var wine in results)
        {
            DisplayWineDetails(wine);
        }
    }
    
    private void DisplayWineDetails(Wine wine)
    {
        Console.WriteLine("\nName: {0}", wine.Name);
        Console.WriteLine("Type: {0}", wine.Type);
        Console.WriteLine("Year: {0}", wine.Year);
        Console.WriteLine("Region: {0}", wine.Region);
        Console.WriteLine("Tasting Notes: {0}", wine.TastingNotes);
        Console.WriteLine("Rating: {0}/10", wine.Rating);
        Console.WriteLine("----------------------------");
    }
}

public class Wine
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Year { get; set; } = DateTime.Now.Year;
    public string Region { get; set; } = string.Empty;
    public string TastingNotes { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
}