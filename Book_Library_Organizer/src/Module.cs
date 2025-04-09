using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookLibraryModule : IGeneratedModule
{
    public string Name { get; set; } = "Book Library Organizer";
    
    private List<Book> _books;
    private string _dataFilePath;
    
    public BookLibraryModule()
    {
        _books = new List<Book>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Book Library Organizer...");
        
        _dataFilePath = Path.Combine(dataFolder, "books.json");
        
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _books = JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();
                Console.WriteLine("Loaded existing book library.");
            }
            else
            {
                Console.WriteLine("No existing library found. Starting with an empty library.");
            }
            
            bool running = true;
            while (running)
            {
                DisplayMenu();
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        SearchBooks();
                        break;
                    case "3":
                        ListAllBooks();
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveLibrary();
            Console.WriteLine("Book library saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nBook Library Menu:");
        Console.WriteLine("1. Add a new book");
        Console.WriteLine("2. Search books");
        Console.WriteLine("3. List all books");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddBook()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter author name: ");
        string author = Console.ReadLine();
        
        Console.Write("Enter genre: ");
        string genre = Console.ReadLine();
        
        _books.Add(new Book { Title = title, Author = author, Genre = genre });
        Console.WriteLine("Book added successfully!");
    }
    
    private void SearchBooks()
    {
        Console.WriteLine("\nSearch by:");
        Console.WriteLine("1. Title");
        Console.WriteLine("2. Author");
        Console.WriteLine("3. Genre");
        Console.Write("Select search criteria: ");
        
        string criteria = Console.ReadLine();
        Console.Write("Enter search term: ");
        string term = Console.ReadLine().ToLower();
        
        List<Book> results = new List<Book>();
        
        switch (criteria)
        {
            case "1":
                results = _books.FindAll(b => b.Title.ToLower().Contains(term));
                break;
            case "2":
                results = _books.FindAll(b => b.Author.ToLower().Contains(term));
                break;
            case "3":
                results = _books.FindAll(b => b.Genre.ToLower().Contains(term));
                break;
            default:
                Console.WriteLine("Invalid search criteria.");
                return;
        }
        
        if (results.Count == 0)
        {
            Console.WriteLine("No books found matching your search.");
        }
        else
        {
            Console.WriteLine("\nSearch results:");
            foreach (var book in results)
            {
                Console.WriteLine($"{book.Title} by {book.Author} ({book.Genre})");
            }
        }
    }
    
    private void ListAllBooks()
    {
        if (_books.Count == 0)
        {
            Console.WriteLine("The library is currently empty.");
            return;
        }
        
        Console.WriteLine("\nAll books in the library:");
        foreach (var book in _books)
        {
            Console.WriteLine($"{book.Title} by {book.Author} ({book.Genre})");
        }
    }
    
    private void SaveLibrary()
    {
        string jsonData = JsonSerializer.Serialize(_books);
        File.WriteAllText(_dataFilePath, jsonData);
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
}