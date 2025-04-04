using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TaskTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Task Tracker Module";

    private string _tasksFilePath;
    private string _membersFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Task Tracker Module...");

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

        Console.WriteLine("Task Tracker Module completed successfully.");
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
        Console.WriteLine("\nTask Tracker Menu:");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. Assign Task to Team Member");
        Console.WriteLine("3. View All Tasks");
        Console.WriteLine("4. Add Team Member");
        Console.WriteLine("5. View Team Members");
        Console.WriteLine("6. Exit");
        Console.Write("Select an option: ");
    }

    private void AddTask()
    {
        Console.Write("Enter task name: ");
        string name = Console.ReadLine();

        Console.Write("Enter task description: ");
        string description = Console.ReadLine();

        var tasks = LoadTasks();
        tasks.Add(new Task
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            AssignedMemberId = null,
            IsCompleted = false
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
            Console.WriteLine(i + 1 + ". " + tasks[i].Name);
        }

        Console.Write("Select task to assign: ");
        if (!int.TryParse(Console.ReadLine(), out int taskIndex) || taskIndex < 1 || taskIndex > tasks.Count)
        {
            Console.WriteLine("Invalid task selection.");
            return;
        }

        Console.WriteLine("\nAvailable Team Members:");
        for (int i = 0; i < members.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + members[i].Name);
        }

        Console.Write("Select team member to assign: ");
        if (!int.TryParse(Console.ReadLine(), out int memberIndex) || memberIndex < 1 || memberIndex > members.Count)
        {
            Console.WriteLine("Invalid member selection.");
            return;
        }

        tasks[taskIndex - 1].AssignedMemberId = members[memberIndex - 1].Id;
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

        Console.WriteLine("\nAll Tasks:");
        foreach (var task in tasks)
        {
            string assignedTo = "Unassigned";
            if (task.AssignedMemberId.HasValue)
            {
                var member = members.Find(m => m.Id == task.AssignedMemberId.Value);
                assignedTo = member != null ? member.Name : "Unknown";
            }

            Console.WriteLine($"Task: {task.Name}");
            Console.WriteLine($"Description: {task.Description}");
            Console.WriteLine($"Assigned To: {assignedTo}");
            Console.WriteLine($"Status: {(task.IsCompleted ? "Completed" : "Pending")}");
            Console.WriteLine("----------------------");
        }
    }

    private void AddTeamMember()
    {
        Console.Write("Enter team member name: ");
        string name = Console.ReadLine();

        Console.Write("Enter team member email: ");
        string email = Console.ReadLine();

        var members = LoadTeamMembers();
        members.Add(new TeamMember
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email
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
            Console.WriteLine($"Email: {member.Email}");
            Console.WriteLine("----------------------");
        }
    }

    private List<Task> LoadTasks()
    {
        string json = File.ReadAllText(_tasksFilePath);
        return JsonSerializer.Deserialize<List<Task>>(json) ?? new List<Task>();
    }

    private void SaveTasks(List<Task> tasks)
    {
        string json = JsonSerializer.Serialize(tasks);
        File.WriteAllText(_tasksFilePath, json);
    }

    private List<TeamMember> LoadTeamMembers()
    {
        string json = File.ReadAllText(_membersFilePath);
        return JsonSerializer.Deserialize<List<TeamMember>>(json) ?? new List<TeamMember>();
    }

    private void SaveTeamMembers(List<TeamMember> members)
    {
        string json = JsonSerializer.Serialize(members);
        File.WriteAllText(_membersFilePath, json);
    }

    private class Task
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? AssignedMemberId { get; set; }
        public bool IsCompleted { get; set; }
    }

    private class TeamMember
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}