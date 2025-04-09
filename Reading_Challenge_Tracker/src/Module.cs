using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ReadingChallengeTracker : IGeneratedModule
{
    public string Name { get; set; } = "Reading Challenge Tracker";
    
    private string _booksFilePath;
    private string _progressFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Reading Challenge Tracker...");
        
        try
        {
            // Ensure data directory exists
            Directory.CreateDirectory(dataFolder);
            
            _booksFilePath = Path.Combine(dataFolder, "books.json");
            _progressFilePath = Path.Combine(dataFolder, "progress.json");
            
            // Initialize files if they don't exist
            if (!File.Exists(_booksFilePath))
            {
                File.WriteAllText(_booksFilePath, "[]");
            }
            
            if (!File.Exists(_progressFilePath))
            {
                var initialProgress = new ReadingProgress
                {
                    TargetBooks = 0,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6)
                };
                
                File.WriteAllText(_progressFilePath, JsonSerializer.Serialize(initialProgress));
            }
            
            // Load data
            var books = LoadBooks();
            var progress = LoadProgress();
            
            // Display summary
            Console.WriteLine("Reading Challenge Summary:");
            Console.WriteLine("Target Books: " + progress.TargetBooks);
            Console.WriteLine("Books Completed: " + progress.CompletedBooks.Count);
            Console.WriteLine("Progress: " + CalculateProgressPercentage(progress) + "%");
            Console.WriteLine("Days Remaining: " + (progress.EndDate - DateTime.Now).Days);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private List<Book> LoadBooks()
    {
        var json = File.ReadAllText(_booksFilePath);
        return JsonSerializer.Deserialize<List<Book>>(json);
    }
    
    private ReadingProgress LoadProgress()
    {
        var json = File.ReadAllText(_progressFilePath);
        return JsonSerializer.Deserialize<ReadingProgress>(json);
    }
    
    private double CalculateProgressPercentage(ReadingProgress progress)
    {
        if (progress.TargetBooks == 0) return 0;
        return Math.Round((double)progress.CompletedBooks.Count / progress.TargetBooks * 100, 2);
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Pages { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class ReadingProgress
{
    public int TargetBooks { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> CompletedBooks { get; set; } = new List<string>();
}