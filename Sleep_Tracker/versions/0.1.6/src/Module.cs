using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SleepTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Sleep Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Sleep Tracker Module is running...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        _dataFilePath = Path.Combine(dataFolder, "sleep_data.json");
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    RecordSleepSession();
                    break;
                case "2":
                    ViewSleepStatistics();
                    break;
                case "3":
                    exitRequested = true;
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
        Console.WriteLine("\nSleep Tracker Menu:");
        Console.WriteLine("1. Record Sleep Session");
        Console.WriteLine("2. View Sleep Statistics");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
    }
    
    private void RecordSleepSession()
    {
        Console.WriteLine("\nRecording Sleep Session");
        
        Console.Write("Enter sleep date (yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();
        
        Console.Write("Enter sleep time (HH:mm): ");
        var sleepTimeInput = Console.ReadLine();
        
        Console.Write("Enter wake time (HH:mm): ");
        var wakeTimeInput = Console.ReadLine();
        
        Console.Write("Enter sleep quality (1-5): ");
        var qualityInput = Console.ReadLine();
        
        if (!DateTime.TryParse(dateInput, out var date) ||
            !TimeSpan.TryParse(sleepTimeInput, out var sleepTime) ||
            !TimeSpan.TryParse(wakeTimeInput, out var wakeTime) ||
            !int.TryParse(qualityInput, out var quality) || quality < 1 || quality > 5)
        {
            Console.WriteLine("Invalid input. Recording cancelled.");
            return;
        }
        
        var sleepData = LoadSleepData();
        
        sleepData.Add(new SleepRecord
        {
            Date = date,
            SleepTime = sleepTime,
            WakeTime = wakeTime,
            Duration = CalculateDuration(sleepTime, wakeTime),
            Quality = quality
        });
        
        SaveSleepData(sleepData);
        Console.WriteLine("Sleep session recorded successfully.");
    }
    
    private TimeSpan CalculateDuration(TimeSpan sleepTime, TimeSpan wakeTime)
    {
        if (wakeTime > sleepTime)
        {
            return wakeTime - sleepTime;
        }
        else
        {
            return TimeSpan.FromHours(24) - sleepTime + wakeTime;
        }
    }
    
    private void ViewSleepStatistics()
    {
        var sleepData = LoadSleepData();
        
        if (sleepData.Count == 0)
        {
            Console.WriteLine("No sleep data available.");
            return;
        }
        
        Console.WriteLine("\nSleep Statistics:");
        Console.WriteLine("Total records: " + sleepData.Count);
        
        TimeSpan totalDuration = TimeSpan.Zero;
        int totalQuality = 0;
        
        foreach (var record in sleepData)
        {
            totalDuration += record.Duration;
            totalQuality += record.Quality;
        }
        
        var avgDuration = totalDuration.TotalHours / sleepData.Count;
        var avgQuality = (double)totalQuality / sleepData.Count;
        
        Console.WriteLine("Average sleep duration: " + avgDuration.ToString("F2") + " hours");
        Console.WriteLine("Average sleep quality: " + avgQuality.ToString("F1") + " / 5");
        
        Console.WriteLine("\nRecent Sleep Sessions:");
        var recentRecords = sleepData.Count > 5 ? sleepData.GetRange(sleepData.Count - 5, 5) : sleepData;
        
        foreach (var record in recentRecords)
        {
            Console.WriteLine($"{record.Date:yyyy-MM-dd}: {record.SleepTime:hh\:mm} to {record.WakeTime:hh\:mm} ({record.Duration.TotalHours:F2} hrs), Quality: {record.Quality}/5");
        }
    }
    
    private List<SleepRecord> LoadSleepData()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<SleepRecord>();
        }
        
        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<SleepRecord>>(json) ?? new List<SleepRecord>();
    }
    
    private void SaveSleepData(List<SleepRecord> data)
    {
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(_dataFilePath, json);
    }
}

public class SleepRecord
{
    public DateTime Date { get; set; }
    public TimeSpan SleepTime { get; set; }
    public TimeSpan WakeTime { get; set; }
    public TimeSpan Duration { get; set; }
    public int Quality { get; set; }
}