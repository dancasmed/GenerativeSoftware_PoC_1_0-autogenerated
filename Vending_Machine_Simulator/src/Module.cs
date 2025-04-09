using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VendingMachineModule : IGeneratedModule
{
    public string Name { get; set; } = "Vending Machine Simulator";

    private List<VendingItem> _items;
    private string _itemsFilePath;
    private const string ItemsFileName = "vending_items.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Vending Machine Simulator...");
        _itemsFilePath = Path.Combine(dataFolder, ItemsFileName);
        
        LoadItems();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    DisplayItems();
                    break;
                case "2":
                    SelectItem();
                    break;
                case "3":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Vending Machine Simulator shutting down...");
        return true;
    }

    private void LoadItems()
    {
        try
        {
            if (File.Exists(_itemsFilePath))
            {
                string json = File.ReadAllText(_itemsFilePath);
                _items = JsonSerializer.Deserialize<List<VendingItem>>(json);
            }
            else
            {
                InitializeDefaultItems();
                SaveItems();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading items: " + ex.Message);
            InitializeDefaultItems();
        }
    }

    private void SaveItems()
    {
        try
        {
            string json = JsonSerializer.Serialize(_items);
            File.WriteAllText(_itemsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving items: " + ex.Message);
        }
    }

    private void InitializeDefaultItems()
    {
        _items = new List<VendingItem>
        {
            new VendingItem { Id = 1, Name = "Soda", Price = 1.50m, Quantity = 10 },
            new VendingItem { Id = 2, Name = "Chips", Price = 1.00m, Quantity = 8 },
            new VendingItem { Id = 3, Name = "Candy", Price = 0.75m, Quantity = 15 },
            new VendingItem { Id = 4, Name = "Water", Price = 1.25m, Quantity = 12 }
        };
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nVending Machine Menu:");
        Console.WriteLine("1. Display Available Items");
        Console.WriteLine("2. Select Item");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
    }

    private void DisplayItems()
    {
        Console.WriteLine("\nAvailable Items:");
        Console.WriteLine("ID\tName\tPrice\tQuantity");
        foreach (var item in _items)
        {
            Console.WriteLine(item.Id + "\t" + item.Name + "\t" + item.Price + "\t" + item.Quantity);
        }
    }

    private void SelectItem()
    {
        DisplayItems();
        Console.Write("\nEnter the ID of the item you want to purchase: ");
        if (int.TryParse(Console.ReadLine(), out int itemId))
        {
            var selectedItem = _items.Find(item => item.Id == itemId);
            if (selectedItem != null)
            {
                if (selectedItem.Quantity > 0)
                {
                    Console.Write("Enter the amount of money inserted: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        if (amount >= selectedItem.Price)
                        {
                            selectedItem.Quantity--;
                            SaveItems();
                            decimal change = amount - selectedItem.Price;
                            Console.WriteLine("Dispensing " + selectedItem.Name);
                            if (change > 0)
                            {
                                Console.WriteLine("Your change is: " + change);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Insufficient funds. Please insert more money.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount entered.");
                    }
                }
                else
                {
                    Console.WriteLine("Item is out of stock.");
                }
            }
            else
            {
                Console.WriteLine("Item not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid item ID.");
        }
    }
}

public class VendingItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}