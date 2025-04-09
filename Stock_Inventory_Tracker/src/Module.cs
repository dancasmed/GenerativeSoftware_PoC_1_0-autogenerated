using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class StockInventoryTracker : IGeneratedModule
{
    public string Name { get; set; } = "Stock Inventory Tracker";
    
    private string _inventoryFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Stock Inventory Tracker module is running.");
        
        _inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        bool exitRequested = false;
        
        while (!exitRequested)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddItem();
                    break;
                case "2":
                    RemoveItem();
                    break;
                case "3":
                    UpdateItem();
                    break;
                case "4":
                    ViewInventory();
                    break;
                case "5":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Exiting Stock Inventory Tracker module.");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nStock Inventory Tracker Menu:");
        Console.WriteLine("1. Add Item");
        Console.WriteLine("2. Remove Item");
        Console.WriteLine("3. Update Item Quantity");
        Console.WriteLine("4. View Inventory");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private List<InventoryItem> LoadInventory()
    {
        if (!File.Exists(_inventoryFilePath))
        {
            return new List<InventoryItem>();
        }
        
        string json = File.ReadAllText(_inventoryFilePath);
        return JsonSerializer.Deserialize<List<InventoryItem>>(json);
    }
    
    private void SaveInventory(List<InventoryItem> inventory)
    {
        string json = JsonSerializer.Serialize(inventory, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_inventoryFilePath, json);
    }
    
    private void AddItem()
    {
        Console.Write("Enter item ID: ");
        string id = Console.ReadLine();
        
        Console.Write("Enter item name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter item quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity. Operation cancelled.");
            return;
        }
        
        Console.Write("Enter item price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price. Operation cancelled.");
            return;
        }
        
        var inventory = LoadInventory();
        
        if (inventory.Exists(item => item.Id == id))
        {
            Console.WriteLine("Item with this ID already exists.");
            return;
        }
        
        inventory.Add(new InventoryItem
        {
            Id = id,
            Name = name,
            Quantity = quantity,
            Price = price
        });
        
        SaveInventory(inventory);
        Console.WriteLine("Item added successfully.");
    }
    
    private void RemoveItem()
    {
        Console.Write("Enter item ID to remove: ");
        string id = Console.ReadLine();
        
        var inventory = LoadInventory();
        var itemToRemove = inventory.Find(item => item.Id == id);
        
        if (itemToRemove == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }
        
        inventory.Remove(itemToRemove);
        SaveInventory(inventory);
        Console.WriteLine("Item removed successfully.");
    }
    
    private void UpdateItem()
    {
        Console.Write("Enter item ID to update: ");
        string id = Console.ReadLine();
        
        var inventory = LoadInventory();
        var itemToUpdate = inventory.Find(item => item.Id == id);
        
        if (itemToUpdate == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }
        
        Console.Write("Enter new quantity (leave blank to keep current): ");
        string quantityInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(quantityInput))
        {
            if (int.TryParse(quantityInput, out int newQuantity))
            {
                itemToUpdate.Quantity = newQuantity;
            }
            else
            {
                Console.WriteLine("Invalid quantity. Keeping current value.");
            }
        }
        
        Console.Write("Enter new price (leave blank to keep current): ");
        string priceInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(priceInput))
        {
            if (decimal.TryParse(priceInput, out decimal newPrice))
            {
                itemToUpdate.Price = newPrice;
            }
            else
            {
                Console.WriteLine("Invalid price. Keeping current value.");
            }
        }
        
        SaveInventory(inventory);
        Console.WriteLine("Item updated successfully.");
    }
    
    private void ViewInventory()
    {
        var inventory = LoadInventory();
        
        if (inventory.Count == 0)
        {
            Console.WriteLine("Inventory is empty.");
            return;
        }
        
        Console.WriteLine("\nCurrent Inventory:");
        Console.WriteLine("ID\tName\tQuantity\tPrice");
        
        foreach (var item in inventory)
        {
            Console.WriteLine(string.Format("{0}\t{1}\t{2}\t{3:C}", 
                item.Id, item.Name, item.Quantity, item.Price));
        }
    }
}

public class InventoryItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}