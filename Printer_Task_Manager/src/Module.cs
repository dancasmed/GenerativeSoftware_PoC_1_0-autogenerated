using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class PrinterTaskManager : IGeneratedModule
{
    public string Name { get; set; } = "Printer Task Manager";

    private string _dataFilePath;

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "printer_tasks.json");
        
        Console.WriteLine("Printer Task Manager module is running.");
        Console.WriteLine("Loading tasks from: " + _dataFilePath);

        List<PrinterTask> tasks = LoadTasks();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Process Next Task");
            Console.WriteLine("3. List All Tasks");
            Console.WriteLine("4. Exit Module");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTask(tasks);
                    break;
                case "2":
                    ProcessNextTask(tasks);
                    break;
                case "3":
                    ListAllTasks(tasks);
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveTasks(tasks);
        }
        
        Console.WriteLine("Printer Task Manager module is exiting.");
        return true;
    }
    
    private List<PrinterTask> LoadTasks()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<PrinterTask>>(json) ?? new List<PrinterTask>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
        }
        
        return new List<PrinterTask>();
    }
    
    private void SaveTasks(List<PrinterTask> tasks)
    {
        try
        {
            string json = JsonSerializer.Serialize(tasks);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving tasks: " + ex.Message);
        }
    }
    
    private void AddTask(List<PrinterTask> tasks)
    {
        Console.Write("Enter task description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter priority (1-High, 2-Medium, 3-Low): ");
        if (!int.TryParse(Console.ReadLine(), out int priority) || priority < 1 || priority > 3)
        {
            Console.WriteLine("Invalid priority. Using default Medium priority.");
            priority = 2;
        }
        
        tasks.Add(new PrinterTask
        {
            Id = Guid.NewGuid(),
            Description = description,
            Priority = (PriorityLevel)priority,
            CreatedAt = DateTime.Now
        });
        
        Console.WriteLine("Task added successfully.");
    }
    
    private void ProcessNextTask(List<PrinterTask> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks in the queue.");
            return;
        }
        
        var nextTask = tasks
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .First();
        
        Console.WriteLine("Processing task:");
        Console.WriteLine("ID: " + nextTask.Id);
        Console.WriteLine("Description: " + nextTask.Description);
        Console.WriteLine("Priority: " + nextTask.Priority);
        Console.WriteLine("Created At: " + nextTask.CreatedAt);
        
        tasks.Remove(nextTask);
        Console.WriteLine("Task processed and removed from queue.");
    }
    
    private void ListAllTasks(List<PrinterTask> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks in the queue.");
            return;
        }
        
        Console.WriteLine("Current Tasks in Queue:");
        foreach (var task in tasks.OrderBy(t => t.Priority).ThenBy(t => t.CreatedAt))
        {
            Console.WriteLine("ID: " + task.Id);
            Console.WriteLine("Description: " + task.Description);
            Console.WriteLine("Priority: " + task.Priority);
            Console.WriteLine("Created At: " + task.CreatedAt);
            Console.WriteLine("-----");
        }
    }
}

public class PrinterTask
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum PriorityLevel
{
    High = 1,
    Medium = 2,
    Low = 3
}