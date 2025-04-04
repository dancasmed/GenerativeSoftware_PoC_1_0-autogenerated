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
    private List<StudySession> _studySessions;

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Initializing Study Tracker module...");
            
            _dataFilePath = Path.Combine(dataFolder, "study_sessions.json");
            LoadStudySessions();
            
            bool continueRunning = true;
            while (continueRunning)
            {
                Console.WriteLine("\nStudy Tracker Menu:");
                Console.WriteLine("1. Add study session");
                Console.WriteLine("2. View weekly summary");
                Console.WriteLine("3. Exit module");
                Console.Write("Select an option: ");
                
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }
                
                switch (choice)
                {
                    case 1:
                        AddStudySession();
                        break;
                    case 2:
                        ShowWeeklySummary();
                        break;
                    case 3:
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveStudySessions();
            Console.WriteLine("Study Tracker module completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LoadStudySessions()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _studySessions = JsonSerializer.Deserialize<List<StudySession>>(json) ?? new List<StudySession>();
            }
            else
            {
                _studySessions = new List<StudySession>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading study sessions: " + ex.Message);
            _studySessions = new List<StudySession>();
        }
    }
    
    private void SaveStudySessions()
    {
        try
        {
            string json = JsonSerializer.Serialize(_studySessions);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving study sessions: " + ex.Message);
        }
    }
    
    private void AddStudySession()
    {
        try
        {
            Console.Write("Enter subject name: ");
            string subject = Console.ReadLine();
            
            Console.Write("Enter duration in hours: ");
            if (!double.TryParse(Console.ReadLine(), out double duration) || duration <= 0)
            {
                Console.WriteLine("Invalid duration. Please enter a positive number.");
                return;
            }
            
            var session = new StudySession
            {
                Subject = subject,
                DurationHours = duration,
                Date = DateTime.Now
            };
            
            _studySessions.Add(session);
            Console.WriteLine("Study session added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding study session: " + ex.Message);
        }
    }
    
    private void ShowWeeklySummary()
    {
        try
        {
            if (!_studySessions.Any())
            {
                Console.WriteLine("No study sessions recorded yet.");
                return;
            }
            
            var oneWeekAgo = DateTime.Now.AddDays(-7);
            var weeklySessions = _studySessions
                .Where(s => s.Date >= oneWeekAgo)
                .GroupBy(s => s.Subject)
                .OrderByDescending(g => g.Sum(s => s.DurationHours));
            
            Console.WriteLine("\nWeekly Study Summary:");
            Console.WriteLine("----------------------");
            
            foreach (var group in weeklySessions)
            {
                Console.WriteLine("Subject: " + group.Key);
                Console.WriteLine("Total Hours: " + group.Sum(s => s.DurationHours));
                Console.WriteLine("Average Hours/Day: " + group.Sum(s => s.DurationHours) / 7);
                Console.WriteLine("Sessions: " + group.Count());
                Console.WriteLine();
            }
            
            Console.WriteLine("Total study time this week: " + 
                weeklySessions.Sum(g => g.Sum(s => s.DurationHours)) + " hours");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating weekly summary: " + ex.Message);
        }
    }
}

public class StudySession
{
    public string Subject { get; set; }
    public double DurationHours { get; set; }
    public DateTime Date { get; set; }
}