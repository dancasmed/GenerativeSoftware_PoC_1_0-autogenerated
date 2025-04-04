using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TodoItem
{
    public string Task { get; set; }
    public int Priority { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsCompleted { get; set; }
}

public class TodoListModule : IGeneratedModule
{
    public string Name { get; set; } = "Todo List Manager";
    private List<TodoItem> _todoItems;
    private string _dataFilePath;

    public TodoListModule()
    {
        _todoItems = new List<TodoItem>();
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "todolist.json");
        
        Console.WriteLine("Initializing Todo List Manager...");
        
        LoadTodoList();
        
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nTodo List Manager");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. View Tasks");
            Console.WriteLine("3. Mark Task as Completed");
            Console.WriteLine("4. Save and Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTask();
                    break;
                case "2":
                    ViewTasks();
                    break;
                case "3":
                    MarkTaskCompleted();
                    break;
                case "4":
                    SaveTodoList();
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Todo List Manager has exited successfully.");
        return true;
    }
    
    private void LoadTodoList()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json);
                Console.WriteLine("Todo list loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading todo list: " + ex.Message);
                _todoItems = new List<TodoItem>();
            }
        }
        else
        {
            Console.WriteLine("No existing todo list found. A new one will be created.");
        }
    }
    
    private void SaveTodoList()
    {
        try
        {
            string json = JsonSerializer.Serialize(_todoItems);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Todo list saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving todo list: " + ex.Message);
        }
    }
    
    private void AddTask()
    {
        Console.Write("Enter task description: ");
        string task = Console.ReadLine();
        
        Console.Write("Enter priority (1-5, where 5 is highest): ");
        int priority;
        while (!int.TryParse(Console.ReadLine(), out priority) || priority < 1 || priority > 5)
        {
            Console.Write("Invalid input. Enter priority (1-5): ");
        }
        
        Console.Write("Enter deadline (yyyy-MM-dd): ");
        DateTime deadline;
        while (!DateTime.TryParse(Console.ReadLine(), out deadline))
        {
            Console.Write("Invalid date format. Enter deadline (yyyy-MM-dd): ");
        }
        
        _todoItems.Add(new TodoItem
        {
            Task = task,
            Priority = priority,
            Deadline = deadline,
            IsCompleted = false
        });
        
        Console.WriteLine("Task added successfully.");
    }
    
    private void ViewTasks()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }
        
        Console.WriteLine("\nTasks:");
        Console.WriteLine("ID | Task | Priority | Deadline | Status");
        Console.WriteLine("----------------------------------------");
        
        for (int i = 0; i < _todoItems.Count; i++)
        {
            var item = _todoItems[i];
            Console.WriteLine($"{i + 1} | {item.Task} | {item.Priority} | {item.Deadline:yyyy-MM-dd} | {(item.IsCompleted ? "Completed" : "Pending")}");
        }
    }
    
    private void MarkTaskCompleted()
    {
        ViewTasks();
        
        if (_todoItems.Count == 0)
        {
            return;
        }
        
        Console.Write("Enter task ID to mark as completed: ");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id) || id < 1 || id > _todoItems.Count)
        {
            Console.Write("Invalid ID. Enter task ID: ");
        }
        
        _todoItems[id - 1].IsCompleted = true;
        Console.WriteLine("Task marked as completed.");
    }
}