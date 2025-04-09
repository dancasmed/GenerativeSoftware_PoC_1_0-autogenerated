using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ShoppingCartModule : IGeneratedModule
{
    public string Name { get; set; } = "Shopping Cart Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Shopping Cart Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "shopping_config.json");
            string itemsPath = Path.Combine(dataFolder, "shopping_items.json");
            
            if (!File.Exists(configPath) || !File.Exists(itemsPath))
            {
                CreateDefaultFiles(configPath, itemsPath);
                Console.WriteLine("Default configuration and items files created. Please populate them and run again.");
                return false;
            }
            
            var config = JsonSerializer.Deserialize<ShoppingConfig>(File.ReadAllText(configPath));
            var items = JsonSerializer.Deserialize<List<ShoppingItem>>(File.ReadAllText(itemsPath));
            
            if (config == null || items == null || items.Count == 0)
            {
                Console.WriteLine("Invalid configuration or empty shopping cart.");
                return false;
            }
            
            decimal subtotal = CalculateSubtotal(items);
            decimal discount = CalculateDiscount(subtotal, config);
            decimal tax = CalculateTax(subtotal - discount, config.TaxRate);
            decimal total = subtotal - discount + tax;
            
            Console.WriteLine("\n--- Shopping Cart Summary ---");
            Console.WriteLine("Subtotal: " + subtotal.ToString("C2"));
            Console.WriteLine("Discount: " + discount.ToString("C2"));
            Console.WriteLine("Tax: " + tax.ToString("C2"));
            Console.WriteLine("Total: " + total.ToString("C2"));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error processing shopping cart: " + ex.Message);
            return false;
        }
    }
    
    private decimal CalculateSubtotal(List<ShoppingItem> items)
    {
        decimal subtotal = 0;
        foreach (var item in items)
        {
            subtotal += item.Price * item.Quantity;
        }
        return subtotal;
    }
    
    private decimal CalculateDiscount(decimal subtotal, ShoppingConfig config)
    {
        if (subtotal >= config.DiscountThreshold)
        {
            return subtotal * (config.DiscountPercentage / 100m);
        }
        return 0;
    }
    
    private decimal CalculateTax(decimal amount, decimal taxRate)
    {
        return amount * (taxRate / 100m);
    }
    
    private void CreateDefaultFiles(string configPath, string itemsPath)
    {
        var defaultConfig = new ShoppingConfig
        {
            TaxRate = 8.5m,
            DiscountThreshold = 100,
            DiscountPercentage = 10
        };
        
        var defaultItems = new List<ShoppingItem>
        {
            new ShoppingItem { Name = "Item 1", Price = 25.99m, Quantity = 2 },
            new ShoppingItem { Name = "Item 2", Price = 12.50m, Quantity = 1 }
        };
        
        File.WriteAllText(configPath, JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true }));
        File.WriteAllText(itemsPath, JsonSerializer.Serialize(defaultItems, new JsonSerializerOptions { WriteIndented = true }));
    }
}

public class ShoppingConfig
{
    public decimal TaxRate { get; set; }
    public decimal DiscountThreshold { get; set; }
    public decimal DiscountPercentage { get; set; }
}

public class ShoppingItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}