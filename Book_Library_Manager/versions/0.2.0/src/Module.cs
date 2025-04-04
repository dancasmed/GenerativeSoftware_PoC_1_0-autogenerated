using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookLibraryManager : IGeneratedModule
{
    public string Name { get; set; } = "Book Library Manager";
    
    private string _dataFilePath;
    private List<Book> _books;
    
    public BookLibraryManager()
    {
        _books = new List<Book>();
    }
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "book_library.json");
        
        Console.WriteLine("Book Library Manager is running...");
        
        LoadBooks();
        
        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                    ListBooks();
                    break;
                case "3":
                    UpdateBookStatus();
                    break;
                case "4":
                    DeleteBook();
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
        Console.WriteLine("Book Library Manager has finished.");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nBook Library Manager");
        Console.WriteLine("1. Add a new book");
        Console.WriteLine("2. List all books");
        Console.WriteLine("3. Update book reading status");
        Console.WriteLine("4. Delete a book");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddBook()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter author: ");
        string author = Console.ReadLine();
        
        Console.Write("Enter genre: ");
        string genre = Console.ReadLine();
        
        Console.Write("Enter reading status (Not Started/In Progress/Completed): ");
        string status = Console.ReadLine();
        
        _books.Add(new Book
        {
            Title = title,
            Author = author,
            Genre = genre,
            ReadingStatus = status
        });
        
        Console.WriteLine("Book added successfully.");
    }
    
    private void ListBooks()
    {
        if (_books.Count == 0)
        {
            Console.WriteLine("No books in the library.");
            return;
        }
        
        Console.WriteLine("\nBooks in your library:");
        for (int i = 0; i < _books.Count; i++)
        {
            var book = _books[i];
            Console.WriteLine($"{i + 1}. {book.Title} by {book.Author} ({book.Genre}) - {book.ReadingStatus}");
        }
    }
    
    private void UpdateBookStatus()
    {
        ListBooks();
        if (_books.Count == 0) return;
        
        Console.Write("Enter the number of the book to update: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _books.Count)
        {
            Console.Write("Enter new reading status (Not Started/In Progress/Completed): ");
            string newStatus = Console.ReadLine();
            
            _books[index - 1].ReadingStatus = newStatus;
            Console.WriteLine("Book status updated successfully.");
        }
        else
        {
            Console.WriteLine("Invalid book number.");
        }
    }
    
    private void DeleteBook()
    {
        ListBooks();
        if (_books.Count == 0) return;
        
        Console.Write("Enter the number of the book to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _books.Count)
        {
            _books.RemoveAt(index - 1);
            Console.WriteLine("Book deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid book number.");
        }
    }
    
    private void LoadBooks()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _books = JsonSerializer.Deserialize<List<Book>>(json);
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
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving books: " + ex.Message);
        }
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string ReadingStatus { get; set; }
}