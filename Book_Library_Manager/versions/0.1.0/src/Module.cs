using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class BookLibraryManager : IGeneratedModule
{
    public string Name { get; set; } = "Book Library Manager";
    
    private List<Book> _books;
    private string _dataFilePath;
    
    public BookLibraryManager()
    {
        _books = new List<Book>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Book Library Manager is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "books.json");
        
        LoadBooks();
        
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add a book");
            Console.WriteLine("2. Search by title");
            Console.WriteLine("3. Search by author");
            Console.WriteLine("4. List all books");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                    SearchByTitle();
                    break;
                case "3":
                    SearchByAuthor();
                    break;
                case "4":
                    ListAllBooks();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveBooks();
        Console.WriteLine("Book Library Manager is shutting down...");
        return true;
    }
    
    private void LoadBooks()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _books = JsonSerializer.Deserialize<List<Book>>(json);
                Console.WriteLine("Books loaded successfully.");
            }
            else
            {
                Console.WriteLine("No existing book data found. Starting with an empty library.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading books: " + ex.Message);
        }
    }
    
    private void SaveBooks()
    {
        try
        {
            string json = JsonSerializer.Serialize(_books);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Books saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving books: " + ex.Message);
        }
    }
    
    private void AddBook()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter book author: ");
        string author = Console.ReadLine();
        
        Console.Write("Enter publication year: ");
        if (int.TryParse(Console.ReadLine(), out int year))
        {
            _books.Add(new Book { Title = title, Author = author, Year = year });
            Console.WriteLine("Book added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid year. Book not added.");
        }
    }
    
    private void SearchByTitle()
    {
        Console.Write("Enter title to search: ");
        string searchTerm = Console.ReadLine();
        
        var results = _books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        
        if (results.Any())
        {
            Console.WriteLine("\nSearch results:");
            foreach (var book in results)
            {
                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year})");
            }
        }
        else
        {
            Console.WriteLine("No books found with that title.");
        }
    }
    
    private void SearchByAuthor()
    {
        Console.Write("Enter author to search: ");
        string searchTerm = Console.ReadLine();
        
        var results = _books.Where(b => b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        
        if (results.Any())
        {
            Console.WriteLine("\nSearch results:");
            foreach (var book in results)
            {
                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year})");
            }
        }
        else
        {
            Console.WriteLine("No books found by that author.");
        }
    }
    
    private void ListAllBooks()
    {
        if (_books.Any())
        {
            Console.WriteLine("\nAll books in library:");
            foreach (var book in _books.OrderBy(b => b.Title))
            {
                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year})");
            }
        }
        else
        {
            Console.WriteLine("The library is currently empty.");
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}