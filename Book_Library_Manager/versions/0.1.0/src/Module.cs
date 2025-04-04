using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class BookLibraryModule : IGeneratedModule
{
    public string Name { get; set; } = "Book Library Manager";
    
    private string _booksFilePath;
    private List<Book> _books;
    
    public BookLibraryModule()
    {
        _books = new List<Book>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Book Library Manager...");
        
        _booksFilePath = Path.Combine(dataFolder, "books.json");
        
        try
        {
            LoadBooks();
            RunLibraryManager();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void LoadBooks()
    {
        if (File.Exists(_booksFilePath))
        {
            string json = File.ReadAllText(_booksFilePath);
            _books = JsonSerializer.Deserialize<List<Book>>(json);
        }
    }
    
    private void SaveBooks()
    {
        string json = JsonSerializer.Serialize(_books);
        File.WriteAllText(_booksFilePath, json);
    }
    
    private void RunLibraryManager()
    {
        bool exit = false;
        
        while (!exit)
        {
            Console.WriteLine("\nBook Library Manager");
            Console.WriteLine("1. Add a book");
            Console.WriteLine("2. Search by title");
            Console.WriteLine("3. Search by author");
            Console.WriteLine("4. List all books");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
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
        Console.WriteLine("Library data saved. Exiting Book Library Manager.");
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
    
    private void DisplaySearchResults(List<Book> books, string searchType, string searchTerm)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books found matching the " + searchType + " '" + searchTerm + "'.");
        }
        else
        {
            Console.WriteLine("Found " + books.Count + " books matching the " + searchType + " '" + searchTerm + "':");
            foreach (var book in books)
            {
                Console.WriteLine(" - " + book.Title + " by " + book.Author + " (" + book.Year + ")");
            }
        }
    }
    
    private void ListAllBooks()
    {
        if (_books.Count == 0)
        {
            Console.WriteLine("The library is currently empty.");
        }
        else
        {
            Console.WriteLine("Library contains " + _books.Count + " books:");
            foreach (var book in _books.OrderBy(b => b.Title))
            {
                Console.WriteLine(" - " + book.Title + " by " + book.Author + " (" + book.Year + ")");
            }
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}