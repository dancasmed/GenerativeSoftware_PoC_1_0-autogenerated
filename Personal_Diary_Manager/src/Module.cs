using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PersonalDiaryModule : IGeneratedModule
{
    public string Name { get; set; } = "Personal Diary Manager";
    
    private string _entriesFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Personal Diary Manager is running...");
        
        _entriesFilePath = Path.Combine(dataFolder, "diary_entries.json");
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddNewEntry();
                    break;
                case "2":
                    ViewAllEntries();
                    break;
                case "3":
                    SearchEntries();
                    break;
                case "4":
                    continueRunning = false;
                    Console.WriteLine("Exiting Personal Diary Manager...");
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
        Console.WriteLine("\nPersonal Diary Menu:");
        Console.WriteLine("1. Add new entry");
        Console.WriteLine("2. View all entries");
        Console.WriteLine("3. Search entries");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddNewEntry()
    {
        Console.Write("Enter your diary entry: ");
        string content = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(content))
        {
            Console.WriteLine("Entry cannot be empty.");
            return;
        }
        
        var newEntry = new DiaryEntry
        {
            Date = DateTime.Now,
            Content = content
        };
        
        List<DiaryEntry> entries = LoadEntries();
        entries.Add(newEntry);
        
        SaveEntries(entries);
        
        Console.WriteLine("Entry added successfully.");
    }
    
    private void ViewAllEntries()
    {
        List<DiaryEntry> entries = LoadEntries();
        
        if (entries.Count == 0)
        {
            Console.WriteLine("No entries found.");
            return;
        }
        
        Console.WriteLine("\nAll Diary Entries:");
        foreach (var entry in entries)
        {
            Console.WriteLine($"{entry.Date}: {entry.Content}");
        }
    }
    
    private void SearchEntries()
    {
        Console.Write("Enter search term: ");
        string searchTerm = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            Console.WriteLine("Search term cannot be empty.");
            return;
        }
        
        List<DiaryEntry> entries = LoadEntries();
        var results = entries.FindAll(e => e.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        
        if (results.Count == 0)
        {
            Console.WriteLine("No matching entries found.");
            return;
        }
        
        Console.WriteLine("\nSearch Results:");
        foreach (var entry in results)
        {
            Console.WriteLine($"{entry.Date}: {entry.Content}");
        }
    }
    
    private List<DiaryEntry> LoadEntries()
    {
        if (!File.Exists(_entriesFilePath))
        {
            return new List<DiaryEntry>();
        }
        
        string json = File.ReadAllText(_entriesFilePath);
        return JsonSerializer.Deserialize<List<DiaryEntry>>(json) ?? new List<DiaryEntry>();
    }
    
    private void SaveEntries(List<DiaryEntry> entries)
    {
        string json = JsonSerializer.Serialize(entries);
        File.WriteAllText(_entriesFilePath, json);
    }
}

public class DiaryEntry
{
    public DateTime Date { get; set; }
    public string Content { get; set; }
}