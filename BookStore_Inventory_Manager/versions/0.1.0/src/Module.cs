using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookStoreInventory : IGeneratedModule
{
    public string Name { get; set; } = "BookStore Inventory Manager";
    
    private string _dataFolder;
    private string _inventoryFilePath;
    private string _salesFilePath;
    
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int RestockThreshold { get; set; } = 5;
    }
    
    public class SaleRecord
    {
        public string BookId { get; set; }
        public DateTime SaleDate { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
    
    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        _inventoryFilePath = Path.Combine(_dataFolder, "inventory.json");
        _salesFilePath = Path.Combine(_dataFolder, "sales.json");
        
        Console.WriteLine("BookStore Inventory Module is running.");
        
        InitializeDataFiles();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddNewBook();
                    break;
                case "2":
                    ListAllBooks();
                    break;
                case "3":
                    RecordSale();
                    break;
                case "4":
                    CheckRestockAlerts();
                    break;
                case "5":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void InitializeDataFiles()
    {
        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }
        
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
        Console.WriteLine("\nBookStore Inventory Menu:");
        Console.WriteLine("1. Add a new book");
        Console.WriteLine("2. List all books");
        Console.WriteLine("3. Record a sale");
        Console.WriteLine("4. Check restock alerts");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddNewBook()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter author: ");
        string author = Console.ReadLine();
        
        Console.Write("Enter price: ");
        decimal price = decimal.Parse(Console.ReadLine());
        
        Console.Write("Enter initial stock: ");
        int stock = int.Parse(Console.ReadLine());
        
        Console.Write("Enter restock threshold (default 5): ");
        string thresholdInput = Console.ReadLine();
        int threshold = string.IsNullOrEmpty(thresholdInput) ? 5 : int.Parse(thresholdInput);
        
        var newBook = new Book
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Author = author,
            Price = price,
            Stock = stock,
            RestockThreshold = threshold
        };
        
        var books = GetInventory();
        books.Add(newBook);
        SaveInventory(books);
        
        Console.WriteLine("Book added successfully.");
    }
    
    private void ListAllBooks()
    {
        var books = GetInventory();
        
        if (books.Count == 0)
        {
            Console.WriteLine("No books in inventory.");
            return;
        }
        
        Console.WriteLine("\nCurrent Inventory:");
        foreach (var book in books)
        {
            Console.WriteLine($"ID: {book.Id}");
            Console.WriteLine($"Title: {book.Title}");
            Console.WriteLine($"Author: {book.Author}");
            Console.WriteLine($"Price: {book.Price:C}");
            Console.WriteLine($"Stock: {book.Stock}");
            Console.WriteLine($"Restock Threshold: {book.RestockThreshold}");
            Console.WriteLine("-----------------------");
        }
    }
    
    private void RecordSale()
    {
        ListAllBooks();
        
        Console.Write("Enter book ID to sell: ");
        string bookId = Console.ReadLine();
        
        var books = GetInventory();
        var book = books.Find(b => b.Id == bookId);
        
        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }
        
        Console.Write("Enter quantity sold: ");
        int quantity = int.Parse(Console.ReadLine());
        
        if (quantity > book.Stock)
        {
            Console.WriteLine("Not enough stock available.");
            return;
        }
        
        book.Stock -= quantity;
        SaveInventory(books);
        
        var sale = new SaleRecord
        {
            BookId = bookId,
            SaleDate = DateTime.Now,
            Quantity = quantity,
            TotalAmount = quantity * book.Price
        };
        
        var sales = GetSales();
        sales.Add(sale);
        SaveSales(sales);
        
        Console.WriteLine("Sale recorded successfully.");
    }
    
    private void CheckRestockAlerts()
    {
        var books = GetInventory();
        var needsRestock = books.FindAll(b => b.Stock <= b.RestockThreshold);
        
        if (needsRestock.Count == 0)
        {
            Console.WriteLine("No books need restocking.");
            return;
        }
        
        Console.WriteLine("\nBooks that need restocking:");
        foreach (var book in needsRestock)
        {
            Console.WriteLine($"{book.Title} by {book.Author} - Current stock: {book.Stock} (Threshold: {book.RestockThreshold})");
        }
    }
    
    private List<Book> GetInventory()
    {
        string json = File.ReadAllText(_inventoryFilePath);
        return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
    }
    
    private void SaveInventory(List<Book> books)
    {
        string json = JsonSerializer.Serialize(books);
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