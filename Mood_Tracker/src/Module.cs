using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MoodTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Mood Tracker";

    private string moodDataPath;
    private const string MoodDataFileName = "mood_entries.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Mood Tracker Module is running...");
        
        moodDataPath = Path.Combine(dataFolder, MoodDataFileName);
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add today's mood");
            Console.WriteLine("2. View weekly summary");
            Console.WriteLine("3. Exit module");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddMoodEntry();
                    break;
                case "2":
                    GenerateWeeklySummary();
                    break;
                case "3":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void AddMoodEntry()
    {
        Console.WriteLine("\nRate your mood today (1-5, where 1 is very bad and 5 is excellent):");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
        {
            Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
            return;
        }
        
        Console.WriteLine("Add a note about your mood (optional):");
        string note = Console.ReadLine();
        
        MoodEntry newEntry = new MoodEntry
        {
            Date = DateTime.Today,
            Rating = rating,
            Note = note
        };
        
        List<MoodEntry> entries = LoadMoodEntries();
        entries.Add(newEntry);
        SaveMoodEntries(entries);
        
        Console.WriteLine("Mood entry saved successfully!");
    }
    
    private void GenerateWeeklySummary()
    {
        List<MoodEntry> entries = LoadMoodEntries();
        DateTime oneWeekAgo = DateTime.Today.AddDays(-7);
        
        var weeklyEntries = entries.FindAll(e => e.Date >= oneWeekAgo);
        
        if (weeklyEntries.Count == 0)
        {
            Console.WriteLine("No mood entries found for the past week.");
            return;
        }
        
        double averageRating = 0;
        foreach (var entry in weeklyEntries)
        {
            averageRating += entry.Rating;
        }
        averageRating /= weeklyEntries.Count;
        
        Console.WriteLine("\nWeekly Mood Summary:");
        Console.WriteLine("------------------------");
        Console.WriteLine("Entries count: " + weeklyEntries.Count);
        Console.WriteLine("Average mood rating: " + averageRating.ToString("0.0"));
        Console.WriteLine("------------------------");
        
        foreach (var entry in weeklyEntries)
        {
            Console.WriteLine(entry.Date.ToString("yyyy-MM-dd") + ": " + entry.Rating + "/5");
            if (!string.IsNullOrEmpty(entry.Note))
            {
                Console.WriteLine("   Note: " + entry.Note);
            }
        }
    }
    
    private List<MoodEntry> LoadMoodEntries()
    {
        if (!File.Exists(moodDataPath))
        {
            return new List<MoodEntry>();
        }
        
        string json = File.ReadAllText(moodDataPath);
        return JsonSerializer.Deserialize<List<MoodEntry>>(json);
    }
    
    private void SaveMoodEntries(List<MoodEntry> entries)
    {
        string json = JsonSerializer.Serialize(entries);
        File.WriteAllText(moodDataPath, json);
    }
}

public class MoodEntry
{
    public DateTime Date { get; set; }
    public int Rating { get; set; }
    public string Note { get; set; }
}