using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WaterUsageTracker : IGeneratedModule
{
    public string Name { get; set; } = "Household Water Usage Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Household Water Usage Tracker...");
        
        _dataFilePath = Path.Combine(dataFolder, "water_usage_data.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        var waterUsageData = LoadWaterUsageData();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            Console.WriteLine("\nHousehold Water Usage Tracker");
            Console.WriteLine("1. Add water usage entry");
            Console.WriteLine("2. View usage statistics");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            
            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }
            
            switch (option)
            {
                case 1:
                    AddWaterUsageEntry(waterUsageData);
                    break;
                case 2:
                    DisplayUsageStatistics(waterUsageData);
                    break;
                case 3:
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveWaterUsageData(waterUsageData);
        }
        
        Console.WriteLine("Water usage data saved. Exiting module...");
        return true;
    }
    
    private List<WaterUsageEntry> LoadWaterUsageData()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<WaterUsageEntry>();
        }
        
        string jsonData = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<WaterUsageEntry>>(jsonData) ?? new List<WaterUsageEntry>();
    }
    
    private void SaveWaterUsageData(List<WaterUsageEntry> data)
    {
        string jsonData = JsonSerializer.Serialize(data);
        File.WriteAllText(_dataFilePath, jsonData);
    }
    
    private void AddWaterUsageEntry(List<WaterUsageEntry> waterUsageData)
    {
        Console.Write("Enter date (YYYY-MM-DD): ");
        string dateString = Console.ReadLine();
        
        if (!DateTime.TryParse(dateString, out DateTime date))
        {
            Console.WriteLine("Invalid date format. Please use YYYY-MM-DD.");
            return;
        }
        
        Console.Write("Enter water usage in liters: ");
        if (!double.TryParse(Console.ReadLine(), out double liters))
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            return;
        }
        
        Console.Write("Enter usage category (e.g., Shower, Laundry, Kitchen): ");
        string category = Console.ReadLine();
        
        waterUsageData.Add(new WaterUsageEntry
        {
            Date = date,
            Liters = liters,
            Category = category
        });
        
        Console.WriteLine("Water usage entry added successfully.");
    }
    
    private void DisplayUsageStatistics(List<WaterUsageEntry> waterUsageData)
    {
        if (waterUsageData.Count == 0)
        {
            Console.WriteLine("No water usage data available.");
            return;
        }
        
        double totalUsage = 0;
        var categoryUsage = new Dictionary<string, double>();
        
        foreach (var entry in waterUsageData)
        {
            totalUsage += entry.Liters;
            
            if (categoryUsage.ContainsKey(entry.Category))
            {
                categoryUsage[entry.Category] += entry.Liters;
            }
            else
            {
                categoryUsage[entry.Category] = entry.Liters;
            }
        }
        
        Console.WriteLine("\nWater Usage Statistics");
        Console.WriteLine("Total Usage: " + totalUsage + " liters");
        Console.WriteLine("Average Daily Usage: " + (totalUsage / waterUsageData.Count) + " liters");
        
        Console.WriteLine("\nUsage by Category:");
        foreach (var category in categoryUsage)
        {
            double percentage = (category.Value / totalUsage) * 100;
            Console.WriteLine(category.Key + ": " + category.Value + " liters (" + percentage.ToString("0.00") + "%)");
        }
    }
}

public class WaterUsageEntry
{
    public DateTime Date { get; set; }
    public double Liters { get; set; }
    public string Category { get; set; }
}