using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ComicBookManager : IGeneratedModule
{
    public string Name { get; set; } = "Comic Book Manager";
    
    private List<ComicBook> _comicBooks;
    private string _dataFilePath;
    
    public ComicBookManager()
    {
        _comicBooks = new List<ComicBook>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Comic Book Manager module is running...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        _dataFilePath = Path.Combine(dataFolder, "comicbooks.json");
        
        LoadComicBooks();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddComicBook();
                    break;
                case "2":
                    ListComicBooks();
                    break;
                case "3":
                    SearchComicBooks();
                    break;
                case "4":
                    SaveComicBooks();
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nComic Book Manager");
        Console.WriteLine("1. Add Comic Book");
        Console.WriteLine("2. List All Comic Books");
        Console.WriteLine("3. Search Comic Books");
        Console.WriteLine("4. Exit and Save");
        Console.Write("Select an option: ");
    }
    
    private void AddComicBook()
    {
        Console.Write("Enter title: ");
        var title = Console.ReadLine();
        
        Console.Write("Enter issue number: ");
        var issueNumber = Console.ReadLine();
        
        Console.Write("Enter publisher: ");
        var publisher = Console.ReadLine();
        
        Console.Write("Enter year: ");
        var yearInput = Console.ReadLine();
        
        if (int.TryParse(yearInput, out int year))
        {
            _comicBooks.Add(new ComicBook
            {
                Title = title,
                IssueNumber = issueNumber,
                Publisher = publisher,
                Year = year
            });
            
            Console.WriteLine("Comic book added successfully!");
        }
        else
        {
            Console.WriteLine("Invalid year. Comic book not added.");
        }
    }
    
    private void ListComicBooks()
    {
        Console.WriteLine("\nAll Comic Books:");
        Console.WriteLine("----------------");
        
        if (_comicBooks.Count == 0)
        {
            Console.WriteLine("No comic books found.");
            return;
        }
        
        foreach (var comic in _comicBooks)
        {
            Console.WriteLine("Title: " + comic.Title);
            Console.WriteLine("Issue: " + comic.IssueNumber);
            Console.WriteLine("Publisher: " + comic.Publisher);
            Console.WriteLine("Year: " + comic.Year);
            Console.WriteLine("----------------");
        }
    }
    
    private void SearchComicBooks()
    {
        Console.Write("Enter search term: ");
        var term = Console.ReadLine().ToLower();
        
        var results = _comicBooks.FindAll(c => 
            c.Title.ToLower().Contains(term) || 
            c.Publisher.ToLower().Contains(term) ||
            c.IssueNumber.ToLower().Contains(term));
        
        Console.WriteLine("\nSearch Results:");
        Console.WriteLine("----------------");
        
        if (results.Count == 0)
        {
            Console.WriteLine("No matching comic books found.");
            return;
        }
        
        foreach (var comic in results)
        {
            Console.WriteLine("Title: " + comic.Title);
            Console.WriteLine("Issue: " + comic.IssueNumber);
            Console.WriteLine("Publisher: " + comic.Publisher);
            Console.WriteLine("Year: " + comic.Year);
            Console.WriteLine("----------------");
        }
    }
    
    private void LoadComicBooks()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                var json = File.ReadAllText(_dataFilePath);
                _comicBooks = JsonSerializer.Deserialize<List<ComicBook>>(json);
                Console.WriteLine("Comic books loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading comic books: " + ex.Message);
            }
        }
    }
    
    private void SaveComicBooks()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_comicBooks, options);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Comic books saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving comic books: " + ex.Message);
        }
    }
}

public class ComicBook
{
    public string Title { get; set; }
    public string IssueNumber { get; set; }
    public string Publisher { get; set; }
    public int Year { get; set; }
}