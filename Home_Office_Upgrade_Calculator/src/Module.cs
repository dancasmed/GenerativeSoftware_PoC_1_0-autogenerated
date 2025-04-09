using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HomeOfficeUpgradeCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Office Upgrade Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Office Upgrade Calculator...");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "home_office_config.json");
            string resultPath = Path.Combine(dataFolder, "home_office_result.json");
            
            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please edit it and run again.");
                return false;
            }
            
            var config = LoadConfig(configPath);
            var result = CalculateCosts(config);
            
            SaveResult(resultPath, result);
            
            Console.WriteLine("Calculation completed successfully!");
            Console.WriteLine("Total Cost: " + result.TotalCost.ToString("C"));
            Console.WriteLine("Results saved to: " + resultPath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void CreateDefaultConfig(string path)
    {
        var defaultConfig = new HomeOfficeConfig
        {
            TechnologyItems = new List<TechnologyItem>
            {
                new TechnologyItem { Name = "Laptop", Price = 999.99m, Quantity = 1 },
                new TechnologyItem { Name = "Monitor", Price = 249.99m, Quantity = 2 },
                new TechnologyItem { Name = "Keyboard", Price = 79.99m, Quantity = 1 },
                new TechnologyItem { Name = "Mouse", Price = 49.99m, Quantity = 1 },
                new TechnologyItem { Name = "Docking Station", Price = 199.99m, Quantity = 1 }
            },
            FurnitureItems = new List<FurnitureItem>
            {
                new FurnitureItem { Name = "Desk", Price = 399.99m, Quantity = 1 },
                new FurnitureItem { Name = "Chair", Price = 299.99m, Quantity = 1 },
                new FurnitureItem { Name = "Shelving Unit", Price = 149.99m, Quantity = 1 }
            },
            TaxRate = 0.08m,
            ShippingCost = 49.99m
        };
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(defaultConfig, options);
        File.WriteAllText(path, json);
    }
    
    private HomeOfficeConfig LoadConfig(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<HomeOfficeConfig>(json);
    }
    
    private CalculationResult CalculateCosts(HomeOfficeConfig config)
    {
        decimal techSubtotal = 0m;
        decimal furnitureSubtotal = 0m;
        
        foreach (var item in config.TechnologyItems)
        {
            techSubtotal += item.Price * item.Quantity;
        }
        
        foreach (var item in config.FurnitureItems)
        {
            furnitureSubtotal += item.Price * item.Quantity;
        }
        
        decimal subtotal = techSubtotal + furnitureSubtotal;
        decimal tax = subtotal * config.TaxRate;
        decimal total = subtotal + tax + config.ShippingCost;
        
        return new CalculationResult
        {
            TechnologySubtotal = techSubtotal,
            FurnitureSubtotal = furnitureSubtotal,
            Subtotal = subtotal,
            Tax = tax,
            ShippingCost = config.ShippingCost,
            TotalCost = total,
            CalculatedAt = DateTime.Now
        };
    }
    
    private void SaveResult(string path, CalculationResult result)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(result, options);
        File.WriteAllText(path, json);
    }
}

public class HomeOfficeConfig
{
    public List<TechnologyItem> TechnologyItems { get; set; }
    public List<FurnitureItem> FurnitureItems { get; set; }
    public decimal TaxRate { get; set; }
    public decimal ShippingCost { get; set; }
}

public class TechnologyItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class FurnitureItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class CalculationResult
{
    public decimal TechnologySubtotal { get; set; }
    public decimal FurnitureSubtotal { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime CalculatedAt { get; set; }
}