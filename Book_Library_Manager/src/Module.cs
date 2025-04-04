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
        Console.WriteLine("Initializing Book Library Module...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        _dataFilePath = Path.Combine(dataFolder, "books.json");
        
        LoadBooks();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nBook Library Menu:");
            Console.WriteLine("1. Add a book");
            Console.WriteLine("2. Search books by title");
            Console.WriteLine("3. Search books by author");
            Console.WriteLine("4. List all books");
            Console.WriteLine("5. Exit module");
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
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveBooks();
        Console.WriteLine("Book Library Module finished.");
        return true;
    }
    
    private void LoadBooks()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _books = JsonSerializer.Deserialize<List<Book>>(json);
                Console.WriteLine("Loaded " + _books.Count + " books from storage.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading books: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("No existing book data found. Starting with empty library.");
        }
    }
    
    private void SaveBooks()
    {
        try
        {
            string json = JsonSerializer.Serialize(_books);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Saved " + _books.Count + " books to storage.");
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
        
        var results = _books
            .Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        DisplaySearchResults(results, "title");
    }
    
    private void SearchByAuthor()
    {
        Console.Write("Enter author to search: ");
        string searchTerm = Console.ReadLine();
        
        var results = _books
            .Where(b => b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        DisplaySearchResults(results, "author");
    }
    
    private void ListAllBooks()
    {
        DisplaySearchResults(_books, "all");
    }
    
    private void DisplaySearchResults(List<Book> books, string searchType)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books found matching your search.");
            return;
        }
        
        Console.WriteLine("\nFound " + books.Count + " books" + (searchType != "all" ? " by " + searchType : "") + ":");
        foreach (var book in books)
        {
            Console.WriteLine("Title: " + book.Title);
            Console.WriteLine("Author: " + book.Author);
            Console.WriteLine("Year: " + book.Year);
            Console.WriteLine();
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}