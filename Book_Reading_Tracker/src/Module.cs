using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BookReadingTracker : IGeneratedModule
{
    public string Name { get; set; } = "Book Reading Tracker";

    private string _dataFilePath;
    
    private class BookProgress
    {
        public string Title { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public double PercentageCompleted => TotalPages > 0 ? (CurrentPage * 100.0) / TotalPages : 0;
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "book_progress.json");
        
        Console.WriteLine("Book Reading Tracker Module Started");
        Console.WriteLine("----------------------------------");
        
        List<BookProgress> books = LoadBookProgress();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddNewBook(books);
                    break;
                case "2":
                    UpdateReadingProgress(books);
                    break;
                case "3":
                    ViewAllBooks(books);
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveBookProgress(books);
        }
        
        Console.WriteLine("Book Reading Tracker Module Finished");
        return true;
    }
    
    private List<BookProgress> LoadBookProgress()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<BookProgress>>(json) ?? new List<BookProgress>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading book progress: " + ex.Message);
        }
        
        return new List<BookProgress>();
    }
    
    private void SaveBookProgress(List<BookProgress> books)
    {
        try
        {
            string json = JsonSerializer.Serialize(books);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving book progress: " + ex.Message);
        }
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine();
        Console.WriteLine("BOOK READING TRACKER");
        Console.WriteLine("1. Add New Book");
        Console.WriteLine("2. Update Reading Progress");
        Console.WriteLine("3. View All Books");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddNewBook(List<BookProgress> books)
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter total pages: ");
        if (int.TryParse(Console.ReadLine(), out int totalPages))
        {
            books.Add(new BookProgress { Title = title, TotalPages = totalPages, CurrentPage = 0 });
            Console.WriteLine("Book added successfully!");
        }
        else
        {
            Console.WriteLine("Invalid number of pages. Book not added.");
        }
    }
    
    private void UpdateReadingProgress(List<BookProgress> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books available to update.");
            return;
        }
        
        Console.WriteLine("Select a book to update:");
        for (int i = 0; i < books.Count; i++)
        {
            Console.WriteLine(string.Format("{0}. {1} (Page {2}/{3}, {4:0.00}%)", 
                i + 1, 
                books[i].Title, 
                books[i].CurrentPage, 
                books[i].TotalPages, 
                books[i].PercentageCompleted));
        }
        
        Console.Write("Enter book number: ");
        if (int.TryParse(Console.ReadLine(), out int bookIndex) && bookIndex > 0 && bookIndex <= books.Count)
        {
            var book = books[bookIndex - 1];
            
            Console.Write(string.Format("Enter current page (0-{0}): ", book.TotalPages));
            if (int.TryParse(Console.ReadLine(), out int currentPage) && currentPage >= 0 && currentPage <= book.TotalPages)
            {
                book.CurrentPage = currentPage;
                Console.WriteLine(string.Format("Progress updated: {0:0.00}% completed", book.PercentageCompleted));
            }
            else
            {
                Console.WriteLine("Invalid page number.");
            }
        }
        else
        {
            Console.WriteLine("Invalid book selection.");
        }
    }
    
    private void ViewAllBooks(List<BookProgress> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books in your reading list.");
            return;
        }
        
        Console.WriteLine("YOUR READING PROGRESS");
        Console.WriteLine("----------------------");
        
        foreach (var book in books)
        {
            Console.WriteLine(string.Format("{0}: Page {1}/{2} ({3:0.00}% completed)", 
                book.Title, 
                book.CurrentPage, 
                book.TotalPages, 
                book.PercentageCompleted));
        }
    }
}