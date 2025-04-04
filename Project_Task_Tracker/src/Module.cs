using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ProjectTaskTracker : IGeneratedModule
{
    public string Name { get; set; } = "Project Task Tracker";

    private string tasksFilePath;
    private string membersFilePath;

    private List<ProjectTask> tasks;
    private List<TeamMember> teamMembers;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Project Task Tracker...");
        
        tasksFilePath = Path.Combine(dataFolder, "tasks.json");
        membersFilePath = Path.Combine(dataFolder, "members.json");

        LoadData();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    AddTeamMember();
                    break;
                case "3":
                    AssignTask();
                    break;
                case "4":
                    ViewTasks();
                    break;
                case "5":
                    SaveData();
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Project Task Tracker has finished its execution.");
        return true;
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(tasksFilePath))
            {
                string json = File.ReadAllText(tasksFilePath);
                tasks = JsonSerializer.Deserialize<List<ProjectTask>>(json);
            }
            else
            {
                tasks = new List<ProjectTask>();
            }

            if (File.Exists(membersFilePath))
            {
                string json = File.ReadAllText(membersFilePath);
                teamMembers = JsonSerializer.Deserialize<List<TeamMember>>(json);
            }
            else
            {
                teamMembers = new List<TeamMember>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data: " + ex.Message);
            tasks = new List<ProjectTask>();
            teamMembers = new List<TeamMember>();
        }
    }

    private void SaveData()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tasksFilePath));
            
            string tasksJson = JsonSerializer.Serialize(tasks);
            File.WriteAllText(tasksFilePath, tasksJson);
            
            string membersJson = JsonSerializer.Serialize(teamMembers);
            File.WriteAllText(membersFilePath, membersJson);
            
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving data: " + ex.Message);
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nProject Task Tracker Menu:");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. Add Team Member");
        Console.WriteLine("3. Assign Task to Member");
        Console.WriteLine("4. View All Tasks");
        Console.WriteLine("5. Exit and Save");
        Console.Write("Select an option: ");
    }

    private void AddTask()
    {
        Console.Write("Enter task name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter task description: ");
        string description = Console.ReadLine();
        
        tasks.Add(new ProjectTask
        {
            Id = tasks.Count + 1,
            Name = name,
            Description = description,
            Status = "Pending"
        });
        
        Console.WriteLine("Task added successfully.");
    }

    private void AddTeamMember()
    {
        Console.Write("Enter member name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter member email: ");
        string email = Console.ReadLine();
        
        teamMembers.Add(new TeamMember
        {
            Id = teamMembers.Count + 1,
            Name = name,
            Email = email
        });
        
        Console.WriteLine("Team member added successfully.");
    }

    private void AssignTask()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available to assign.");
            return;
        }
        
        if (teamMembers.Count == 0)
        {
            Console.WriteLine("No team members available.");
            return;
        }
        
        ViewTasks();
        Console.Write("Enter task ID to assign: ");
        if (!int.TryParse(Console.ReadLine(), out int taskId))
        {
            Console.WriteLine("Invalid task ID.");
            return;
        }
        
        var task = tasks.Find(t => t.Id == taskId);
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }
        
        Console.WriteLine("Available team members:");
        foreach (var member in teamMembers)
        {
            Console.WriteLine(member.Id + ". " + member.Name);
        }
        
        Console.Write("Enter member ID to assign: ");
        if (!int.TryParse(Console.ReadLine(), out int memberId))
        {
            Console.WriteLine("Invalid member ID.");
            return;
        }
        
        var member = teamMembers.Find(m => m.Id == memberId);
        if (member == null)
        {
            Console.WriteLine("Member not found.");
            return;
        }
        
        task.AssignedMemberId = memberId;
        task.Status = "Assigned";
        
        Console.WriteLine("Task assigned successfully to " + member.Name);
    }

    private void ViewTasks()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available.");
            return;
        }
        
        Console.WriteLine("\nCurrent Tasks:");
        foreach (var task in tasks)
        {
            string assignedTo = "Unassigned";
            if (task.AssignedMemberId.HasValue)
            {
                var member = teamMembers.Find(m => m.Id == task.AssignedMemberId.Value);
                assignedTo = member != null ? member.Name : "Unknown";
            }
            
            Console.WriteLine($"ID: {task.Id}, Name: {task.Name}, Status: {task.Status}, Assigned To: {assignedTo}");
        }
    }
}

public class ProjectTask
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public int? AssignedMemberId { get; set; }
}

public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}