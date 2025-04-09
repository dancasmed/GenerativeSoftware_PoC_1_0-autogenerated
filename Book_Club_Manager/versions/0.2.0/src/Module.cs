using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookClubManager : IGeneratedModule
{
    public string Name { get; set; } = "Book Club Manager";

    private string _booksFilePath;
    private string _meetingsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Book Club Manager...");

        _booksFilePath = Path.Combine(dataFolder, "books.json");
        _meetingsFilePath = Path.Combine(dataFolder, "meetings.json");

        EnsureDataFilesExist();

        bool running = true;
        while (running)
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
                    ScheduleMeeting();
                    break;
                case "4":
                    ListMeetings();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Book Club Manager is shutting down...");
        return true;
    }

    private void EnsureDataFilesExist()
    {
        if (!File.Exists(_booksFilePath))
        {
            File.WriteAllText(_booksFilePath, "[]");
        }

        if (!File.Exists(_meetingsFilePath))
        {
            File.WriteAllText(_meetingsFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nBook Club Manager");
        Console.WriteLine("1. Add a book");
        Console.WriteLine("2. List all books");
        Console.WriteLine("3. Schedule a meeting");
        Console.WriteLine("4. List all meetings");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddBook()
    {
        Console.Write("Enter book title: ");
        var title = Console.ReadLine();

        Console.Write("Enter author: ");
        var author = Console.ReadLine();

        Console.Write("Enter publication year: ");
        var yearInput = Console.ReadLine();

        if (!int.TryParse(yearInput, out int year))
        {
            Console.WriteLine("Invalid year. Book not added.");
            return;
        }

        var books = LoadBooks();
        books.Add(new Book { Title = title, Author = author, Year = year });
        SaveBooks(books);

        Console.WriteLine("Book added successfully.");
    }

    private void ListBooks()
    {
        var books = LoadBooks();

        if (books.Count == 0)
        {
            Console.WriteLine("No books in the reading list.");
            return;
        }

        Console.WriteLine("\nReading List:");
        foreach (var book in books)
        {
            Console.WriteLine($"{book.Title} by {book.Author} ({book.Year})");
        }
    }

    private void ScheduleMeeting()
    {
        Console.Write("Enter meeting date (yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();

        if (!DateTime.TryParse(dateInput, out DateTime date))
        {
            Console.WriteLine("Invalid date format. Meeting not scheduled.");
            return;
        }

        Console.Write("Enter meeting time (HH:mm): ");
        var timeInput = Console.ReadLine();

        if (!TimeSpan.TryParse(timeInput, out TimeSpan time))
        {
            Console.WriteLine("Invalid time format. Meeting not scheduled.");
            return;
        }

        var dateTime = date.Date.Add(time);

        Console.Write("Enter location: ");
        var location = Console.ReadLine();

        var books = LoadBooks();
        if (books.Count == 0)
        {
            Console.WriteLine("No books available to discuss. Add books first.");
            return;
        }

        Console.WriteLine("Select a book to discuss:");
        for (int i = 0; i < books.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {books[i].Title}");
        }

        var bookChoiceInput = Console.ReadLine();
        if (!int.TryParse(bookChoiceInput, out int bookChoice) || bookChoice < 1 || bookChoice > books.Count)
        {
            Console.WriteLine("Invalid book selection. Meeting not scheduled.");
            return;
        }

        var meetings = LoadMeetings();
        meetings.Add(new Meeting
        {
            DateTime = dateTime,
            Location = location,
            BookTitle = books[bookChoice - 1].Title
        });

        SaveMeetings(meetings);
        Console.WriteLine("Meeting scheduled successfully.");
    }

    private void ListMeetings()
    {
        var meetings = LoadMeetings();

        if (meetings.Count == 0)
        {
            Console.WriteLine("No meetings scheduled.");
            return;
        }

        Console.WriteLine("\nScheduled Meetings:");
        foreach (var meeting in meetings)
        {
            Console.WriteLine($"{meeting.DateTime:yyyy-MM-dd HH:mm} at {meeting.Location} - Discussing: {meeting.BookTitle}");
        }
    }

    private List<Book> LoadBooks()
    {
        var json = File.ReadAllText(_booksFilePath);
        return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
    }

    private void SaveBooks(List<Book> books)
    {
        var json = JsonSerializer.Serialize(books);
        File.WriteAllText(_booksFilePath, json);
    }

    private List<Meeting> LoadMeetings()
    {
        var json = File.ReadAllText(_meetingsFilePath);
        return JsonSerializer.Deserialize<List<Meeting>>(json) ?? new List<Meeting>();
    }

    private void SaveMeetings(List<Meeting> meetings)
    {
        var json = JsonSerializer.Serialize(meetings);
        File.WriteAllText(_meetingsFilePath, json);
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}

public class Meeting
{
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string BookTitle { get; set; }
}