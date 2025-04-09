using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HomeOfficeCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Office Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Office Calculator...");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "home_office_config.json");
            
            if (!File.Exists(configPath))
            {
                Console.WriteLine("No configuration file found. Creating default configuration.");
                CreateDefaultConfig(configPath);
            }
            
            var config = LoadConfig(configPath);
            double totalCost = CalculateTotalCost(config);
            
            Console.WriteLine("Home Office Setup Calculation Complete");
            Console.WriteLine("Total Cost: " + totalCost.ToString("C"));
            
            SaveResult(dataFolder, totalCost);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void CreateDefaultConfig(string configPath)
    {
        var defaultConfig = new HomeOfficeConfig
        {
            FurnitureItems = new List<FurnitureItem>
            {
                new FurnitureItem { Name = "Desk", Price = 200.00 },
                new FurnitureItem { Name = "Chair", Price = 150.00 },
                new FurnitureItem { Name = "Bookshelf", Price = 80.00 }
            },
            EquipmentItems = new List<EquipmentItem>
            {
                new EquipmentItem { Name = "Computer", Price = 1000.00 },
                new EquipmentItem { Name = "Monitor", Price = 250.00 },
                new EquipmentItem { Name = "Keyboard", Price = 50.00 },
                new EquipmentItem { Name = "Mouse", Price = 30.00 }
            }
        };
        
        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configPath, json);
    }
    
    private HomeOfficeConfig LoadConfig(string configPath)
    {
        string json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<HomeOfficeConfig>(json);
    }
    
    private double CalculateTotalCost(HomeOfficeConfig config)
    {
        double total = 0.0;
        
        foreach (var item in config.FurnitureItems)
        {
            total += item.Price;
        }
        
        foreach (var item in config.EquipmentItems)
        {
            total += item.Price;
        }
        
        return total;
    }
    
    private void SaveResult(string dataFolder, double totalCost)
    {
        string resultPath = Path.Combine(dataFolder, "home_office_result.json");
        var result = new { TotalCost = totalCost, CalculationDate = DateTime.Now };
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(resultPath, json);
    }
}

public class HomeOfficeConfig
{
    public List<FurnitureItem> FurnitureItems { get; set; }
    public List<EquipmentItem> EquipmentItems { get; set; }
}

public class FurnitureItem
{
    public string Name { get; set; }
    public double Price { get; set; }
}

public class EquipmentItem
{
    public string Name { get; set; }
    public double Price { get; set; }
}