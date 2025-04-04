using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CodingProjectManager : IGeneratedModule
{
    public string Name { get; set; } = "Coding Project Manager";
    
    private string _projectsFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Coding Project Manager...");
        
        _projectsFilePath = Path.Combine(dataFolder, "projects.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<CodingProject> projects = LoadProjects();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddProject(projects);
                    break;
                case "2":
                    UpdateProjectProgress(projects);
                    break;
                case "3":
                    ListProjects(projects);
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveProjects(projects);
        }
        
        Console.WriteLine("Exiting Coding Project Manager...");
        return true;
    }
    
    private List<CodingProject> LoadProjects()
    {
        if (!File.Exists(_projectsFilePath))
        {
            return new List<CodingProject>();
        }
        
        string json = File.ReadAllText(_projectsFilePath);
        return JsonSerializer.Deserialize<List<CodingProject>>(json);
    }
    
    private void SaveProjects(List<CodingProject> projects)
    {
        string json = JsonSerializer.Serialize(projects);
        File.WriteAllText(_projectsFilePath, json);
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nCoding Project Manager");
        Console.WriteLine("1. Add new project");
        Console.WriteLine("2. Update project progress");
        Console.WriteLine("3. List all projects");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddProject(List<CodingProject> projects)
    {
        Console.Write("Enter project name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter project description: ");
        string description = Console.ReadLine();
        
        projects.Add(new CodingProject
        {
            Name = name,
            Description = description,
            Progress = 0,
            StartDate = DateTime.Now
        });
        
        Console.WriteLine("Project added successfully.");
    }
    
    private void UpdateProjectProgress(List<CodingProject> projects)
    {
        if (projects.Count == 0)
        {
            Console.WriteLine("No projects available.");
            return;
        }
        
        ListProjects(projects);
        
        Console.Write("Enter project number to update: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= projects.Count)
        {
            Console.Write("Enter new progress (0-100): ");
            if (int.TryParse(Console.ReadLine(), out int progress) && progress >= 0 && progress <= 100)
            {
                projects[index - 1].Progress = progress;
                projects[index - 1].LastUpdated = DateTime.Now;
                Console.WriteLine("Progress updated successfully.");
            }
            else
            {
                Console.WriteLine("Invalid progress value.");
            }
        }
        else
        {
            Console.WriteLine("Invalid project number.");
        }
    }
    
    private void ListProjects(List<CodingProject> projects)
    {
        if (projects.Count == 0)
        {
            Console.WriteLine("No projects available.");
            return;
        }
        
        Console.WriteLine("\nProjects List:");
        for (int i = 0; i < projects.Count; i++)
        {
            var project = projects[i];
            Console.WriteLine($"{i + 1}. {project.Name} - {project.Description}");
            Console.WriteLine($"   Progress: {project.Progress}%");
            Console.WriteLine($"   Started: {project.StartDate:yyyy-MM-dd}");
            if (project.LastUpdated.HasValue)
            {
                Console.WriteLine($"   Last Updated: {project.LastUpdated.Value:yyyy-MM-dd}");
            }
            Console.WriteLine();
        }
    }
}

public class CodingProject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Progress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? LastUpdated { get; set; }
}