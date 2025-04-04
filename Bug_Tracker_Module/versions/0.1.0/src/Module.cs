using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BugTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Bug Tracker Module";
    
    private string _bugsFilePath;
    
    public BugTrackerModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Bug Tracker Module is running");
        
        _bugsFilePath = Path.Combine(dataFolder, "bugs.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        var bugs = LoadBugs();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddBug(bugs);
                    break;
                case "2":
                    ViewBugs(bugs);
                    break;
                case "3":
                    UpdateBugStatus(bugs);
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveBugs(bugs);
        }
        
        return true;
    }
    
    private List<Bug> LoadBugs()
    {
        if (!File.Exists(_bugsFilePath))
        {
            return new List<Bug>();
        }
        
        var json = File.ReadAllText(_bugsFilePath);
        return JsonSerializer.Deserialize<List<Bug>>(json) ?? new List<Bug>();
    }
    
    private void SaveBugs(List<Bug> bugs)
    {
        var json = JsonSerializer.Serialize(bugs);
        File.WriteAllText(_bugsFilePath, json);
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nBug Tracker Menu:");
        Console.WriteLine("1. Add Bug");
        Console.WriteLine("2. View Bugs");
        Console.WriteLine("3. Update Bug Status");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddBug(List<Bug> bugs)
    {
        Console.Write("Enter bug description: ");
        var description = Console.ReadLine();
        
        Console.WriteLine("Select severity level:");
        Console.WriteLine("1. Low");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. High");
        Console.WriteLine("4. Critical");
        Console.Write("Enter severity level (1-4): ");
        
        if (!int.TryParse(Console.ReadLine(), out int severityLevel) || severityLevel < 1 || severityLevel > 4)
        {
            Console.WriteLine("Invalid severity level. Defaulting to Medium.");
            severityLevel = 2;
        }
        
        var severity = (BugSeverity)(severityLevel - 1);
        
        var bug = new Bug
        {
            Id = Guid.NewGuid(),
            Description = description,
            Severity = severity,
            Status = BugStatus.Open,
            CreatedDate = DateTime.Now
        };
        
        bugs.Add(bug);
        Console.WriteLine("Bug added successfully.");
    }
    
    private void ViewBugs(List<Bug> bugs)
    {
        if (bugs.Count == 0)
        {
            Console.WriteLine("No bugs found.");
            return;
        }
        
        Console.WriteLine("\nList of Bugs:");
        foreach (var bug in bugs)
        {
            Console.WriteLine($"ID: {bug.Id}");
            Console.WriteLine($"Description: {bug.Description}");
            Console.WriteLine($"Severity: {bug.Severity}");
            Console.WriteLine($"Status: {bug.Status}");
            Console.WriteLine($"Created: {bug.CreatedDate}");
            Console.WriteLine("----------------------------");
        }
    }
    
    private void UpdateBugStatus(List<Bug> bugs)
    {
        if (bugs.Count == 0)
        {
            Console.WriteLine("No bugs found to update.");
            return;
        }
        
        ViewBugs(bugs);
        
        Console.Write("Enter the ID of the bug to update: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid bugId))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }
        
        var bug = bugs.Find(b => b.Id == bugId);
        if (bug == null)
        {
            Console.WriteLine("Bug not found.");
            return;
        }
        
        Console.WriteLine("Select new status:");
        Console.WriteLine("1. Open");
        Console.WriteLine("2. In Progress");
        Console.WriteLine("3. Resolved");
        Console.WriteLine("4. Closed");
        Console.Write("Enter status (1-4): ");
        
        if (!int.TryParse(Console.ReadLine(), out int statusValue) || statusValue < 1 || statusValue > 4)
        {
            Console.WriteLine("Invalid status. No changes made.");
            return;
        }
        
        bug.Status = (BugStatus)(statusValue - 1);
        Console.WriteLine("Bug status updated successfully.");
    }
}

public class Bug
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public BugSeverity Severity { get; set; }
    public BugStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
}

public enum BugSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum BugStatus
{
    Open,
    InProgress,
    Resolved,
    Closed
}