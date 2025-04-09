using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LibraryLendingSystem : IGeneratedModule
{
    public string Name { get; set; } = "Library Lending System";

    private string _booksFilePath;
    private string _loansFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Library Lending System is running...");

        _booksFilePath = Path.Combine(dataFolder, "books.json");
        _loansFilePath = Path.Combine(dataFolder, "loans.json");

        InitializeDataFiles();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nLibrary Lending System Menu:");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. List Books");
            Console.WriteLine("3. Lend Book");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. List Loans");
            Console.WriteLine("6. Exit");
            Console.Write("Select an option: ");

            var input = Console.ReadLine();
            if (int.TryParse(input, out int option))
            {
                switch (option)
                {
                    case 1:
                        AddBook();
                        break;
                    case 2:
                        ListBooks();
                        break;
                    case 3:
                        LendBook();
                        break;
                    case 4:
                        ReturnBook();
                        break;
                    case 5:
                        ListLoans();
                        break;
                    case 6:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }

        Console.WriteLine("Library Lending System is shutting down...");
        return true;
    }

    private void InitializeDataFiles()
    {
        if (!Directory.Exists(Path.GetDirectoryName(_booksFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_booksFilePath));
        }

        if (!File.Exists(_booksFilePath))
        {
            File.WriteAllText(_booksFilePath, "[]");
        }

        if (!File.Exists(_loansFilePath))
        {
            File.WriteAllText(_loansFilePath, "[]");
        }
    }

    private void AddBook()
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();

        Console.Write("Enter author: ");
        string author = Console.ReadLine();

        Console.Write("Enter ISBN: ");
        string isbn = Console.ReadLine();

        var books = GetBooks();
        books.Add(new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            ISBN = isbn,
            Available = true
        });

        SaveBooks(books);
        Console.WriteLine("Book added successfully.");
    }

    private void ListBooks()
    {
        var books = GetBooks();
        Console.WriteLine("\nAvailable Books:");
        foreach (var book in books)
        {
            Console.WriteLine($"{book.Title} by {book.Author} (ISBN: {book.ISBN}) - {(book.Available ? "Available" : "On Loan")}");
        }
    }

    private void LendBook()
    {
        ListBooks();
        Console.Write("\nEnter ISBN of book to lend: ");
        string isbn = Console.ReadLine();

        var books = GetBooks();
        var book = books.Find(b => b.ISBN == isbn && b.Available);

        if (book == null)
        {
            Console.WriteLine("Book not found or already on loan.");
            return;
        }

        Console.Write("Enter borrower name: ");
        string borrower = Console.ReadLine();

        Console.Write("Enter loan duration (days): ");
        if (!int.TryParse(Console.ReadLine(), out int days))
        {
            Console.WriteLine("Invalid duration.");
            return;
        }

        var loans = GetLoans();
        var dueDate = DateTime.Now.AddDays(days);

        loans.Add(new Loan
        {
            Id = Guid.NewGuid(),
            BookId = book.Id,
            Borrower = borrower,
            LoanDate = DateTime.Now,
            DueDate = dueDate,
            Returned = false
        });

        book.Available = false;
        SaveBooks(books);
        SaveLoans(loans);

        Console.WriteLine($"Book '{book.Title}' lent to {borrower} until {dueDate.ToShortDateString()}.");
    }

    private void ReturnBook()
    {
        Console.Write("Enter ISBN of book to return: ");
        string isbn = Console.ReadLine();

        var books = GetBooks();
        var book = books.Find(b => b.ISBN == isbn && !b.Available);

        if (book == null)
        {
            Console.WriteLine("Book not found or already returned.");
            return;
        }

        var loans = GetLoans();
        var loan = loans.Find(l => l.BookId == book.Id && !l.Returned);

        if (loan == null)
        {
            Console.WriteLine("Loan record not found.");
            return;
        }

        loan.Returned = true;
        loan.ActualReturnDate = DateTime.Now;
        book.Available = true;

        SaveBooks(books);
        SaveLoans(loans);

        Console.WriteLine($"Book '{book.Title}' returned successfully.");
    }

    private void ListLoans()
    {
        var loans = GetLoans();
        var books = GetBooks();

        Console.WriteLine("\nCurrent Loans:");
        foreach (var loan in loans)
        {
            if (!loan.Returned)
            {
                var book = books.Find(b => b.Id == loan.BookId);
                Console.WriteLine($"{book?.Title ?? "Unknown Book"} loaned to {loan.Borrower} until {loan.DueDate.ToShortDateString()}");
            }
        }

        Console.WriteLine("\nReturned Loans:");
        foreach (var loan in loans)
        {
            if (loan.Returned && loan.ActualReturnDate.HasValue)
            {
                var book = books.Find(b => b.Id == loan.BookId);
                Console.WriteLine($"{book?.Title ?? "Unknown Book"} returned by {loan.Borrower} on {loan.ActualReturnDate.Value.ToShortDateString()}");
            }
        }
    }

    private List<Book> GetBooks()
    {
        var json = File.ReadAllText(_booksFilePath);
        return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
    }

    private void SaveBooks(List<Book> books)
    {
        var json = JsonSerializer.Serialize(books);
        File.WriteAllText(_booksFilePath, json);
    }

    private List<Loan> GetLoans()
    {
        var json = File.ReadAllText(_loansFilePath);
        return JsonSerializer.Deserialize<List<Loan>>(json) ?? new List<Loan>();
    }

    private void SaveLoans(List<Loan> loans)
    {
        var json = JsonSerializer.Serialize(loans);
        File.WriteAllText(_loansFilePath, json);
    }
}

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public bool Available { get; set; }
}

public class Loan
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string Borrower { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool Returned { get; set; }
    public DateTime? ActualReturnDate { get; set; }
}