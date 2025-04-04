using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BugTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Bug Tracker Module";

    private string _bugsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Bug Tracker Module...");
        
        _bugsFilePath = Path.Combine(dataFolder, "bugs.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddBug();
                    break;
                case "2":
                    ViewBugs();
                    break;
                case "3":
                    UpdateBugStatus();
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Bug Tracker Module completed successfully.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nBug Tracker Menu:");
        Console.WriteLine("1. Add a new bug");
        Console.WriteLine("2. View all bugs");
        Console.WriteLine("3. Update bug status");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddBug()
    {
        Console.Write("Enter bug description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter severity (Low/Medium/High/Critical): ");
        string severity = Console.ReadLine();
        
        var bug = new Bug
        {
            Id = Guid.NewGuid(),
            Description = description,
            Severity = severity,
            Status = "Open",
            ReportedDate = DateTime.Now
        };
        
        List<Bug> bugs = LoadBugs();
        bugs.Add(bug);
        SaveBugs(bugs);
        
        Console.WriteLine("Bug added successfully.");
    }

    private void ViewBugs()
    {
        List<Bug> bugs = LoadBugs();
        
        if (bugs.Count == 0)
        {
            Console.WriteLine("No bugs found.");
            return;
        }
        
        Console.WriteLine("\nList of Bugs:");
        foreach (var bug in bugs)
        {
            Console.WriteLine("ID: " + bug.Id);
            Console.WriteLine("Description: " + bug.Description);
            Console.WriteLine("Severity: " + bug.Severity);
            Console.WriteLine("Status: " + bug.Status);
            Console.WriteLine("Reported Date: " + bug.ReportedDate);
            Console.WriteLine("----------------------------");
        }
    }

    private void UpdateBugStatus()
    {
        ViewBugs();
        
        Console.Write("Enter the ID of the bug to update: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid bugId))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }
        
        List<Bug> bugs = LoadBugs();
        Bug bugToUpdate = bugs.Find(b => b.Id == bugId);
        
        if (bugToUpdate == null)
        {
            Console.WriteLine("Bug not found.");
            return;
        }
        
        Console.Write("Enter new status (Open/In Progress/Resolved/Closed): ");
        string newStatus = Console.ReadLine();
        
        bugToUpdate.Status = newStatus;
        SaveBugs(bugs);
        
        Console.WriteLine("Bug status updated successfully.");
    }

    private List<Bug> LoadBugs()
    {
        if (!File.Exists(_bugsFilePath))
        {
            return new List<Bug>();
        }
        
        string json = File.ReadAllText(_bugsFilePath);
        return JsonSerializer.Deserialize<List<Bug>>(json) ?? new List<Bug>();
    }

    private void SaveBugs(List<Bug> bugs)
    {
        string json = JsonSerializer.Serialize(bugs);
        File.WriteAllText(_bugsFilePath, json);
    }

    private class Bug
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public DateTime ReportedDate { get; set; }
    }
}