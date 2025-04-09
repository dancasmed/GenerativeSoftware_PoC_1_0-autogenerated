using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ToDoListManager : IGeneratedModule
{
    public string Name { get; set; } = "To-Do List Manager";
    
    private List<TodoItem> _todoItems;
    private string _dataFilePath;
    
    public ToDoListManager()
    {
        _todoItems = new List<TodoItem>();
    }
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "todolist.json");
        
        Console.WriteLine("To-Do List Manager is running...");
        
        LoadTodoItems();
        
        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddTodoItem();
                    break;
                case "2":
                    ViewTodoItems();
                    break;
                case "3":
                    MarkItemComplete();
                    break;
                case "4":
                    DeleteTodoItem();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveTodoItems();
        }
        
        Console.WriteLine("Exiting To-Do List Manager...");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nTo-Do List Manager");
        Console.WriteLine("1. Add new item");
        Console.WriteLine("2. View all items");
        Console.WriteLine("3. Mark item as complete");
        Console.WriteLine("4. Delete item");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddTodoItem()
    {
        Console.Write("Enter task description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter due date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
        {
            Console.WriteLine("Invalid date format. Using today's date.");
            dueDate = DateTime.Today;
        }
        
        Console.Write("Enter priority (1-High, 2-Medium, 3-Low): ");
        if (!int.TryParse(Console.ReadLine(), out int priority) || priority < 1 || priority > 3)
        {
            Console.WriteLine("Invalid priority. Setting to Medium (2).");
            priority = 2;
        }
        
        _todoItems.Add(new TodoItem
        {
            Id = Guid.NewGuid(),
            Description = description,
            DueDate = dueDate,
            Priority = priority,
            IsComplete = false,
            CreatedDate = DateTime.Now
        });
        
        Console.WriteLine("Task added successfully.");
    }
    
    private void ViewTodoItems()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }
        
        Console.WriteLine("\nID\tDescription\tDue Date\t\tPriority\tStatus");
        Console.WriteLine(new string('-', 80));
        
        foreach (var item in _todoItems)
        {
            string status = item.IsComplete ? "Complete" : "Pending";
            string priority = item.Priority switch
            {
                1 => "High",
                2 => "Medium",
                3 => "Low",
                _ => "Unknown"
            };
            
            Console.WriteLine($"{item.Id.ToString().Substring(0, 8)}...\t{item.Description}\t{item.DueDate:yyyy-MM-dd}\t{priority}\t\t{status}");
        }
    }
    
    private void MarkItemComplete()
    {
        ViewTodoItems();
        
        if (_todoItems.Count == 0) return;
        
        Console.Write("Enter the ID of the task to mark as complete: ");
        string idInput = Console.ReadLine();
        
        if (Guid.TryParse(idInput, out Guid id))
        {
            var item = _todoItems.Find(x => x.Id == id);
            if (item != null)
            {
                item.IsComplete = true;
                Console.WriteLine("Task marked as complete.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
    
    private void DeleteTodoItem()
    {
        ViewTodoItems();
        
        if (_todoItems.Count == 0) return;
        
        Console.Write("Enter the ID of the task to delete: ");
        string idInput = Console.ReadLine();
        
        if (Guid.TryParse(idInput, out Guid id))
        {
            int removed = _todoItems.RemoveAll(x => x.Id == id);
            if (removed > 0)
            {
                Console.WriteLine("Task deleted successfully.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
    
    private void LoadTodoItems()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading todo items: " + ex.Message);
        }
    }
    
    private void SaveTodoItems()
    {
        try
        {
            string json = JsonSerializer.Serialize(_todoItems);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving todo items: " + ex.Message);
        }
    }
}

public class TodoItem
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public int Priority { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedDate { get; set; }
}