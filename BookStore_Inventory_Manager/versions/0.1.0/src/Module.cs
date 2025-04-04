using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookStoreInventory : IGeneratedModule
{
    public string Name { get; set; } = "BookStore Inventory Manager";
    
    private string _dataFolder;
    private List<Book> _inventory;
    private List<Sale> _sales;
    
    private const string InventoryFileName = "inventory.json";
    private const string SalesFileName = "sales.json";
    
    public BookStoreInventory()
    {
        _inventory = new List<Book>();
        _sales = new List<Sale>();
    }
    
    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        
        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }
        
        LoadData();
        
        Console.WriteLine("BookStore Inventory Manager is running");
        Console.WriteLine("Current inventory loaded with " + _inventory.Count + " books");
        
        // Simulate some operations
        CheckRestockAlerts();
        RecordSampleSale();
        
        SaveData();
        
        return true;
    }
    
    private void LoadData()
    {
        try
        {
            string inventoryPath = Path.Combine(_dataFolder, InventoryFileName);
            if (File.Exists(inventoryPath))
            {
                string json = File.ReadAllText(inventoryPath);
                _inventory = JsonSerializer.Deserialize<List<Book>>(json);
            }
            
            string salesPath = Path.Combine(_dataFolder, SalesFileName);
            if (File.Exists(salesPath))
            {
                string json = File.ReadAllText(salesPath);
                _sales = JsonSerializer.Deserialize<List<Sale>>(json);
            }
            
            // Initialize with sample data if empty
            if (_inventory.Count == 0)
            {
                _inventory.Add(new Book { Id = 1, Title = "C# Programming", Author = "John Doe", Price = 39.99m, Quantity = 10, RestockThreshold = 5 });
                _inventory.Add(new Book { Id = 2, Title = ".NET Core Essentials", Author = "Jane Smith", Price = 29.99m, Quantity = 3, RestockThreshold = 5 });
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
            string inventoryPath = Path.Combine(_dataFolder, InventoryFileName);
            string inventoryJson = JsonSerializer.Serialize(_inventory);
            File.WriteAllText(inventoryPath, inventoryJson);
            
            string salesPath = Path.Combine(_dataFolder, SalesFileName);
            string salesJson = JsonSerializer.Serialize(_sales);
            File.WriteAllText(salesPath, salesJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving data: " + ex.Message);
        }
    }
    
    private void CheckRestockAlerts()
    {
        bool needsRestock = false;
        
        foreach (var book in _inventory)
        {
            if (book.Quantity < book.RestockThreshold)
            {
                Console.WriteLine("RESTOCK ALERT: " + book.Title + " (Current: " + book.Quantity + ", Threshold: " + book.RestockThreshold + ")");
                needsRestock = true;
            }
        }
        
        if (!needsRestock)
        {
            Console.WriteLine("No restock alerts at this time");
        }
    }
    
    private void RecordSampleSale()
    {
        if (_inventory.Count == 0) return;
        
        var bookToSell = _inventory[0];
        if (bookToSell.Quantity > 0)
        {
            var sale = new Sale
            {
                BookId = bookToSell.Id,
                BookTitle = bookToSell.Title,
                SaleDate = DateTime.Now,
                Quantity = 1,
                TotalPrice = bookToSell.Price
            };
            
            _sales.Add(sale);
            bookToSell.Quantity--;
            
            Console.WriteLine("Recorded sale of " + bookToSell.Title + ". New quantity: " + bookToSell.Quantity);
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
    public int RestockThreshold { get; set; }
}

public class Sale
{
    public int BookId { get; set; }
    public string BookTitle { get; set; }
    public DateTime SaleDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}