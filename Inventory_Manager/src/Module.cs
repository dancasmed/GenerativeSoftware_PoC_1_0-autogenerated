using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class InventoryManager : IGeneratedModule
{
    public string Name { get; set; } = "Inventory Manager";

    private string _inventoryFilePath;
    private string _salesFilePath;

    public InventoryManager()
    {
    }

    public bool Main(string dataFolder)
    {
        _inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
        _salesFilePath = Path.Combine(dataFolder, "sales.json");

        Console.WriteLine("Inventory Manager module is running.");
        Console.WriteLine("Data will be stored in: " + dataFolder);

        InitializeFiles();

        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    UpdateStock();
                    break;
                case "3":
                    RecordSale();
                    break;
                case "4":
                    ViewInventory();
                    break;
                case "5":
                    CheckReorderPoints();
                    break;
                case "6":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

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

    private void DisplayMenu()
    {
        Console.WriteLine("\nInventory Management System");
        Console.WriteLine("1. Add Product");
        Console.WriteLine("2. Update Stock");
        Console.WriteLine("3. Record Sale");
        Console.WriteLine("4. View Inventory");
        Console.WriteLine("5. Check Reorder Points");
        Console.WriteLine("6. Exit");
        Console.Write("Select an option: ");
    }

    private List<Product> LoadInventory()
    {
        string json = File.ReadAllText(_inventoryFilePath);
        return JsonSerializer.Deserialize<List<Product>>(json);
    }

    private void SaveInventory(List<Product> inventory)
    {
        string json = JsonSerializer.Serialize(inventory);
        File.WriteAllText(_inventoryFilePath, json);
    }

    private List<Sale> LoadSales()
    {
        string json = File.ReadAllText(_salesFilePath);
        return JsonSerializer.Deserialize<List<Sale>>(json);
    }

    private void SaveSales(List<Sale> sales)
    {
        string json = JsonSerializer.Serialize(sales);
        File.WriteAllText(_salesFilePath, json);
    }

    private void AddProduct()
    {
        Console.Write("Enter product ID: ");
        string id = Console.ReadLine();

        Console.Write("Enter product name: ");
        string name = Console.ReadLine();

        Console.Write("Enter initial stock quantity: ");
        int quantity = int.Parse(Console.ReadLine());

        Console.Write("Enter price: ");
        decimal price = decimal.Parse(Console.ReadLine());

        Console.Write("Enter reorder point: ");
        int reorderPoint = int.Parse(Console.ReadLine());

        var product = new Product
        {
            Id = id,
            Name = name,
            Quantity = quantity,
            Price = price,
            ReorderPoint = reorderPoint
        };

        var inventory = LoadInventory();
        inventory.Add(product);
        SaveInventory(inventory);

        Console.WriteLine("Product added successfully.");
    }

    private void UpdateStock()
    {
        Console.Write("Enter product ID: ");
        string id = Console.ReadLine();

        var inventory = LoadInventory();
        var product = inventory.Find(p => p.Id == id);

        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.Write("Enter quantity to add (use negative to subtract): ");
        int quantity = int.Parse(Console.ReadLine());

        product.Quantity += quantity;
        SaveInventory(inventory);

        Console.WriteLine("Stock updated successfully.");
    }

    private void RecordSale()
    {
        Console.Write("Enter product ID: ");
        string id = Console.ReadLine();

        var inventory = LoadInventory();
        var product = inventory.Find(p => p.Id == id);

        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.Write("Enter quantity sold: ");
        int quantity = int.Parse(Console.ReadLine());

        if (quantity > product.Quantity)
        {
            Console.WriteLine("Not enough stock available.");
            return;
        }

        product.Quantity -= quantity;
        SaveInventory(inventory);

        var sale = new Sale
        {
            ProductId = id,
            Quantity = quantity,
            SaleDate = DateTime.Now,
            TotalPrice = quantity * product.Price
        };

        var sales = LoadSales();
        sales.Add(sale);
        SaveSales(sales);

        Console.WriteLine("Sale recorded successfully.");
    }

    private void ViewInventory()
    {
        var inventory = LoadInventory();

        Console.WriteLine("\nCurrent Inventory:");
        Console.WriteLine("ID\tName\tQuantity\tPrice\tReorder Point");

        foreach (var product in inventory)
        {
            Console.WriteLine(product.Id + "\t" + product.Name + "\t" + product.Quantity + "\t" + product.Price + "\t" + product.ReorderPoint);
        }
    }

    private void CheckReorderPoints()
    {
        var inventory = LoadInventory();
        bool needsReorder = false;

        Console.WriteLine("\nProducts below reorder point:");
        Console.WriteLine("ID\tName\tCurrent Stock\tReorder Point");

        foreach (var product in inventory)
        {
            if (product.Quantity <= product.ReorderPoint)
            {
                needsReorder = true;
                Console.WriteLine(product.Id + "\t" + product.Name + "\t" + product.Quantity + "\t" + product.ReorderPoint);
            }
        }

        if (!needsReorder)
        {
            Console.WriteLine("All products are above their reorder points.");
        }
    }
}

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int ReorderPoint { get; set; }
}

public class Sale
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalPrice { get; set; }
}