using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class LibraryManager : IGeneratedModule
{
    public string Name { get; set; } = "Library Manager";
    
    private List<Book> _books;
    private string _dataFilePath;
    
    public LibraryManager()
    {
        _books = new List<Book>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Library Manager module is running.");
        
        _dataFilePath = Path.Combine(dataFolder, "library_data.json");
        
        try
        {
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
                Console.Write("Select an option: ");
                
                var input = Console.ReadLine();
                
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
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LoadBooks()
    {
        if (File.Exists(_dataFilePath))
        {
            var json = File.ReadAllText(_dataFilePath);
            _books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }
    }
    
    private void SaveBooks()
    {
        var json = JsonSerializer.Serialize(_books);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddBook()
    {
        Console.Write("Enter book title: ");
        var title = Console.ReadLine();
        
        Console.Write("Enter book author: ");
        var author = Console.ReadLine();
        
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
        var searchTerm = Console.ReadLine();
        
        var results = _books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        DisplayResults(results, "Books found by title:");
    }
    
    private void SearchByAuthor()
    {
        Console.Write("Enter author to search: ");
        var searchTerm = Console.ReadLine();
        
        var results = _books.Where(b => b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        DisplayResults(results, "Books found by author:");
    }
    
    private void ListAllBooks()
    {
        DisplayResults(_books, "All books in library:");
    }
    
    private void DisplayResults(List<Book> books, string header)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }
        
        Console.WriteLine(header);
        foreach (var book in books)
        {
            Console.WriteLine($"{book.Title} by {book.Author} ({book.Year})");
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}