using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PetStoreManager : IGeneratedModule
{
    public string Name { get; set; } = "Pet Store Inventory and Sales Manager";

    private string inventoryFilePath;
    private string salesFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Pet Store Manager...");

        inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
        salesFilePath = Path.Combine(dataFolder, "sales.json");

        EnsureDataFilesExist();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddNewPet();
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

        Console.WriteLine("Pet Store Manager is shutting down...");
        return true;
    }

    private void EnsureDataFilesExist()
    {
        if (!File.Exists(inventoryFilePath))
        {
            File.WriteAllText(inventoryFilePath, "[]");
        }

        if (!File.Exists(salesFilePath))
        {
            File.WriteAllText(salesFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPet Store Manager");
        Console.WriteLine("1. Add new pet to inventory");
        Console.WriteLine("2. View inventory");
        Console.WriteLine("3. Record a sale");
        Console.WriteLine("4. View sales records");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddNewPet()
    {
        Console.Write("Enter pet name: ");
        string name = Console.ReadLine();

        Console.Write("Enter pet type (e.g., Dog, Cat, Bird): ");
        string type = Console.ReadLine();

        Console.Write("Enter pet price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price format.");
            return;
        }

        Console.Write("Enter quantity in stock: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity format.");
            return;
        }

        var inventory = LoadInventory();
        inventory.Add(new Pet
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Type = type,
            Price = price,
            Quantity = quantity
        });

        SaveInventory(inventory);
        Console.WriteLine("Pet added to inventory successfully.");
    }

    private void ViewInventory()
    {
        var inventory = LoadInventory();
        Console.WriteLine("\nCurrent Inventory:");
        Console.WriteLine("ID\t\tName\tType\tPrice\tQuantity");

        foreach (var pet in inventory)
        {
            Console.WriteLine($"{pet.Id}\t{pet.Name}\t{pet.Type}\t{pet.Price:C2}\t{pet.Quantity}");
        }
    }

    private void RecordSale()
    {
        ViewInventory();
        Console.Write("Enter ID of pet sold: ");
        string id = Console.ReadLine();

        var inventory = LoadInventory();
        var pet = inventory.Find(p => p.Id == id);

        if (pet == null)
        {
            Console.WriteLine("Pet not found in inventory.");
            return;
        }

        Console.Write("Enter quantity sold: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        if (quantity > pet.Quantity)
        {
            Console.WriteLine("Not enough stock available.");
            return;
        }

        Console.Write("Enter customer name: ");
        string customerName = Console.ReadLine();

        pet.Quantity -= quantity;
        SaveInventory(inventory);

        var sales = LoadSales();
        sales.Add(new Sale
        {
            Id = Guid.NewGuid().ToString(),
            PetId = pet.Id,
            PetName = pet.Name,
            Quantity = quantity,
            TotalPrice = pet.Price * quantity,
            CustomerName = customerName,
            SaleDate = DateTime.Now
        });

        SaveSales(sales);
        Console.WriteLine("Sale recorded successfully.");
    }

    private void ViewSales()
    {
        var sales = LoadSales();
        Console.WriteLine("\nSales Records:");
        Console.WriteLine("Date\t\t\tCustomer\tPet\tQty\tTotal");

        foreach (var sale in sales)
        {
            Console.WriteLine($"{sale.SaleDate}\t{sale.CustomerName}\t{sale.PetName}\t{sale.Quantity}\t{sale.TotalPrice:C2}");
        }
    }

    private List<Pet> LoadInventory()
    {
        string json = File.ReadAllText(inventoryFilePath);
        return JsonSerializer.Deserialize<List<Pet>>(json) ?? new List<Pet>();
    }

    private void SaveInventory(List<Pet> inventory)
    {
        string json = JsonSerializer.Serialize(inventory);
        File.WriteAllText(inventoryFilePath, json);
    }

    private List<Sale> LoadSales()
    {
        string json = File.ReadAllText(salesFilePath);
        return JsonSerializer.Deserialize<List<Sale>>(json) ?? new List<Sale>();
    }

    private void SaveSales(List<Sale> sales)
    {
        string json = JsonSerializer.Serialize(sales);
        File.WriteAllText(salesFilePath, json);
    }
}

public class Pet
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Sale
{
    public string Id { get; set; }
    public string PetId { get; set; }
    public string PetName { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
}