using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MenuItem
{
    public string Name { get; set; }
    public double Price { get; set; }
    public string Category { get; set; }
}

public class MenuManager : IGeneratedModule
{
    public string Name { get; set; } = "Restaurant Menu Manager";
    private List<MenuItem> _menuItems;
    private string _menuFilePath;

    public MenuManager()
    {
        _menuItems = new List<MenuItem>();
        _menuFilePath = string.Empty;
    }

    public bool Main(string dataFolder)
    {
        _menuFilePath = Path.Combine(dataFolder, "menu_items.json");
        Console.WriteLine("Restaurant Menu Manager is running.");
        LoadMenuItems();

        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddMenuItem();
                    break;
                case "2":
                    ViewMenuItems();
                    break;
                case "3":
                    UpdateMenuItem();
                    break;
                case "4":
                    DeleteMenuItem();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveMenuItems();
        Console.WriteLine("Menu items saved successfully.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nRestaurant Menu Manager");
        Console.WriteLine("1. Add Menu Item");
        Console.WriteLine("2. View Menu Items");
        Console.WriteLine("3. Update Menu Item");
        Console.WriteLine("4. Delete Menu Item");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddMenuItem()
    {
        Console.Write("Enter item name: ");
        string name = Console.ReadLine();

        Console.Write("Enter item price: ");
        if (!double.TryParse(Console.ReadLine(), out double price))
        {
            Console.WriteLine("Invalid price. Please enter a valid number.");
            return;
        }

        Console.Write("Enter item category: ");
        string category = Console.ReadLine();

        _menuItems.Add(new MenuItem { Name = name, Price = price, Category = category });
        Console.WriteLine("Menu item added successfully.");
    }

    private void ViewMenuItems()
    {
        if (_menuItems.Count == 0)
        {
            Console.WriteLine("No menu items available.");
            return;
        }

        Console.WriteLine("\nMenu Items:");
        foreach (var item in _menuItems)
        {
            Console.WriteLine($"Name: {item.Name}, Price: {item.Price}, Category: {item.Category}");
        }
    }

    private void UpdateMenuItem()
    {
        ViewMenuItems();
        if (_menuItems.Count == 0)
            return;

        Console.Write("Enter the name of the item to update: ");
        string name = Console.ReadLine();

        var item = _menuItems.Find(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }

        Console.Write("Enter new name (leave blank to keep current): ");
        string newName = Console.ReadLine();
        if (!string.IsNullOrEmpty(newName))
            item.Name = newName;

        Console.Write("Enter new price (leave blank to keep current): ");
        string priceInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(priceInput) && double.TryParse(priceInput, out double newPrice))
            item.Price = newPrice;

        Console.Write("Enter new category (leave blank to keep current): ");
        string newCategory = Console.ReadLine();
        if (!string.IsNullOrEmpty(newCategory))
            item.Category = newCategory;

        Console.WriteLine("Menu item updated successfully.");
    }

    private void DeleteMenuItem()
    {
        ViewMenuItems();
        if (_menuItems.Count == 0)
            return;

        Console.Write("Enter the name of the item to delete: ");
        string name = Console.ReadLine();

        var item = _menuItems.Find(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            Console.WriteLine("Item not found.");
            return;
        }

        _menuItems.Remove(item);
        Console.WriteLine("Menu item deleted successfully.");
    }

    private void LoadMenuItems()
    {
        if (File.Exists(_menuFilePath))
        {
            string json = File.ReadAllText(_menuFilePath);
            _menuItems = JsonSerializer.Deserialize<List<MenuItem>>(json) ?? new List<MenuItem>();
        }
    }

    private void SaveMenuItems()
    {
        string json = JsonSerializer.Serialize(_menuItems);
        File.WriteAllText(_menuFilePath, json);
    }
}