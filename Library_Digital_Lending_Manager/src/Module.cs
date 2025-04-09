using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LibraryLendingModule : IGeneratedModule
{
    public string Name { get; set; } = "Library Digital Lending Manager";
    
    private string _booksFilePath;
    private string _usersFilePath;
    private string _loansFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Library Digital Lending Manager...");
        
        // Initialize file paths
        _booksFilePath = Path.Combine(dataFolder, "books.json");
        _usersFilePath = Path.Combine(dataFolder, "users.json");
        _loansFilePath = Path.Combine(dataFolder, "loans.json");
        
        // Ensure data directory exists
        Directory.CreateDirectory(dataFolder);
        
        // Initialize files if they don't exist
        InitializeFile(_booksFilePath, new List<Book>());
        InitializeFile(_usersFilePath, new List<User>());
        InitializeFile(_loansFilePath, new List<Loan>());
        
        // Main menu loop
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nLibrary Digital Lending Manager");
            Console.WriteLine("1. Manage Books");
            Console.WriteLine("2. Manage Users");
            Console.WriteLine("3. Manage Loans");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ManageBooks();
                    break;
                case "2":
                    ManageUsers();
                    break;
                case "3":
                    ManageLoans();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Library Digital Lending Manager is shutting down...");
        return true;
    }
    
    private void InitializeFile<T>(string filePath, T defaultValue)
    {
        if (!File.Exists(filePath))
        {
            string json = JsonSerializer.Serialize(defaultValue);
            File.WriteAllText(filePath, json);
        }
    }
    
    private void ManageBooks()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nBook Management");
            Console.WriteLine("1. List All Books");
            Console.WriteLine("2. Add New Book");
            Console.WriteLine("3. Remove Book");
            Console.WriteLine("4. Back");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ListBooks();
                    break;
                case "2":
                    AddBook();
                    break;
                case "3":
                    RemoveBook();
                    break;
                case "4":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private void ListBooks()
    {
        var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(_booksFilePath));
        Console.WriteLine("\nList of Books:");
        foreach (var book in books)
        {
            Console.WriteLine("ID: " + book.Id + ", Title: " + book.Title + ", Author: " + book.Author + ", Available: " + (book.IsAvailable ? "Yes" : "No"));
        }
    }
    
    private void AddBook()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        Console.Write("Enter book author: ");
        string author = Console.ReadLine();
        
        var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(_booksFilePath));
        books.Add(new Book
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Author = author,
            IsAvailable = true
        });
        
        File.WriteAllText(_booksFilePath, JsonSerializer.Serialize(books));
        Console.WriteLine("Book added successfully.");
    }
    
    private void RemoveBook()
    {
        Console.Write("Enter book ID to remove: ");
        string id = Console.ReadLine();
        
        var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(_booksFilePath));
        var book = books.Find(b => b.Id == id);
        
        if (book != null)
        {
            books.Remove(book);
            File.WriteAllText(_booksFilePath, JsonSerializer.Serialize(books));
            Console.WriteLine("Book removed successfully.");
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
    }
    
    private void ManageUsers()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nUser Management");
            Console.WriteLine("1. List All Users");
            Console.WriteLine("2. Add New User");
            Console.WriteLine("3. Remove User");
            Console.WriteLine("4. Back");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ListUsers();
                    break;
                case "2":
                    AddUser();
                    break;
                case "3":
                    RemoveUser();
                    break;
                case "4":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private void ListUsers()
    {
        var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_usersFilePath));
        Console.WriteLine("\nList of Users:");
        foreach (var user in users)
        {
            Console.WriteLine("ID: " + user.Id + ", Name: " + user.Name + ", Email: " + user.Email);
        }
    }
    
    private void AddUser()
    {
        Console.Write("Enter user name: ");
        string name = Console.ReadLine();
        Console.Write("Enter user email: ");
        string email = Console.ReadLine();
        
        var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_usersFilePath));
        users.Add(new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Email = email
        });
        
        File.WriteAllText(_usersFilePath, JsonSerializer.Serialize(users));
        Console.WriteLine("User added successfully.");
    }
    
    private void RemoveUser()
    {
        Console.Write("Enter user ID to remove: ");
        string id = Console.ReadLine();
        
        var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_usersFilePath));
        var user = users.Find(u => u.Id == id);
        
        if (user != null)
        {
            users.Remove(user);
            File.WriteAllText(_usersFilePath, JsonSerializer.Serialize(users));
            Console.WriteLine("User removed successfully.");
        }
        else
        {
            Console.WriteLine("User not found.");
        }
    }
    
    private void ManageLoans()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nLoan Management");
            Console.WriteLine("1. List All Loans");
            Console.WriteLine("2. Create New Loan");
            Console.WriteLine("3. Return Book");
            Console.WriteLine("4. Back");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ListLoans();
                    break;
                case "2":
                    CreateLoan();
                    break;
                case "3":
                    ReturnBook();
                    break;
                case "4":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private void ListLoans()
    {
        var loans = JsonSerializer.Deserialize<List<Loan>>(File.ReadAllText(_loansFilePath));
        var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(_booksFilePath));
        var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_usersFilePath));
        
        Console.WriteLine("\nList of Active Loans:");
        foreach (var loan in loans)
        {
            if (!loan.IsReturned)
            {
                var book = books.Find(b => b.Id == loan.BookId);
                var user = users.Find(u => u.Id == loan.UserId);
                
                Console.WriteLine("Loan ID: " + loan.Id + ", Book: " + book.Title + ", User: " + user.Name + ", Due Date: " + loan.DueDate.ToString("yyyy-MM-dd"));
            }
        }
    }
    
    private void CreateLoan()
    {
        Console.Write("Enter user ID: ");
        string userId = Console.ReadLine();
        Console.Write("Enter book ID: ");
        string bookId = Console.ReadLine();
        
        var users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_usersFilePath));
        var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(_booksFilePath));
        
        var user = users.Find(u => u.Id == userId);
        var book = books.Find(b => b.Id == bookId);
        
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }
        
        if (book == null)
        {
            Console.WriteLine("Book not found.");
            return;
        }
        
        if (!book.IsAvailable)
        {
            Console.WriteLine("Book is not available for loan.");
            return;
        }
        
        // Update book availability
        book.IsAvailable = false;
        File.WriteAllText(_booksFilePath, JsonSerializer.Serialize(books));
        
        // Create loan
        var loans = JsonSerializer.Deserialize<List<Loan>>(File.ReadAllText(_loansFilePath));
        loans.Add(new Loan
        {
            Id = Guid.NewGuid().ToString(),
            BookId = bookId,
            UserId = userId,
            LoanDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(14),
            IsReturned = false
        });
        
        File.WriteAllText(_loansFilePath, JsonSerializer.Serialize(loans));
        Console.WriteLine("Loan created successfully. Due date: " + DateTime.Now.AddDays(14).ToString("yyyy-MM-dd"));
    }
    
    private void ReturnBook()
    {
        Console.Write("Enter loan ID: ");
        string loanId = Console.ReadLine();
        
        var loans = JsonSerializer.Deserialize<List<Loan>>(File.ReadAllText(_loansFilePath));
        var loan = loans.Find(l => l.Id == loanId);
        
        if (loan == null || loan.IsReturned)
        {
            Console.WriteLine("Loan not found or already returned.");
            return;
        }
        
        // Update loan status
        loan.IsReturned = true;
        loan.ReturnDate = DateTime.Now;
        
        // Update book availability
        var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(_booksFilePath));
        var book = books.Find(b => b.Id == loan.BookId);
        book.IsAvailable = true;
        
        File.WriteAllText(_loansFilePath, JsonSerializer.Serialize(loans));
        File.WriteAllText(_booksFilePath, JsonSerializer.Serialize(books));
        
        Console.WriteLine("Book returned successfully.");
    }
}

public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public bool IsAvailable { get; set; }
}

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Loan
{
    public string Id { get; set; }
    public string BookId { get; set; }
    public string UserId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned { get; set; }
}