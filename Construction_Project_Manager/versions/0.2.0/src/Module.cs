using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ConstructionProjectManager : IGeneratedModule
{
    public string Name { get; set; } = "Construction Project Manager";

    private string projectsFilePath;
    private List<Project> projects;

    public ConstructionProjectManager()
    {
        projects = new List<Project>();
    }

    public bool Main(string dataFolder)
    {
        projectsFilePath = Path.Combine(dataFolder, "projects.json");
        
        Console.WriteLine("Initializing Construction Project Manager...");
        
        try
        {
            LoadProjects();
            
            Console.WriteLine("Projects loaded successfully.");
            Console.WriteLine("Total projects: " + projects.Count);
            
            // Example operations
            if (projects.Count == 0)
            {
                AddSampleProjects();
            }
            
            DisplayProjectSummary();
            
            SaveProjects();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void LoadProjects()
    {
        if (File.Exists(projectsFilePath))
        {
            string json = File.ReadAllText(projectsFilePath);
            projects = JsonSerializer.Deserialize<List<Project>>(json);
        }
    }

    private void SaveProjects()
    {
        string json = JsonSerializer.Serialize(projects, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(projectsFilePath, json);
    }

    private void AddSampleProjects()
    {
        projects.Add(new Project
        {
            Id = 1,
            Name = "Downtown Office Building",
            Budget = 5000000,
            StartDate = new DateTime(2023, 1, 1),
            EndDate = new DateTime(2024, 6, 30),
            Status = ProjectStatus.InProgress
        });

        projects.Add(new Project
        {
            Id = 2,
            Name = "Suburban Housing Complex",
            Budget = 7500000,
            StartDate = new DateTime(2023, 3, 15),
            EndDate = new DateTime(2024, 12, 31),
            Status = ProjectStatus.Planned
        });
    }

    private void DisplayProjectSummary()
    {
        Console.WriteLine("\nProject Summary:");
        Console.WriteLine("----------------");
        
        foreach (var project in projects)
        {
            Console.WriteLine("Project: " + project.Name);
            Console.WriteLine("  Budget: " + project.Budget.ToString("C"));
            Console.WriteLine("  Status: " + project.Status);
            Console.WriteLine("  Timeline: " + project.StartDate.ToString("yyyy-MM-dd") + " to " + project.EndDate.ToString("yyyy-MM-dd"));
            Console.WriteLine();
        }
        
        decimal totalBudget = 0;
        foreach (var project in projects)
        {
            totalBudget += project.Budget;
        }
        
        Console.WriteLine("Total Budget Across All Projects: " + totalBudget.ToString("C"));
    }
}

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus Status { get; set; }
}

public enum ProjectStatus
{
    Planned,
    InProgress,
    Completed,
    OnHold,
    Cancelled
}