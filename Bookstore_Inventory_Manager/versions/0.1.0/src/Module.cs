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

    public bool Main(string dataFolder)
    {
        _inventoryFilePath = Path.Combine(dataFolder, "inventory.json");
        _salesFilePath = Path.Combine(dataFolder, "sales.json");
        _restockAlertsFilePath = Path.Combine(dataFolder, "restockAlerts.json");

        InitializeFiles();

        Console.WriteLine("Bookstore Inventory Module is running.");
        Console.WriteLine("Managing inventory, tracking sales, and monitoring restock alerts.");

        // Example operations
        AddBook(new Book { Id = 1, Title = "Sample Book", Author = "Author Name", Price = 19.99, Quantity = 10 });
        RecordSale(1, 2);
        CheckRestockAlerts();

        return true;
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
        return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
    }

    private void SaveInventory(List<Book> inventory)
    {
        string json = JsonSerializer.Serialize(inventory);
        File.WriteAllText(_inventoryFilePath, json);
    }

    private List<Sale> LoadSales()
    {
        string json = File.ReadAllText(_salesFilePath);
        return JsonSerializer.Deserialize<List<Sale>>(json) ?? new List<Sale>();
    }

    private void SaveSales(List<Sale> sales)
    {
        string json = JsonSerializer.Serialize(sales);
        File.WriteAllText(_salesFilePath, json);
    }

    private List<RestockAlert> LoadRestockAlerts()
    {
        string json = File.ReadAllText(_restockAlertsFilePath);
        return JsonSerializer.Deserialize<List<RestockAlert>>(json) ?? new List<RestockAlert>();
    }

    private void SaveRestockAlerts(List<RestockAlert> alerts)
    {
        string json = JsonSerializer.Serialize(alerts);
        File.WriteAllText(_restockAlertsFilePath, json);
    }

    public void AddBook(Book book)
    {
        var inventory = LoadInventory();
        inventory.Add(book);
        SaveInventory(inventory);
        Console.WriteLine("Book added to inventory: " + book.Title);
    }

    public void RecordSale(int bookId, int quantitySold)
    {
        var inventory = LoadInventory();
        var book = inventory.Find(b => b.Id == bookId);

        if (book != null && book.Quantity >= quantitySold)
        {
            book.Quantity -= quantitySold;
            SaveInventory(inventory);

            var sales = LoadSales();
            sales.Add(new Sale { BookId = bookId, Quantity = quantitySold, Date = DateTime.Now });
            SaveSales(sales);

            Console.WriteLine("Sale recorded for book ID: " + bookId);

            if (book.Quantity <= 5)
            {
                GenerateRestockAlert(bookId);
            }
        }
        else
        {
            Console.WriteLine("Unable to record sale. Book not found or insufficient quantity.");
        }
    }

    private void GenerateRestockAlert(int bookId)
    {
        var inventory = LoadInventory();
        var book = inventory.Find(b => b.Id == bookId);

        if (book != null)
        {
            var alerts = LoadRestockAlerts();
            alerts.Add(new RestockAlert { BookId = bookId, Title = book.Title, CurrentQuantity = book.Quantity, DateGenerated = DateTime.Now });
            SaveRestockAlerts(alerts);
            Console.WriteLine("Restock alert generated for book: " + book.Title);
        }
    }

    public void CheckRestockAlerts()
    {
        var alerts = LoadRestockAlerts();
        if (alerts.Count > 0)
        {
            Console.WriteLine("Active Restock Alerts:");
            foreach (var alert in alerts)
            {
                Console.WriteLine("Book: " + alert.Title + " - Current Quantity: " + alert.CurrentQuantity);
            }
        }
        else
        {
            Console.WriteLine("No active restock alerts.");
        }
    }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}

public class Sale
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
}

public class RestockAlert
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public int CurrentQuantity { get; set; }
    public DateTime DateGenerated { get; set; }
}