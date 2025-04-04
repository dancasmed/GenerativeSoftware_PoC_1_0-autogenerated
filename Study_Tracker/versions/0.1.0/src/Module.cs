using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class StudyTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Study Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Study Tracker Module is running...");
            
            _dataFilePath = Path.Combine(dataFolder, "study_data.json");
            
            while (true)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Log study hours");
                Console.WriteLine("2. View weekly summary");
                Console.WriteLine("3. Exit module");
                Console.Write("Select an option: ");
                
                var input = Console.ReadLine();
                
                if (input == "1")
                {
                    LogStudyHours();
                }
                else if (input == "2")
                {
                    ShowWeeklySummary();
                }
                else if (input == "3")
                {
                    Console.WriteLine("Exiting Study Tracker module.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LogStudyHours()
    {
        Console.Write("Enter subject name: ");
        var subject = Console.ReadLine();
        
        Console.Write("Enter hours studied: ");
        if (!double.TryParse(Console.ReadLine(), out var hours) || hours <= 0)
        {
            Console.WriteLine("Invalid hours entered. Please enter a positive number.");
            return;
        }
        
        var now = DateTime.Now;
        var weekStart = now.AddDays(-(int)now.DayOfWeek);
        var weekEnd = weekStart.AddDays(7);
        
        var allData = LoadData();
        
        var weeklyData = allData.FirstOrDefault(d => 
            d.WeekStart == weekStart && d.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase));
        
        if (weeklyData == null)
        {
            weeklyData = new WeeklyStudyData
            {
                Subject = subject,
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                TotalHours = 0
            };
            allData.Add(weeklyData);
        }
        
        weeklyData.TotalHours += hours;
        weeklyData.Sessions.Add(new StudySession
        {
            Date = now,
            Hours = hours
        });
        
        SaveData(allData);
        Console.WriteLine("Study hours logged successfully.");
    }
    
    private void ShowWeeklySummary()
    {
        var allData = LoadData();
        var currentWeekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
        
        var weeklySummaries = allData
            .Where(d => d.WeekStart == currentWeekStart)
            .OrderByDescending(d => d.TotalHours)
            .ToList();
        
        if (weeklySummaries.Count == 0)
        {
            Console.WriteLine("No study data available for this week.");
            return;
        }
        
        Console.WriteLine("\nWeekly Study Summary:");
        Console.WriteLine("----------------------");
        
        foreach (var summary in weeklySummaries)
        {
            Console.WriteLine("Subject: " + summary.Subject);
            Console.WriteLine("Total Hours: " + summary.TotalHours);
            Console.WriteLine("Number of Sessions: " + summary.Sessions.Count);
            Console.WriteLine("Average per Session: " + (summary.TotalHours / summary.Sessions.Count).ToString("0.00"));
            Console.WriteLine("----------------------");
        }
    }
    
    private List<WeeklyStudyData> LoadData()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<WeeklyStudyData>();
        }
        
        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<WeeklyStudyData>>(json) ?? new List<WeeklyStudyData>();
    }
    
    private void SaveData(List<WeeklyStudyData> data)
    {
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(_dataFilePath, json);
    }
}

public class WeeklyStudyData
{
    public string Subject { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public double TotalHours { get; set; }
    public List<StudySession> Sessions { get; set; } = new List<StudySession>();
}

public class StudySession
{
    public DateTime Date { get; set; }
    public double Hours { get; set; }
}