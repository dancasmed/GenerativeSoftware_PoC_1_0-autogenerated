using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MoodTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Mood Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Mood Tracker Module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "mood_entries.json");
        
        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add mood entry");
            Console.WriteLine("2. View mood history");
            Console.WriteLine("3. Exit module");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            if (input == "1")
            {
                AddMoodEntry();
            }
            else if (input == "2")
            {
                ViewMoodHistory();
            }
            else if (input == "3")
            {
                Console.WriteLine("Exiting Mood Tracker...");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }
    
    private void AddMoodEntry()
    {
        Console.Write("Enter today's date (yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();
        
        if (!DateTime.TryParse(dateInput, out var date))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
            return;
        }
        
        Console.Write("How are you feeling today (1-5 where 1=Very Bad, 5=Excellent)? ");
        var moodInput = Console.ReadLine();
        
        if (!int.TryParse(moodInput, out var moodRating) || moodRating < 1 || moodRating > 5)
        {
            Console.WriteLine("Invalid mood rating. Please enter a number between 1 and 5.");
            return;
        }
        
        Console.Write("Add a journal entry (optional): ");
        var journalEntry = Console.ReadLine();
        
        var entries = LoadEntries();
        
        entries.Add(new MoodEntry
        {
            Date = date,
            MoodRating = moodRating,
            JournalEntry = journalEntry
        });
        
        SaveEntries(entries);
        
        Console.WriteLine("Mood entry saved successfully!");
    }
    
    private void ViewMoodHistory()
    {
        var entries = LoadEntries();
        
        if (entries.Count == 0)
        {
            Console.WriteLine("No mood entries found.");
            return;
        }
        
        Console.WriteLine("\nMood History:");
        Console.WriteLine("-------------");
        
        foreach (var entry in entries)
        {
            Console.WriteLine($"Date: {entry.Date:yyyy-MM-dd}");
            Console.WriteLine($"Mood: {GetMoodDescription(entry.MoodRating)}");
            
            if (!string.IsNullOrEmpty(entry.JournalEntry))
            {
                Console.WriteLine($"Journal: {entry.JournalEntry}");
            }
            
            Console.WriteLine();
        }
    }
    
    private List<MoodEntry> LoadEntries()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<MoodEntry>();
        }
        
        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<MoodEntry>>(json) ?? new List<MoodEntry>();
    }
    
    private void SaveEntries(List<MoodEntry> entries)
    {
        var json = JsonSerializer.Serialize(entries);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private string GetMoodDescription(int rating)
    {
        return rating switch
        {
            1 => "Very Bad",
            2 => "Bad",
            3 => "Neutral",
            4 => "Good",
            5 => "Excellent",
            _ => "Unknown"
        };
    }
}

public class MoodEntry
{
    public DateTime Date { get; set; }
    public int MoodRating { get; set; }
    public string JournalEntry { get; set; }
}