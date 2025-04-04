using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PrinterTaskQueueModule : IGeneratedModule
{
    public string Name { get; set; } = "Printer Task Queue Manager";

    private string _dataFilePath;
    private List<PrinterTask> _tasks;

    public PrinterTaskQueueModule()
    {
        _tasks = new List<PrinterTask>();
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "printer_tasks.json");
        
        Console.WriteLine("Printer Task Queue Manager module is running.");
        Console.WriteLine("Loading tasks from " + _dataFilePath);
        
        LoadTasks();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. Process Next Task");
            Console.WriteLine("3. List All Tasks");
            Console.WriteLine("4. Save and Exit");
            Console.Write("Enter choice: ");
            
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    ProcessNextTask();
                    break;
                case "3":
                    ListAllTasks();
                    break;
                case "4":
                    SaveTasks();
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Printer Task Queue Manager module completed.");
        return true;
    }
    
    private void LoadTasks()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                var json = File.ReadAllText(_dataFilePath);
                _tasks = JsonSerializer.Deserialize<List<PrinterTask>>(json);
                Console.WriteLine("Loaded " + _tasks.Count + " tasks.");
            }
            else
            {
                Console.WriteLine("No existing task file found. Starting with empty queue.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
            _tasks = new List<PrinterTask>();
        }
    }
    
    private void SaveTasks()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_tasks, options);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Tasks saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving tasks: " + ex.Message);
        }
    }
    
    private void AddTask()
    {
        Console.Write("Enter document name: ");
        var name = Console.ReadLine();
        
        Console.Write("Enter priority (1=Low, 2=Medium, 3=High): ");
        if (!int.TryParse(Console.ReadLine(), out int priority) || priority < 1 || priority > 3)
        {
            Console.WriteLine("Invalid priority. Using Medium priority.");
            priority = 2;
        }
        
        var task = new PrinterTask
        {
            Id = Guid.NewGuid(),
            DocumentName = name,
            Priority = priority,
            CreatedAt = DateTime.Now
        };
        
        _tasks.Add(task);
        _tasks.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        
        Console.WriteLine("Task added successfully.");
    }
    
    private void ProcessNextTask()
    {
        if (_tasks.Count == 0)
        {
            Console.WriteLine("No tasks in queue.");
            return;
        }
        
        var task = _tasks[0];
        _tasks.RemoveAt(0);
        
        Console.WriteLine("Processing task: " + task.DocumentName);
        Console.WriteLine("Priority: " + GetPriorityName(task.Priority));
        Console.WriteLine("Created at: " + task.CreatedAt);
    }
    
    private void ListAllTasks()
    {
        if (_tasks.Count == 0)
        {
            Console.WriteLine("No tasks in queue.");
            return;
        }
        
        Console.WriteLine("Current Task Queue:");
        Console.WriteLine("------------------");
        
        foreach (var task in _tasks)
        {
            Console.WriteLine("Document: " + task.DocumentName);
            Console.WriteLine("Priority: " + GetPriorityName(task.Priority));
            Console.WriteLine("Created: " + task.CreatedAt);
            Console.WriteLine("------------------");
        }
    }
    
    private string GetPriorityName(int priority)
    {
        return priority switch
        {
            1 => "Low",
            2 => "Medium",
            3 => "High",
            _ => "Unknown"
        };
    }
}

public class PrinterTask
{
    public Guid Id { get; set; }
    public string DocumentName { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}