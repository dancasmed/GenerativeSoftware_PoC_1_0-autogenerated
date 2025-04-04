using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BirdSightingTracker : IGeneratedModule
{
    public string Name { get; set; } = "Bird Sighting Tracker";
    private string sightingsFilePath;
    
    public BirdSightingTracker()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Bird Sighting Tracker module is running.");
        
        sightingsFilePath = Path.Combine(dataFolder, "bird_sightings.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<BirdSighting> sightings = LoadSightings();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add new sighting");
            Console.WriteLine("2. View all sightings");
            Console.WriteLine("3. Search sightings by species");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddSighting(sightings);
                    break;
                case "2":
                    DisplayAllSightings(sightings);
                    break;
                case "3":
                    SearchBySpecies(sightings);
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveSightings(sightings);
        Console.WriteLine("Bird sightings saved. Exiting module.");
        return true;
    }
    
    private List<BirdSighting> LoadSightings()
    {
        if (File.Exists(sightingsFilePath))
        {
            string json = File.ReadAllText(sightingsFilePath);
            return JsonSerializer.Deserialize<List<BirdSighting>>(json) ?? new List<BirdSighting>();
        }
        return new List<BirdSighting>();
    }
    
    private void SaveSightings(List<BirdSighting> sightings)
    {
        string json = JsonSerializer.Serialize(sightings);
        File.WriteAllText(sightingsFilePath, json);
    }
    
    private void AddSighting(List<BirdSighting> sightings)
    {
        Console.Write("Enter bird species: ");
        string species = Console.ReadLine();
        
        Console.Write("Enter location: ");
        string location = Console.ReadLine();
        
        Console.Write("Enter date (yyyy-MM-dd): ");
        string dateInput = Console.ReadLine();
        
        if (DateTime.TryParse(dateInput, out DateTime date))
        {
            sightings.Add(new BirdSighting(species, location, date));
            Console.WriteLine("Sighting added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format. Sighting not added.");
        }
    }
    
    private void DisplayAllSightings(List<BirdSighting> sightings)
    {
        if (sightings.Count == 0)
        {
            Console.WriteLine("No sightings recorded yet.");
            return;
        }
        
        Console.WriteLine("\nAll Bird Sightings:");
        foreach (var sighting in sightings)
        {
            Console.WriteLine(sighting.ToString());
        }
    }
    
    private void SearchBySpecies(List<BirdSighting> sightings)
    {
        Console.Write("Enter species to search for: ");
        string searchTerm = Console.ReadLine();
        
        var results = sightings.FindAll(s => 
            s.Species.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        
        if (results.Count == 0)
        {
            Console.WriteLine("No sightings found for that species.");
        }
        else
        {
            Console.WriteLine("\nMatching Sightings:");
            foreach (var sighting in results)
            {
                Console.WriteLine(sighting.ToString());
            }
        }
    }
}

public class BirdSighting
{
    public string Species { get; set; }
    public string Location { get; set; }
    public DateTime Date { get; set; }
    
    public BirdSighting(string species, string location, DateTime date)
    {
        Species = species;
        Location = location;
        Date = date;
    }
    
    public override string ToString()
    {
        return $"{Date:yyyy-MM-dd}: {Species} at {Location}";
    }
}