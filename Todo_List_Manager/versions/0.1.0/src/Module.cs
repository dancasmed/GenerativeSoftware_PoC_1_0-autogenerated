using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TodoListModule : IGeneratedModule
{
    public string Name { get; set; } = "Todo List Manager";

    private string _dataFilePath;
    private List<TodoItem> _todoItems;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Todo List Manager...");
        _dataFilePath = Path.Combine(dataFolder, "todolist.json");
        
        LoadTodoList();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTodoItem();
                    break;
                case "2":
                    ViewTodoList();
                    break;
                case "3":
                    MarkItemComplete();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveTodoList();
        Console.WriteLine("Todo List Manager has finished.");
        return true;
    }
    
    private void LoadTodoList()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                var json = File.ReadAllText(_dataFilePath);
                _todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json);
            }
            else
            {
                _todoItems = new List<TodoItem>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading todo list: " + ex.Message);
            _todoItems = new List<TodoItem>();
        }
    }
    
    private void SaveTodoList()
    {
        try
        {
            var json = JsonSerializer.Serialize(_todoItems);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving todo list: " + ex.Message);
        }
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nTodo List Manager");
        Console.WriteLine("1. Add new todo item");
        Console.WriteLine("2. View todo list");
        Console.WriteLine("3. Mark item as complete");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddTodoItem()
    {
        Console.Write("Enter task description: ");
        var description = Console.ReadLine();
        
        Console.Write("Enter priority (1-Low, 2-Medium, 3-High): ");
        if (!int.TryParse(Console.ReadLine(), out int priority) || priority < 1 || priority > 3)
        {
            Console.WriteLine("Invalid priority. Using Medium priority.");
            priority = 2;
        }
        
        Console.Write("Enter deadline (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime deadline))
        {
            Console.WriteLine("Invalid date. Using today's date.");
            deadline = DateTime.Today;
        }
        
        _todoItems.Add(new TodoItem
        {
            Description = description,
            Priority = (PriorityLevel)priority,
            Deadline = deadline,
            IsComplete = false
        });
        
        Console.WriteLine("Task added successfully.");
    }
    
    private void ViewTodoList()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No tasks in the todo list.");
            return;
        }
        
        Console.WriteLine("\nTodo List:");
        Console.WriteLine("ID | Description | Priority | Deadline | Status");
        Console.WriteLine(new string('-', 50));
        
        for (int i = 0; i < _todoItems.Count; i++)
        {
            var item = _todoItems[i];
            Console.WriteLine($"{i + 1} | {item.Description} | {item.Priority} | {item.Deadline:yyyy-MM-dd} | {(item.IsComplete ? "Complete" : "Pending")}");
        }
    }
    
    private void MarkItemComplete()
    {
        ViewTodoList();
        
        if (_todoItems.Count == 0) return;
        
        Console.Write("Enter task ID to mark as complete: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id < 1 || id > _todoItems.Count)
        {
            Console.WriteLine("Invalid task ID.");
            return;
        }
        
        _todoItems[id - 1].IsComplete = true;
        Console.WriteLine("Task marked as complete.");
    }
}

public class TodoItem
{
    public string Description { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsComplete { get; set; }
}

public enum PriorityLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}