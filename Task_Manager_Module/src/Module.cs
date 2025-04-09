using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TaskManagerModule : IGeneratedModule
{
    public string Name { get; set; } = "Task Manager Module";
    
    private List<Task> _tasks;
    private string _tasksFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Task Manager Module is running...");
        
        _tasksFilePath = Path.Combine(dataFolder, "tasks.json");
        LoadTasks();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    DeleteTask();
                    break;
                case "3":
                    MarkTaskAsCompleted();
                    break;
                case "4":
                    ListTasks();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveTasks();
        Console.WriteLine("Task Manager Module finished.");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nTask Manager Menu:");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. Delete Task");
        Console.WriteLine("3. Mark Task as Completed");
        Console.WriteLine("4. List Tasks");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddTask()
    {
        Console.Write("Enter task description: ");
        string description = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(description))
        {
            _tasks.Add(new Task
            {
                Id = Guid.NewGuid(),
                Description = description,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            });
            
            Console.WriteLine("Task added successfully.");
        }
        else
        {
            Console.WriteLine("Task description cannot be empty.");
        }
    }
    
    private void DeleteTask()
    {
        if (_tasks.Count == 0)
        {
            Console.WriteLine("No tasks available to delete.");
            return;
        }
        
        ListTasks();
        Console.Write("Enter task number to delete: ");
        
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= _tasks.Count)
        {
            _tasks.RemoveAt(taskNumber - 1);
            Console.WriteLine("Task deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
    }
    
    private void MarkTaskAsCompleted()
    {
        if (_tasks.Count == 0)
        {
            Console.WriteLine("No tasks available to mark as completed.");
            return;
        }
        
        ListTasks();
        Console.Write("Enter task number to mark as completed: ");
        
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= _tasks.Count)
        {
            _tasks[taskNumber - 1].IsCompleted = true;
            Console.WriteLine("Task marked as completed.");
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
    }
    
    private void ListTasks()
    {
        if (_tasks.Count == 0)
        {
            Console.WriteLine("No tasks available.");
            return;
        }
        
        Console.WriteLine("\nTasks:");
        for (int i = 0; i < _tasks.Count; i++)
        {
            var task = _tasks[i];
            Console.WriteLine($"{i + 1}. [{(task.IsCompleted ? "X" : " ")}] {task.Description} (Created: {task.CreatedAt:yyyy-MM-dd HH:mm})");
        }
    }
    
    private void LoadTasks()
    {
        try
        {
            if (File.Exists(_tasksFilePath))
            {
                string json = File.ReadAllText(_tasksFilePath);
                _tasks = JsonSerializer.Deserialize<List<Task>>(json) ?? new List<Task>();
            }
            else
            {
                _tasks = new List<Task>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
            _tasks = new List<Task>();
        }
    }
    
    private void SaveTasks()
    {
        try
        {
            string json = JsonSerializer.Serialize(_tasks);
            File.WriteAllText(_tasksFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving tasks: " + ex.Message);
        }
    }
}

public class Task
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}