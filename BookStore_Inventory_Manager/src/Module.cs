using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookStoreInventoryModule : IGeneratedModule
{
    public string Name { get; set; } = "BookStore Inventory Manager";

    private string _inventoryFilePath;
    private string _salesFilePath;
    private string _restockAlertsFilePath;

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Initializing BookStore Inventory Module...");

            _inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
            _salesFilePath = Path.Combine(dataFolder, "sales.json");
            _restockAlertsFilePath = Path.Combine(dataFolder, "restockAlerts.json");

            InitializeFiles();

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nBookStore Inventory Menu:");
                Console.WriteLine("1. View Inventory");
                Console.WriteLine("2. Add Book to Inventory");
                Console.WriteLine("3. Record Sale");
                Console.WriteLine("4. View Restock Alerts");
                Console.WriteLine("5. Exit Module");
                Console.Write("Select an option: ");

                string input = Console.ReadLine();
                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            ViewInventory();
                            break;
                        case 2:
                            AddBookToInventory();
                            break;
                        case 3:
                            RecordSale();
                            break;
                        case 4:
                            ViewRestockAlerts();
                            break;
                        case 5:
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }

            Console.WriteLine("BookStore Inventory Module completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void InitializeFiles()
    {
        if (!Directory.Exists(Path.GetDirectoryName(_inventoryFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_inventoryFilePath));
        }

        if (!File.Exists(_inventoryFilePath))
        {
            File.WriteAllText(_inventoryFilePath, "[]");
        }

        if (!File.Exists(_salesFilePath))
        {
            File.WriteAllText(_salesFilePath, "[]");
        }

        if (!File.Exists(_restockAlertsFilePath))
        {
            File.WriteAllText(_restockAlertsFilePath, "[]");
        }
    }

    private List<Book> LoadInventory()
    {
        string json = File.ReadAllText(_inventoryFilePath);
        return JsonSerializer.Deserialize<List<Book>>(json);
    }

    private void SaveInventory(List<Book> inventory)
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

    private List<RestockAlert> LoadRestockAlerts()
    {
        string json = File.ReadAllText(_restockAlertsFilePath);
        return JsonSerializer.Deserialize<List<RestockAlert>>(json);
    }

    private void SaveRestockAlerts(List<RestockAlert> alerts)
    {
        string json = JsonSerializer.Serialize(alerts);
        File.WriteAllText(_restockAlertsFilePath, json);
    }

    private void ViewInventory()
    {
        var inventory = LoadInventory();
        Console.WriteLine("\nCurrent Inventory:");
        Console.WriteLine("ID\tTitle\t\tAuthor\t\tPrice\tQuantity");
        foreach (var book in inventory)
        {
            Console.WriteLine(book.Id + "\t" + book.Title + "\t" + book.Author + "\t" + book.Price + "\t" + book.Quantity);
        }
    }

    private void AddBookToInventory()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        Console.Write("Enter book author: ");
        string author = Console.ReadLine();
        Console.Write("Enter book price: ");
        decimal price = decimal.Parse(Console.ReadLine());
        Console.Write("Enter initial quantity: ");
        int quantity = int.Parse(Console.ReadLine());

        var inventory = LoadInventory();
        int newId = inventory.Count > 0 ? inventory.Max(b => b.Id) + 1 : 1;

        inventory.Add(new Book
        {
            Id = newId,
            Title = title,
            Author = author,
            Price = price,
            Quantity = quantity
        });

        SaveInventory(inventory);
        Console.WriteLine("Book added to inventory successfully.");
    }

    private void RecordSale()
    {
        ViewInventory();
        Console.Write("\nEnter book ID to sell: ");
        int bookId = int.Parse(Console.ReadLine());
        Console.Write("Enter quantity sold: ");
        int quantity = int.Parse(Console.ReadLine());

        var inventory = LoadInventory();
        var book = inventory.FirstOrDefault(b => b.Id == bookId);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }

        if (book.Quantity < quantity)
        {
            Console.WriteLine("Not enough stock available.");
            return;
        }

        book.Quantity -= quantity;
        SaveInventory(inventory);

        var sales = LoadSales();
        sales.Add(new Sale
        {
            BookId = bookId,
            Quantity = quantity,
            SaleDate = DateTime.Now,
            TotalAmount = book.Price * quantity
        });
        SaveSales(sales);

        // Check for restock alert
        if (book.Quantity < 5)
        {
            var alerts = LoadRestockAlerts();
            if (!alerts.Any(a => a.BookId == bookId))
            {
                alerts.Add(new RestockAlert
                {
                    BookId = bookId,
                    BookTitle = book.Title,
                    CurrentQuantity = book.Quantity,
                    AlertDate = DateTime.Now
                });
                SaveRestockAlerts(alerts);
            }
        }

        Console.WriteLine("Sale recorded successfully.");
    }

    private void ViewRestockAlerts()
    {
        var alerts = LoadRestockAlerts();
        Console.WriteLine("\nRestock Alerts:");
        Console.WriteLine("Book ID\tTitle\t\tCurrent Quantity\tAlert Date");
        foreach (var alert in alerts)
        {
            Console.WriteLine(alert.BookId + "\t\t" + alert.BookTitle + "\t\t" + alert.CurrentQuantity + "\t\t" + alert.AlertDate);
        }
    }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Sale
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class RestockAlert
{
    public int BookId { get; set; }
    public string BookTitle { get; set; }
    public int CurrentQuantity { get; set; }
    public DateTime AlertDate { get; set; }
}