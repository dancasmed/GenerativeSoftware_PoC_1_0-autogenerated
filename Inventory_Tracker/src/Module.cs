using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class InventoryTracker : IGeneratedModule
{
    public string Name { get; set; } = "Inventory Tracker";

    private string inventoryFilePath;
    
    public InventoryTracker()
    {
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Inventory Tracker module...");
        
        inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        if (!File.Exists(inventoryFilePath))
        {
            File.WriteAllText(inventoryFilePath, "[]");
        }

        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddItem();
                    break;
                case "2":
                    UpdateItem();
                    break;
                case "3":
                    ViewInventory();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Inventory Tracker module completed.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nInventory Tracker Menu:");
        Console.WriteLine("1. Add new item");
        Console.WriteLine("2. Update item quantity");
        Console.WriteLine("3. View inventory");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private List<InventoryItem> LoadInventory()
    {
        string json = File.ReadAllText(inventoryFilePath);
        return JsonSerializer.Deserialize<List<InventoryItem>>(json) ?? new List<InventoryItem>();
    }

    private void SaveInventory(List<InventoryItem> inventory)
    {
        string json = JsonSerializer.Serialize(inventory, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(inventoryFilePath, json);
    }

    private void AddItem()
    {
        Console.Write("Enter item name: ");
        string name = Console.ReadLine();

        Console.Write("Enter item price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price format.");
            return;
        }


        Console.Write("Enter initial quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity format.");
            return;
        }

        var inventory = LoadInventory();
        inventory.Add(new InventoryItem { Name = name, Price = price, Quantity = quantity });
        SaveInventory(inventory);

        Console.WriteLine("Item added successfully.");
    }

    private void UpdateItem()
    {
        var inventory = LoadInventory();
        if (inventory.Count == 0)
        {
            Console.WriteLine("Inventory is empty.");
            return;
        }

        ViewInventory();
        Console.Write("Enter item number to update: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > inventory.Count)
        {
            Console.WriteLine("Invalid item number.");
            return;
        }

        Console.Write("Enter new quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int newQuantity))
        {
            Console.WriteLine("Invalid quantity format.");
            return;
        }

        inventory[index - 1].Quantity = newQuantity;
        SaveInventory(inventory);
        Console.WriteLine("Quantity updated successfully.");
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
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("#  | Name                | Price    | Quantity");
        Console.WriteLine("-------------------------------------------------");

        for (int i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            Console.WriteLine(string.Format("{0,-3}| {1,-20}| {2,-9}| {3}", 
                i + 1, 
                item.Name, 
                item.Price.ToString("C"), 
                item.Quantity));
        }

        Console.WriteLine("-------------------------------------------------");
    }
}

public class InventoryItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}