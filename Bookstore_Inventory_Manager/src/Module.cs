using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class BookInventoryManager : IGeneratedModule
{
    public string Name { get; set; } = "Book Inventory Manager";
    
    private string _booksPath;
    private string _salesPath;
    private string _alertsPath;
    private string _categoriesPath;
    private string _suppliersPath;
    
    private List<Book> _books = new();
    private List<Sale> _sales = new();
    private List<RestockAlert> _alerts = new();
    private List<Category> _categories = new();
    private List<Supplier> _suppliers = new();

    public bool Main(string dataFolder)
    {
        try
        {
            InitializeDataPaths(dataFolder);
            LoadInitialData();
            ShowMainMenu();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Module error: " + ex.Message);
            return false;
        }
    }

    private void InitializeDataPaths(string rootPath)
    {
        Directory.CreateDirectory(rootPath);
        _booksPath = Path.Combine(rootPath, "books.json");
        _salesPath = Path.Combine(rootPath, "sales.json");
        _alertsPath = Path.Combine(rootPath, "alerts.json");
        _categoriesPath = Path.Combine(rootPath, "categories.json");
        _suppliersPath = Path.Combine(rootPath, "suppliers.json");
    }

    private void LoadInitialData()
    {
        _books = LoadData<Book>(_booksPath);
        _sales = LoadData<Sale>(_salesPath);
        _alerts = LoadData<RestockAlert>(_alertsPath);
        _categories = LoadData<Category>(_categoriesPath);
        _suppliers = LoadData<Supplier>(_suppliersPath);
    }

    private List<T> LoadData<T>(string path) where T : new()
    {
        if (!File.Exists(path)) return new List<T>();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    private void SaveData<T>(string path, List<T> data)
    {
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
    }

    private void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Bookstore Inventory Management System");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Update Book");
            Console.WriteLine("3. Delete Book");
            Console.WriteLine("4. View Books");
            Console.WriteLine("5. Search Books");
            Console.WriteLine("6. Record Sale");
            Console.WriteLine("7. View Sales");
            Console.WriteLine("8. Restock Alerts");
            Console.WriteLine("9. Generate Reports");
            Console.WriteLine("10. Adjust Thresholds");
            Console.WriteLine("11. Exit");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": AddBook(); break;
                case "2": UpdateBook(); break;
                case "3": DeleteBook(); break;
                case "4": ViewBooks(); break;
                case "5": SearchBooks(); break;
                case "6": RecordSale(); break;
                case "7": ViewSales(); break;
                case "8": ShowRestockAlerts(); break;
                case "9": GenerateReports(); break;
                case "10": AdjustThresholds(); break;
                case "11": SaveAllData(); return;
                default: Console.WriteLine("Invalid option"); break;
            }
        }
    }

    private void AddBook()
    {
        var book = new Book();
        Console.Write("Enter ISBN: ");
        book.ISBN = Console.ReadLine();
        Console.Write("Enter Title: ");
        book.Title = Console.ReadLine();
        Console.Write("Enter Author: ");
        book.Author = Console.ReadLine();
        Console.Write("Enter Price: ");
        book.Price = decimal.Parse(Console.ReadLine());
        Console.Write("Initial Stock: ");
        book.CurrentStock = int.Parse(Console.ReadLine());
        Console.Write("Minimum Stock Threshold: ");
        book.MinimumStockThreshold = int.Parse(Console.ReadLine());
        
        book.Id = Guid.NewGuid();
        book.DateAdded = DateTime.Now;
        book.LastUpdated = DateTime.Now;
        
        _books.Add(book);
        SaveData(_booksPath, _books);
        Console.WriteLine("Book added successfully");
    }

    private void UpdateBook()
    {
        Console.Write("Enter ISBN to update: ");
        var isbn = Console.ReadLine();
        var book = _books.Find(b => b.ISBN == isbn);
        
        if (book == null)
        {
            Console.WriteLine("Book not found");
            return;
        }
        
        Console.Write("New Price (current: " + book.Price + "): ");
        var priceInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(priceInput))
            book.Price = decimal.Parse(priceInput);
        
        Console.Write("New Stock Quantity (current: " + book.CurrentStock + "): ");
        var stockInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(stockInput))
            book.CurrentStock = int.Parse(stockInput);
        
        book.LastUpdated = DateTime.Now;
        SaveData(_booksPath, _books);
        Console.WriteLine("Book updated successfully");
    }

    private void DeleteBook()
    {
        Console.Write("Enter ISBN to delete: ");
        var isbn = Console.ReadLine();
        var book = _books.Find(b => b.ISBN == isbn);
        
        if (book != null)
        {
            _books.Remove(book);
            SaveData(_booksPath, _books);
            Console.WriteLine("Book removed successfully");
        }
        else
        {
            Console.WriteLine("Book not found");
        }
    }

    private void ViewBooks()
    {
        Console.WriteLine("List of All Books:");
        foreach (var book in _books)
        {
            Console.WriteLine($"ISBN: {book.ISBN}, Title: {book.Title}, Author: {book.Author}, Price: {book.Price}, Stock: {book.CurrentStock}, Threshold: {book.MinimumStockThreshold}");
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void SearchBooks()
    {
        Console.Write("Enter search term (ISBN, Title, or Author): ");
        var searchTerm = Console.ReadLine()?.ToLower();

        var results = _books.Where(b =>
            b.ISBN.ToLower().Contains(searchTerm) ||
            b.Title.ToLower().Contains(searchTerm) ||
            b.Author.ToLower().Contains(searchTerm)
        ).ToList();

        Console.WriteLine($"Found {results.Count} matches:");
        foreach (var book in results)
        {
            Console.WriteLine($"ISBN: {book.ISBN}, Title: {book.Title}, Author: {book.Author}, Price: {book.Price}, Stock: {book.CurrentStock}");
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void RecordSale()
    {
        Console.Write("Enter Book ISBN: ");
        var isbn = Console.ReadLine();
        var book = _books.Find(b => b.ISBN == isbn);
        
        if (book == null || book.CurrentStock <= 0)
        {
            Console.WriteLine("Book not available");
            return;
        }
        
        Console.Write("Quantity sold: ");
        var qty = int.Parse(Console.ReadLine());
        
        if (qty > book.CurrentStock)
        {
            Console.WriteLine("Insufficient stock");
            return;
        }
        
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            BookId = book.Id,
            Quantity = qty,
            UnitPrice = book.Price,
            TotalPrice = qty * book.Price,
            SaleDate = DateTime.Now
        };
        
        book.CurrentStock -= qty;
        _sales.Add(sale);
        SaveData(_salesPath, _sales);
        SaveData(_booksPath, _books);
        CheckRestockAlerts(book);
        Console.WriteLine("Sale recorded successfully");
    }

    private void CheckRestockAlerts(Book book)
    {
        if (book.CurrentStock >= book.MinimumStockThreshold) return;
        
        var existingAlert = _alerts.Find(a => 
            a.BookId == book.Id && 
            a.Status == "new");
        
        if (existingAlert != null) return;
        
        _alerts.Add(new RestockAlert
        {
            Id = Guid.NewGuid(),
            BookId = book.Id,
            CurrentStock = book.CurrentStock,
            Threshold = book.MinimumStockThreshold,
            AlertDate = DateTime.Now,
            Status = "new"
        });
        
        SaveData(_alertsPath, _alerts);
    }

    private void ShowRestockAlerts()
    {
        Console.WriteLine("Current Restock Alerts:");
        foreach (var alert in _alerts)
        {
            var book = _books.Find(b => b.Id == alert.BookId);
            Console.WriteLine("Book: " + book?.Title + " | Current Stock: " + 
                            alert.CurrentStock + " | Threshold: " + alert.Threshold);
        }
        Console.ReadKey();
    }

    private void GenerateReports()
    {
        Console.WriteLine("Inventory Status Report");
        Console.WriteLine("Total Books: " + _books.Count);
        Console.WriteLine("Low Stock Items: " + _alerts.Count);
        Console.WriteLine("Total Sales: " + _sales.Count);
        Console.ReadKey();
    }

    private void AdjustThresholds()
    {
        Console.Write("Enter ISBN: ");
        var isbn = Console.ReadLine();
        var book = _books.Find(b => b.ISBN == isbn);
        
        if (book == null)
        {
            Console.WriteLine("Book not found");
            return;
        }
        
        Console.Write("New Minimum Threshold (current: " + book.MinimumStockThreshold + "): ");
        book.MinimumStockThreshold = int.Parse(Console.ReadLine());
        book.LastUpdated = DateTime.Now;
        SaveData(_booksPath, _books);
        CheckRestockAlerts(book);
        Console.WriteLine("Threshold updated successfully");
    }

    private void ViewSales()
    {
        Console.WriteLine("Sales History:");
        foreach (var sale in _sales)
        {
            var book = _books.Find(b => b.Id == sale.BookId);
            Console.WriteLine($"Date: {sale.SaleDate}, Book: {book?.Title}, Quantity: {sale.Quantity}, Total Price: {sale.TotalPrice}");
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void SaveAllData()
    {
        SaveData(_booksPath, _books);
        SaveData(_salesPath, _sales);
        SaveData(_alertsPath, _alerts);
        SaveData(_categoriesPath, _categories);
        SaveData(_suppliersPath, _suppliers);
    }
}

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public decimal Price { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStockThreshold { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class Sale
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime SaleDate { get; set; }
}

public class RestockAlert
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public int CurrentStock { get; set; }
    public int Threshold { get; set; }
    public DateTime AlertDate { get; set; }
    public string Status { get; set; }
}

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ContactInfo { get; set; }
    public int LeadTime { get; set; }
    public bool PreferredStatus { get; set; }
}