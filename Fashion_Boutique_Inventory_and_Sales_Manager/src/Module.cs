using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FashionBoutiqueModule : IGeneratedModule
{
    public string Name { get; set; } = "Fashion Boutique Inventory and Sales Manager";

    private string _inventoryFilePath;
    private string _salesFilePath;
    
    public FashionBoutiqueModule()
    {
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Fashion Boutique Inventory and Sales Manager...");
        
        _inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
        _salesFilePath = Path.Combine(dataFolder, "sales.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        InitializeFiles();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nFashion Boutique Management System");
            Console.WriteLine("1. Add Item to Inventory");
            Console.WriteLine("2. View Inventory");
            Console.WriteLine("3. Record Sale");
            Console.WriteLine("4. View Sales Records");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddInventoryItem();
                    break;
                case "2":
                    ViewInventory();
                    break;
                case "3":
                    RecordSale();
                    break;
                case "4":
                    ViewSales();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Fashion Boutique module completed successfully.");
        return true;
    }
    
    private void InitializeFiles()
    {
        if (!File.Exists(_inventoryFilePath))
        {
            File.WriteAllText(_inventoryFilePath, "[]");
        }
        
        if (!File.Exists(_salesFilePath))
        {
            File.WriteAllText(_salesFilePath, "[]");
        }
    }
    
    private void AddInventoryItem()
    {
        Console.Write("Enter item name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter item category: ");
        string category = Console.ReadLine();
        
        Console.Write("Enter item price: ");
        decimal price;
        while (!decimal.TryParse(Console.ReadLine(), out price))
        {
            Console.Write("Invalid price. Please enter a valid decimal value: ");
        }
        
        Console.Write("Enter item quantity: ");
        int quantity;
        while (!int.TryParse(Console.ReadLine(), out quantity))
        {
            Console.Write("Invalid quantity. Please enter a valid integer value: ");
        }
        
        var inventory = GetInventory();
        inventory.Add(new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            Price = price,
            Quantity = quantity,
            DateAdded = DateTime.Now
        });
        
        SaveInventory(inventory);
        Console.WriteLine("Item added to inventory successfully.");
    }
    
    private void ViewInventory()
    {
        var inventory = GetInventory();
        
        if (inventory.Count == 0)
        {
            Console.WriteLine("Inventory is empty.");
            return;
        }
        
        Console.WriteLine("\nCurrent Inventory:");
        Console.WriteLine("ID\t\t\t\tName\t\tCategory\tPrice\tQuantity\tDate Added");
        
        foreach (var item in inventory)
        {
            Console.WriteLine(item.Id + "\t" + item.Name + "\t\t" + item.Category + "\t\t" + 
                              item.Price + "\t" + item.Quantity + "\t\t" + item.DateAdded);
        }
    }
    
    private void RecordSale()
    {
        ViewInventory();
        
        Console.Write("Enter the ID of the item sold: ");
        string idInput = Console.ReadLine();
        
        if (!Guid.TryParse(idInput, out Guid itemId))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }
        
        var inventory = GetInventory();
        var item = inventory.Find(i => i.Id == itemId);
        
        if (item == null)
        {
            Console.WriteLine("Item not found in inventory.");
            return;
        }
        
        Console.Write("Enter quantity sold: ");
        int quantitySold;
        while (!int.TryParse(Console.ReadLine(), out quantitySold) || quantitySold <= 0)
        {
            Console.Write("Invalid quantity. Please enter a positive integer: ");
        }
        
        if (quantitySold > item.Quantity)
        {
            Console.WriteLine("Not enough stock available.");
            return;
        }
        
        item.Quantity -= quantitySold;
        
        var sales = GetSales();
        sales.Add(new SaleRecord
        {
            Id = Guid.NewGuid(),
            ItemId = item.Id,
            ItemName = item.Name,
            QuantitySold = quantitySold,
            UnitPrice = item.Price,
            TotalPrice = item.Price * quantitySold,
            SaleDate = DateTime.Now
        });
        
        SaveInventory(inventory);
        SaveSales(sales);
        
        Console.WriteLine("Sale recorded successfully. Total: " + (item.Price * quantitySold));
    }
    
    private void ViewSales()
    {
        var sales = GetSales();
        
        if (sales.Count == 0)
        {
            Console.WriteLine("No sales records found.");
            return;
        }
        
        Console.WriteLine("\nSales Records:");
        Console.WriteLine("ID\t\t\t\tItem Name\tQuantity\tUnit Price\tTotal\tDate");
        
        foreach (var sale in sales)
        {
            Console.WriteLine(sale.Id + "\t" + sale.ItemName + "\t\t" + sale.QuantitySold + "\t\t" + 
                              sale.UnitPrice + "\t\t" + sale.TotalPrice + "\t" + sale.SaleDate);
        }
    }
    
    private List<InventoryItem> GetInventory()
    {
        string json = File.ReadAllText(_inventoryFilePath);
        return JsonSerializer.Deserialize<List<InventoryItem>>(json) ?? new List<InventoryItem>();
    }
    
    private void SaveInventory(List<InventoryItem> inventory)
    {
        string json = JsonSerializer.Serialize(inventory);
        File.WriteAllText(_inventoryFilePath, json);
    }
    
    private List<SaleRecord> GetSales()
    {
        string json = File.ReadAllText(_salesFilePath);
        return JsonSerializer.Deserialize<List<SaleRecord>>(json) ?? new List<SaleRecord>();
    }
    
    private void SaveSales(List<SaleRecord> sales)
    {
        string json = JsonSerializer.Serialize(sales);
        File.WriteAllText(_salesFilePath, json);
    }
}

public class InventoryItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime DateAdded { get; set; }
}

public class SaleRecord
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public int QuantitySold { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime SaleDate { get; set; }
}