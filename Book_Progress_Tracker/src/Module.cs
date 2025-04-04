using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookProgressTracker : IGeneratedModule
{
    public string Name { get; set; } = "Book Progress Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "book_progress.json");
        
        Console.WriteLine("Book Progress Tracker Module Started");
        Console.WriteLine("------------------------------------");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        var books = LoadBooks();
        
        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add a new book");
            Console.WriteLine("2. Update reading progress");
            Console.WriteLine("3. View all books");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    AddBook(books);
                    break;
                case "2":
                    UpdateProgress(books);
                    break;
                case "3":
                    DisplayBooks(books);
                    break;
                case "4":
                    SaveBooks(books);
                    Console.WriteLine("Saving progress and exiting module...");
                    return true;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
    
    private List<Book> LoadBooks()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<Book>();
        }
        
        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<Book>>(json);
    }
    
    private void SaveBooks(List<Book> books)
    {
        var json = JsonSerializer.Serialize(books);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddBook(List<Book> books)
    {
        Console.Write("Enter book title: ");
        var title = Console.ReadLine();
        
        Console.Write("Enter total pages: ");
        if (!int.TryParse(Console.ReadLine(), out var totalPages))
        {
            Console.WriteLine("Invalid number of pages.");
            return;
        }
        
        books.Add(new Book
        {
            Title = title,
            TotalPages = totalPages,
            PagesRead = 0
        });
        
        Console.WriteLine("Book added successfully!");
    }
    
    private void UpdateProgress(List<Book> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books available to update.");
            return;
        }
        
        DisplayBooks(books);
        
        Console.Write("Enter book number to update: ");
        if (!int.TryParse(Console.ReadLine(), out var bookIndex) || bookIndex < 1 || bookIndex > books.Count)
        {
            Console.WriteLine("Invalid book selection.");
            return;
        }
        
        var book = books[bookIndex - 1];
        
        Console.Write("Enter pages read: ");
        if (!int.TryParse(Console.ReadLine(), out var pagesRead) || pagesRead < 0 || pagesRead > book.TotalPages)
        {
            Console.WriteLine("Invalid number of pages read.");
            return;
        }
        
        book.PagesRead = pagesRead;
        
        var percentage = (double)pagesRead / book.TotalPages * 100;
        Console.WriteLine("Progress updated: " + percentage.ToString("0.00") + "% completed");
    }
    
    private void DisplayBooks(List<Book> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books to display.");
            return;
        }
        
        Console.WriteLine("\nCurrent Books:");
        for (int i = 0; i < books.Count; i++)
        {
            var book = books[i];
            var percentage = book.TotalPages > 0 ? (double)book.PagesRead / book.TotalPages * 100 : 0;
            Console.WriteLine($"{i + 1}. {book.Title} - {book.PagesRead}/{book.TotalPages} pages ({percentage.ToString("0.00")}%)");
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public int TotalPages { get; set; }
    public int PagesRead { get; set; }
}