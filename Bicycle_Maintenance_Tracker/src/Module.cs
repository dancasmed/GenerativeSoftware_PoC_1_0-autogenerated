using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BicycleMaintenanceTracker : IGeneratedModule
{
    public string Name { get; set; } = "Bicycle Maintenance Tracker";
    
    private string _dataFilePath;
    
    public BicycleMaintenanceTracker()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Bicycle Maintenance Tracker module is running");
        
        _dataFilePath = Path.Combine(dataFolder, "bicycle_maintenance.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<MaintenanceTask> tasks = LoadTasks();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTask(tasks);
                    break;
                case "2":
                    ViewTasks(tasks);
                    break;
                case "3":
                    CompleteTask(tasks);
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveTasks(tasks);
        
        return true;
    }
    
    private List<MaintenanceTask> LoadTasks()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<MaintenanceTask>>(json);
        }
        
        return new List<MaintenanceTask>();
    }
    
    private void SaveTasks(List<MaintenanceTask> tasks)
    {
        string json = JsonSerializer.Serialize(tasks);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nBicycle Maintenance Tracker");
        Console.WriteLine("1. Add maintenance task");
        Console.WriteLine("2. View tasks");
        Console.WriteLine("3. Mark task as completed");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddTask(List<MaintenanceTask> tasks)
    {
        Console.Write("Enter task description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter due date (yyyy-MM-dd) or leave empty: ");
        string dateInput = Console.ReadLine();
        
        DateTime? dueDate = null;
        if (!string.IsNullOrEmpty(dateInput) && DateTime.TryParse(dateInput, out DateTime parsedDate))
        {
            dueDate = parsedDate;
        }
        
        tasks.Add(new MaintenanceTask
        {
            Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1,
            Description = description,
            DueDate = dueDate,
            IsCompleted = false,
            CreatedDate = DateTime.Now
        });
        
        Console.WriteLine("Task added successfully.");
    }
    
    private void ViewTasks(List<MaintenanceTask> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No maintenance tasks found.");
            return;
        }
        
        Console.WriteLine("\nMaintenance Tasks:");
        foreach (var task in tasks)
        {
            string status = task.IsCompleted ? "[Completed]" : "[Pending]";
            string dueDate = task.DueDate.HasValue ? task.DueDate.Value.ToString("yyyy-MM-dd") : "No due date";
            Console.WriteLine($"{task.Id}. {status} {task.Description} (Due: {dueDate}, Created: {task.CreatedDate:yyyy-MM-dd})");
        }
    }
    
    private void CompleteTask(List<MaintenanceTask> tasks)
    {
        ViewTasks(tasks);
        
        if (tasks.Count == 0) return;
        
        Console.Write("Enter task ID to mark as completed: ");
        if (int.TryParse(Console.ReadLine(), out int taskId))
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.IsCompleted = true;
                Console.WriteLine("Task marked as completed.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid task ID.");
        }
    }
}

public class MaintenanceTask
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }
}