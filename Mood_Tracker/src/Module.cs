using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MoodTracker : IGeneratedModule
{
    public string Name { get; set; } = "Mood Tracker";
    
    private string _dataFilePath;
    private const string DataFileName = "mood_entries.json";
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Mood Tracker module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, DataFileName);
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<MoodEntry> entries = LoadEntries();
        
        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add mood entry");
            Console.WriteLine("2. View weekly summary");
            Console.WriteLine("3. Exit module");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            if (input == "1")
            {
                AddMoodEntry(entries);
            }
            else if (input == "2")
            {
                GenerateWeeklySummary(entries);
            }
            else if (input == "3")
            {
                SaveEntries(entries);
                Console.WriteLine("Saving data and exiting Mood Tracker module.");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }
    
    private List<MoodEntry> LoadEntries()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<MoodEntry>>(json) ?? new List<MoodEntry>();
        }
        return new List<MoodEntry>();
    }
    
    private void SaveEntries(List<MoodEntry> entries)
    {
        string json = JsonSerializer.Serialize(entries);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddMoodEntry(List<MoodEntry> entries)
    {
        Console.Write("Enter your mood (1-5 where 1=Very Bad, 5=Excellent): ");
        if (int.TryParse(Console.ReadLine(), out int moodRating) && moodRating >= 1 && moodRating <= 5)
        {
            Console.Write("Enter optional notes: ");
            string notes = Console.ReadLine();
            
            entries.Add(new MoodEntry
            {
                Date = DateTime.Now,
                Rating = moodRating,
                Notes = notes
            });
            
            Console.WriteLine("Mood entry added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid mood rating. Please enter a number between 1 and 5.");
        }
    }
    
    private void GenerateWeeklySummary(List<MoodEntry> entries)
    {
        DateTime oneWeekAgo = DateTime.Now.AddDays(-7);
        var weeklyEntries = entries.FindAll(e => e.Date >= oneWeekAgo);
        
        if (weeklyEntries.Count == 0)
        {
            Console.WriteLine("No mood entries found for the past week.");
            return;
        }
        
        double average = weeklyEntries.Average(e => e.Rating);
        int bestDay = weeklyEntries.Max(e => e.Rating);
        int worstDay = weeklyEntries.Min(e => e.Rating);
        
        Console.WriteLine("\nWeekly Mood Summary:");
        Console.WriteLine("--------------------");
        Console.WriteLine("Average mood: " + average.ToString("0.0"));
        Console.WriteLine("Best day rating: " + bestDay);
        Console.WriteLine("Worst day rating: " + worstDay);
        Console.WriteLine("Total entries: " + weeklyEntries.Count);
        Console.WriteLine("\nRecent entries:");
        
        foreach (var entry in weeklyEntries.OrderByDescending(e => e.Date).Take(5))
        {
            Console.WriteLine($"{entry.Date:yyyy-MM-dd}: {entry.Rating} - {entry.Notes}");
        }
    }
}

public class MoodEntry
{
    public DateTime Date { get; set; }
    public int Rating { get; set; }
    public string Notes { get; set; }
}