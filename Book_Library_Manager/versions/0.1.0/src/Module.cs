using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class BookLibraryModule : IGeneratedModule
{
    public string Name { get; set; } = "Book Library Manager";
    
    private List<Book> _books;
    private string _dataFilePath;
    
    public BookLibraryModule()
    {
        _books = new List<Book>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Book Library Module is running...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        _dataFilePath = Path.Combine(dataFolder, "books.json");
        
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _books = JsonSerializer.Deserialize<List<Book>>(json);
                Console.WriteLine("Book library loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading book library: " + ex.Message);
                return false;
            }
        }
        else
        {
            Console.WriteLine("No existing book library found. Starting with empty library.");
        }
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nBook Library Menu:");
            Console.WriteLine("1. Add a book");
            Console.WriteLine("2. Search by title");
            Console.WriteLine("3. Search by author");
            Console.WriteLine("4. List all books");
            Console.WriteLine("5. Save and exit");
            Console.Write("Enter your choice: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
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
                    running = SaveLibrary();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void AddBook()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter author name: ");
        string author = Console.ReadLine();
        
        Console.Write("Enter publication year: ");
        if (int.TryParse(Console.ReadLine(), out int year))
        {
            _books.Add(new Book { Title = title, Author = author, Year = year });
            Console.WriteLine("Book added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid year format. Book not added.");
        }
    }
    
    private void SearchByTitle()
    {
        Console.Write("Enter title to search: ");
        string searchTerm = Console.ReadLine();
        
        var results = _books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                           .ToList();
        
        DisplaySearchResults(results, "title", searchTerm);
    }
    
    private void SearchByAuthor()
    {
        Console.Write("Enter author to search: ");
        string searchTerm = Console.ReadLine();
        
        var results = _books.Where(b => b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                           .ToList();
        
        DisplaySearchResults(results, "author", searchTerm);
    }
    
    private void DisplaySearchResults(List<Book> results, string searchType, string searchTerm)
    {
        if (results.Any())
        {
            Console.WriteLine("\nFound " + results.Count + " books by " + searchType + " '" + searchTerm + "':");
            foreach (var book in results)
            {
                Console.WriteLine(" - " + book.Title + " by " + book.Author + " (" + book.Year + ")");
            }
        }
        else
        {
            Console.WriteLine("No books found by " + searchType + " '" + searchTerm + "'.");
        }
    }
    
    private void ListAllBooks()
    {
        if (_books.Any())
        {
            Console.WriteLine("\nAll books in library (" + _books.Count + " total):");
            foreach (var book in _books.OrderBy(b => b.Title))
            {
                Console.WriteLine(" - " + book.Title + " by " + book.Author + " (" + book.Year + ")");
            }
        }
        else
        {
            Console.WriteLine("The library is currently empty.");
        }
    }
    
    private bool SaveLibrary()
    {
        try
        {
            string json = JsonSerializer.Serialize(_books);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Library saved successfully.");
            return false; // Exit the loop
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving library: " + ex.Message);
            return true; // Continue running
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}