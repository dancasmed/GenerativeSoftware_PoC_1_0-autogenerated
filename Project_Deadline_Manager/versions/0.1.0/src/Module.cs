using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ProjectDeadlineManager : IGeneratedModule
{
    public string Name { get; set; } = "Project Deadline Manager";

    private string _dataFilePath;
    private List<Project> _projects;

    public ProjectDeadlineManager()
    {
        _projects = new List<Project>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Project Deadline Manager module is running.");
        _dataFilePath = Path.Combine(dataFolder, "projects.json");

        LoadProjects();

        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddProject();
                    break;
                case "2":
                    ListProjects();
                    break;
                case "3":
                    UpdateProject();
                    break;
                case "4":
                    DeleteProject();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveProjects();
        Console.WriteLine("Project Deadline Manager module finished.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nProject Deadline Manager");
        Console.WriteLine("1. Add Project");
        Console.WriteLine("2. List Projects");
        Console.WriteLine("3. Update Project");
        Console.WriteLine("4. Delete Project");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddProject()
    {
        Console.Write("Enter project name: ");
        string name = Console.ReadLine();

        Console.Write("Enter project description: ");
        string description = Console.ReadLine();

        Console.Write("Enter deadline (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime deadline))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        var project = new Project
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            Deadline = deadline,
            Milestones = new List<Milestone>()
        };

        _projects.Add(project);
        Console.WriteLine("Project added successfully.");
    }

    private void ListProjects()
    {
        if (_projects.Count == 0)
        {
            Console.WriteLine("No projects found.");
            return;
        }

        foreach (var project in _projects)
        {
            Console.WriteLine("\nProject: " + project.Name);
            Console.WriteLine("Description: " + project.Description);
            Console.WriteLine("Deadline: " + project.Deadline.ToString("yyyy-MM-dd"));
            Console.WriteLine("Milestones:");

            if (project.Milestones.Count == 0)
            {
                Console.WriteLine("  No milestones.");
            }
            else
            {
                foreach (var milestone in project.Milestones)
                {
                    Console.WriteLine("  - " + milestone.Name + " (Due: " + milestone.DueDate.ToString("yyyy-MM-dd") + ")");
                }
            }
        }
    }

    private void UpdateProject()
    {
        ListProjects();
        if (_projects.Count == 0) return;

        Console.Write("Enter project ID to update: ");
        string id = Console.ReadLine();

        var project = _projects.Find(p => p.Id == id);
        if (project == null)
        {
            Console.WriteLine("Project not found.");
            return;
        }

        Console.Write("Enter new project name (leave empty to keep current): ");
        string name = Console.ReadLine();
        if (!string.IsNullOrEmpty(name))
        {
            project.Name = name;
        }

        Console.Write("Enter new project description (leave empty to keep current): ");
        string description = Console.ReadLine();
        if (!string.IsNullOrEmpty(description))
        {
            project.Description = description;
        }

        Console.Write("Enter new deadline (yyyy-MM-dd, leave empty to keep current): ");
        string dateInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(dateInput) && DateTime.TryParse(dateInput, out DateTime newDeadline))
        {
            project.Deadline = newDeadline;
        }

        Console.WriteLine("Project updated successfully.");
    }

    private void DeleteProject()
    {
        ListProjects();
        if (_projects.Count == 0) return;

        Console.Write("Enter project ID to delete: ");
        string id = Console.ReadLine();

        var project = _projects.Find(p => p.Id == id);
        if (project == null)
        {
            Console.WriteLine("Project not found.");
            return;
        }

        _projects.Remove(project);
        Console.WriteLine("Project deleted successfully.");
    }

    private void LoadProjects()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _projects = JsonSerializer.Deserialize<List<Project>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading projects: " + ex.Message);
            }
        }
    }

    private void SaveProjects()
    {
        try
        {
            string json = JsonSerializer.Serialize(_projects);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving projects: " + ex.Message);
        }
    }
}

public class Project
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public List<Milestone> Milestones { get; set; }
}

public class Milestone
{
    public string Name { get; set; }
    public DateTime DueDate { get; set; }
}