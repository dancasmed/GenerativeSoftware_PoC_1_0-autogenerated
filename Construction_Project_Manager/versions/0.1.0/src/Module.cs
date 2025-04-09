using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ConstructionProjectManager : IGeneratedModule
{
    public string Name { get; set; } = "Construction Project Manager";

    private string projectsFilePath;
    private string budgetsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Construction Project Manager...");

        projectsFilePath = Path.Combine(dataFolder, "projects.json");
        budgetsFilePath = Path.Combine(dataFolder, "budgets.json");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        if (!File.Exists(projectsFilePath))
        {
            File.WriteAllText(projectsFilePath, "[]");
        }

        if (!File.Exists(budgetsFilePath))
        {
            File.WriteAllText(budgetsFilePath, "[]");
        }

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nConstruction Project Manager");
            Console.WriteLine("1. Add Project");
            Console.WriteLine("2. List Projects");
            Console.WriteLine("3. Add Budget");
            Console.WriteLine("4. List Budgets");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

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
                    AddBudget();
                    break;
                case "4":
                    ListBudgets();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Construction Project Manager is shutting down...");
        return true;
    }

    private void AddProject()
    {
        Console.Write("Enter project name: ");
        string name = Console.ReadLine();

        Console.Write("Enter project location: ");
        string location = Console.ReadLine();

        Console.Write("Enter project start date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        Console.Write("Enter project end date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }


        var projects = LoadProjects();
        projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Location = location,
            StartDate = startDate,
            EndDate = endDate
        });

        SaveProjects(projects);
        Console.WriteLine("Project added successfully.");
    }

    private void ListProjects()
    {
        var projects = LoadProjects();

        Console.WriteLine("\nProjects List:");
        foreach (var project in projects)
        {
            Console.WriteLine($"ID: {project.Id}");
            Console.WriteLine($"Name: {project.Name}");
            Console.WriteLine($"Location: {project.Location}");
            Console.WriteLine($"Start Date: {project.StartDate:yyyy-MM-dd}");
            Console.WriteLine($"End Date: {project.EndDate:yyyy-MM-dd}");
            Console.WriteLine();
        }
    }

    private void AddBudget()
    {
        var projects = LoadProjects();
        if (projects.Count == 0)
        {
            Console.WriteLine("No projects available. Please add a project first.");
            return;
        }

        ListProjects();
        Console.Write("Enter project ID for the budget: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid projectId))
        {
            Console.WriteLine("Invalid project ID.");
            return;
        }

        var project = projects.Find(p => p.Id == projectId);
        if (project == null)
        {
            Console.WriteLine("Project not found.");
            return;
        }

        Console.Write("Enter budget amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        Console.Write("Enter budget description: ");
        string description = Console.ReadLine();

        var budgets = LoadBudgets();
        budgets.Add(new Budget
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Amount = amount,
            Description = description,
            CreatedDate = DateTime.Now
        });

        SaveBudgets(budgets);
        Console.WriteLine("Budget added successfully.");
    }

    private void ListBudgets()
    {
        var budgets = LoadBudgets();
        var projects = LoadProjects();

        Console.WriteLine("\nBudgets List:");
        foreach (var budget in budgets)
        {
            var project = projects.Find(p => p.Id == budget.ProjectId);
            string projectName = project != null ? project.Name : "Unknown Project";

            Console.WriteLine($"ID: {budget.Id}");
            Console.WriteLine($"Project: {projectName}");
            Console.WriteLine($"Amount: {budget.Amount:C}");
            Console.WriteLine($"Description: {budget.Description}");
            Console.WriteLine($"Created Date: {budget.CreatedDate:yyyy-MM-dd}");
            Console.WriteLine();
        }
    }

    private List<Project> LoadProjects()
    {
        string json = File.ReadAllText(projectsFilePath);
        return JsonSerializer.Deserialize<List<Project>>(json) ?? new List<Project>();
    }

    private void SaveProjects(List<Project> projects)
    {
        string json = JsonSerializer.Serialize(projects);
        File.WriteAllText(projectsFilePath, json);
    }

    private List<Budget> LoadBudgets()
    {
        string json = File.ReadAllText(budgetsFilePath);
        return JsonSerializer.Deserialize<List<Budget>>(json) ?? new List<Budget>();
    }

    private void SaveBudgets(List<Budget> budgets)
    {
        string json = JsonSerializer.Serialize(budgets);
        File.WriteAllText(budgetsFilePath, json);
    }
}

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class Budget
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
}