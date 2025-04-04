using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ProjectTaskTracker : IGeneratedModule
{
    public string Name { get; set; } = "Project Task Tracker";

    private string _tasksFilePath;
    private string _membersFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Project Task Tracker module...");

        _tasksFilePath = Path.Combine(dataFolder, "tasks.json");
        _membersFilePath = Path.Combine(dataFolder, "members.json");

        EnsureDataFilesExist();

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
                    AssignTask();
                    break;
                case "3":
                    ViewTasks();
                    break;
                case "4":
                    AddTeamMember();
                    break;
                case "5":
                    ViewTeamMembers();
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Project Task Tracker module completed successfully.");
        return true;
    }

    private void EnsureDataFilesExist()
    {
        if (!File.Exists(_tasksFilePath))
        {
            File.WriteAllText(_tasksFilePath, "[]");
        }

        if (!File.Exists(_membersFilePath))
        {
            File.WriteAllText(_membersFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nProject Task Tracker Menu:");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. Assign Task to Team Member");
        Console.WriteLine("3. View All Tasks");
        Console.WriteLine("4. Add Team Member");
        Console.WriteLine("5. View Team Members");
        Console.WriteLine("6. Exit Module");
        Console.Write("Select an option: ");
    }

    private void AddTask()
    {
        Console.Write("Enter task description: ");
        var description = Console.ReadLine();

        Console.Write("Enter task priority (High/Medium/Low): ");
        var priority = Console.ReadLine();

        var tasks = LoadTasks();
        tasks.Add(new ProjectTask
        {
            Id = Guid.NewGuid().ToString(),
            Description = description,
            Priority = priority,
            Status = "Pending",
            AssignedMemberId = null
        });

        SaveTasks(tasks);
        Console.WriteLine("Task added successfully.");
    }

    private void AssignTask()
    {
        var tasks = LoadTasks();
        var members = LoadTeamMembers();

        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available to assign.");
            return;
        }

        if (members.Count == 0)
        {
            Console.WriteLine("No team members available to assign tasks to.");
            return;
        }

        Console.WriteLine("Available Tasks:");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i].Description} (Status: {tasks[i].Status})");
        }

        Console.Write("Select task number to assign: ");
        if (!int.TryParse(Console.ReadLine(), out int taskIndex) || taskIndex < 1 || taskIndex > tasks.Count)
        {
            Console.WriteLine("Invalid task selection.");
            return;
        }

        Console.WriteLine("\nAvailable Team Members:");
        for (int i = 0; i < members.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {members[i].Name}");
        }

        Console.Write("Select team member number: ");
        if (!int.TryParse(Console.ReadLine(), out int memberIndex) || memberIndex < 1 || memberIndex > members.Count)
        {
            Console.WriteLine("Invalid team member selection.");
            return;
        }

        tasks[taskIndex - 1].AssignedMemberId = members[memberIndex - 1].Id;
        tasks[taskIndex - 1].Status = "Assigned";

        SaveTasks(tasks);
        Console.WriteLine("Task assigned successfully.");
    }

    private void ViewTasks()
    {
        var tasks = LoadTasks();
        var members = LoadTeamMembers();

        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks available.");
            return;
        }

        Console.WriteLine("\nCurrent Tasks:");
        foreach (var task in tasks)
        {
            var memberName = "Unassigned";
            if (!string.IsNullOrEmpty(task.AssignedMemberId))
            {
                var member = members.Find(m => m.Id == task.AssignedMemberId);
                memberName = member?.Name ?? "Unknown";
            }

            Console.WriteLine($"Task: {task.Description}");
            Console.WriteLine($"Priority: {task.Priority}");
            Console.WriteLine($"Status: {task.Status}");
            Console.WriteLine($"Assigned to: {memberName}");
            Console.WriteLine("-----------------------");
        }
    }

    private void AddTeamMember()
    {
        Console.Write("Enter team member name: ");
        var name = Console.ReadLine();

        Console.Write("Enter team member role: ");
        var role = Console.ReadLine();

        var members = LoadTeamMembers();
        members.Add(new TeamMember
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Role = role
        });

        SaveTeamMembers(members);
        Console.WriteLine("Team member added successfully.");
    }

    private void ViewTeamMembers()
    {
        var members = LoadTeamMembers();

        if (members.Count == 0)
        {
            Console.WriteLine("No team members available.");
            return;
        }

        Console.WriteLine("\nTeam Members:");
        foreach (var member in members)
        {
            Console.WriteLine($"Name: {member.Name}");
            Console.WriteLine($"Role: {member.Role}");
            Console.WriteLine("-----------------------");
        }
    }

    private List<ProjectTask> LoadTasks()
    {
        var json = File.ReadAllText(_tasksFilePath);
        return JsonSerializer.Deserialize<List<ProjectTask>>(json) ?? new List<ProjectTask>();
    }

    private void SaveTasks(List<ProjectTask> tasks)
    {
        var json = JsonSerializer.Serialize(tasks);
        File.WriteAllText(_tasksFilePath, json);
    }

    private List<TeamMember> LoadTeamMembers()
    {
        var json = File.ReadAllText(_membersFilePath);
        return JsonSerializer.Deserialize<List<TeamMember>>(json) ?? new List<TeamMember>();
    }

    private void SaveTeamMembers(List<TeamMember> members)
    {
        var json = JsonSerializer.Serialize(members);
        File.WriteAllText(_membersFilePath, json);
    }
}

public class ProjectTask
{
    public string Id { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
    public string AssignedMemberId { get; set; }
}

public class TeamMember
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
}