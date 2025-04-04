using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookstoreInventoryModule : IGeneratedModule
{
    public string Name { get; set; } = "Bookstore Inventory Manager";

    private string _inventoryFilePath;
    private string _salesFilePath;
    private string _restockAlertsFilePath;

    private List<Book> _inventory;
    private List<Sale> _sales;
    private List<RestockAlert> _restockAlerts;

    public BookstoreInventoryModule()
    {
        _inventory = new List<Book>();
        _sales = new List<Sale>();
        _restockAlerts = new List<RestockAlert>();
    }

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Initializing Bookstore Inventory Module...");

            _inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
            _salesFilePath = Path.Combine(dataFolder, "sales.json");
            _restockAlertsFilePath = Path.Combine(dataFolder, "restockAlerts.json");

            LoadData();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nBookstore Inventory Management");
                Console.WriteLine("1. View Inventory");
                Console.WriteLine("2. Add Book");
                Console.WriteLine("3. Record Sale");
                Console.WriteLine("4. View Sales");
                Console.WriteLine("5. Check Restock Alerts");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case 1:
                            ViewInventory();
                            break;
                        case 2:
                            AddBook();
                            break;
                        case 3:
                            RecordSale();
                            break;
                        case 4:
                            ViewSales();
                            break;
                        case 5:
                            CheckRestockAlerts();
                            break;
                        case 6:
                            exit = true;
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

            SaveData();
            Console.WriteLine("Bookstore Inventory Module completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(_inventoryFilePath))
            {
                string json = File.ReadAllText(_inventoryFilePath);
                _inventory = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }

            if (File.Exists(_salesFilePath))
            {
                string json = File.ReadAllText(_salesFilePath);
                _sales = JsonSerializer.Deserialize<List<Sale>>(json) ?? new List<Sale>();
            }

            if (File.Exists(_restockAlertsFilePath))
            {
                string json = File.ReadAllText(_restockAlertsFilePath);
                _restockAlerts = JsonSerializer.Deserialize<List<RestockAlert>>(json) ?? new List<RestockAlert>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data: " + ex.Message);
        }
    }

    private void SaveData()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_inventoryFilePath));

            string inventoryJson = JsonSerializer.Serialize(_inventory);
            File.WriteAllText(_inventoryFilePath, inventoryJson);

            string salesJson = JsonSerializer.Serialize(_sales);
            File.WriteAllText(_salesFilePath, salesJson);

            string restockAlertsJson = JsonSerializer.Serialize(_restockAlerts);
            File.WriteAllText(_restockAlertsFilePath, restockAlertsJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving data: " + ex.Message);
        }
    }

    private void ViewInventory()
    {
        Console.WriteLine("\nCurrent Inventory:");
        if (_inventory.Count == 0)
        {
            Console.WriteLine("No books in inventory.");
            return;
        }

        foreach (var book in _inventory)
        {
            Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Author: {book.Author}, Price: {book.Price:C}, Quantity: {book.Quantity}");
        }
    }

    private void AddBook()
    {
        Console.WriteLine("\nAdd New Book");
        Console.Write("Enter title: ");
        string title = Console.ReadLine();

        Console.Write("Enter author: ");
        string author = Console.ReadLine();

        Console.Write("Enter price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price. Operation cancelled.");
            return;
        }

        Console.Write("Enter quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Invalid quantity. Operation cancelled.");
            return;
        }

        var newBook = new Book
        {
            Id = _inventory.Count > 0 ? _inventory.Max(b => b.Id) + 1 : 1,
            Title = title,
            Author = author,
            Price = price,
            Quantity = quantity
        };

        _inventory.Add(newBook);
        Console.WriteLine("Book added successfully.");
    }

    private void RecordSale()
    {
        ViewInventory();
        if (_inventory.Count == 0) return;

        Console.Write("\nEnter book ID to sell: ");
        if (!int.TryParse(Console.ReadLine(), out int bookId))
        {
            Console.WriteLine("Invalid book ID.");
            return;
        }

        var book = _inventory.FirstOrDefault(b => b.Id == bookId);
        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }

        Console.Write("Enter quantity sold: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        if (book.Quantity < quantity)
        {
            Console.WriteLine("Not enough stock available.");
            return;
        }

        book.Quantity -= quantity;
        var sale = new Sale
        {
            Id = _sales.Count > 0 ? _sales.Max(s => s.Id) + 1 : 1,
            BookId = book.Id,
            BookTitle = book.Title,
            Quantity = quantity,
            TotalPrice = quantity * book.Price,
            SaleDate = DateTime.Now
        };

        _sales.Add(sale);
        Console.WriteLine("Sale recorded successfully.");

        // Check for restock alert
        if (book.Quantity <= 5)
        {
            var existingAlert = _restockAlerts.FirstOrDefault(a => a.BookId == book.Id);
            if (existingAlert == null)
            {
                _restockAlerts.Add(new RestockAlert
                {
                    Id = _restockAlerts.Count > 0 ? _restockAlerts.Max(a => a.Id) + 1 : 1,
                    BookId = book.Id,
                    BookTitle = book.Title,
                    CurrentQuantity = book.Quantity,
                    AlertDate = DateTime.Now
                });
                Console.WriteLine("Restock alert generated for this book.");
            }
        }
    }

    private void ViewSales()
    {
        Console.WriteLine("\nSales History:");
        if (_sales.Count == 0)
        {
            Console.WriteLine("No sales recorded.");
            return;
        }

        foreach (var sale in _sales)
        {
            Console.WriteLine($"ID: {sale.Id}, Book: {sale.BookTitle}, Quantity: {sale.Quantity}, Total: {sale.TotalPrice:C}, Date: {sale.SaleDate}");
        }
    }

    private void CheckRestockAlerts()
    {
        Console.WriteLine("\nRestock Alerts:");
        if (_restockAlerts.Count == 0)
        {
            Console.WriteLine("No restock alerts.");
            return;
        }

        foreach (var alert in _restockAlerts)
        {
            Console.WriteLine($"Book: {alert.BookTitle}, Current Quantity: {alert.CurrentQuantity}, Alert Date: {alert.AlertDate}");
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
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime SaleDate { get; set; }
}

public class RestockAlert
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; }
    public int CurrentQuantity { get; set; }
    public DateTime AlertDate { get; set; }
}