using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WasteReductionTracker : IGeneratedModule
{
    public string Name { get; set; } = "Waste Reduction Tracker";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Waste Reduction Tracker module is running...");
        
        try
        {
            string dataFilePath = Path.Combine(dataFolder, "waste_data.json");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            List<WasteEntry> entries = LoadData(dataFilePath);
            
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nWaste Reduction Tracker");
                Console.WriteLine("1. Add new waste entry");
                Console.WriteLine("2. View all entries");
                Console.WriteLine("3. Analyze waste reduction");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddWasteEntry(entries);
                        break;
                    case "2":
                        ViewAllEntries(entries);
                        break;
                    case "3":
                        AnalyzeWasteReduction(entries);
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                
                SaveData(dataFilePath, entries);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private List<WasteEntry> LoadData(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<WasteEntry>>(json) ?? new List<WasteEntry>();
        }
        return new List<WasteEntry>();
    }
    
    private void SaveData(string filePath, List<WasteEntry> entries)
    {
        string json = JsonSerializer.Serialize(entries);
        File.WriteAllText(filePath, json);
    }
    
    private void AddWasteEntry(List<WasteEntry> entries)
    {
        Console.Write("Enter date (YYYY-MM-DD): ");
        string dateInput = Console.ReadLine();
        
        Console.Write("Enter waste type: ");
        string wasteType = Console.ReadLine();
        
        Console.Write("Enter amount in kg: ");
        string amountInput = Console.ReadLine();
        
        if (DateTime.TryParse(dateInput, out DateTime date) && double.TryParse(amountInput, out double amount))
        {
            entries.Add(new WasteEntry { Date = date, WasteType = wasteType, Amount = amount });
            Console.WriteLine("Entry added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid input. Entry not added.");
        }
    }
    
    private void ViewAllEntries(List<WasteEntry> entries)
    {
        if (entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            return;
        }
        
        Console.WriteLine("\nAll Waste Entries:");
        foreach (var entry in entries)
        {
            Console.WriteLine($"{entry.Date:yyyy-MM-dd} - {entry.WasteType}: {entry.Amount} kg");
        }
    }
    
    private void AnalyzeWasteReduction(List<WasteEntry> entries)
    {
        if (entries.Count < 2)
        {
            Console.WriteLine("Not enough data for analysis. Please add more entries.");
            return;
        }
        
        // Group by waste type and calculate average reduction
        var wasteGroups = new Dictionary<string, List<double>>();
        
        foreach (var entry in entries)
        {
            if (!wasteGroups.ContainsKey(entry.WasteType))
            {
                wasteGroups[entry.WasteType] = new List<double>();
            }
            wasteGroups[entry.WasteType].Add(entry.Amount);
        }
        
        Console.WriteLine("\nWaste Reduction Analysis:");
        
        foreach (var group in wasteGroups)
        {
            if (group.Value.Count > 1)
            {
                double first = group.Value[0];
                double last = group.Value[^1];
                double reduction = first - last;
                double percentage = (reduction / first) * 100;
                
                Console.WriteLine($"{group.Key}: {reduction:0.00} kg reduction ({percentage:0.00}%)");
            }
        }
    }
}

public class WasteEntry
{
    public DateTime Date { get; set; }
    public string WasteType { get; set; }
    public double Amount { get; set; }
}