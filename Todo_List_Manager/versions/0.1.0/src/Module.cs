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

    public TodoItem(string task, int priority, DateTime deadline)
    {
        Task = task;
        Priority = priority;
        Deadline = deadline;
        IsCompleted = false;
    }
}

public class TodoListModule : IGeneratedModule
{
    public string Name { get; set; } = "Todo List Manager";
    private List<TodoItem> _todoItems = new List<TodoItem>();
    private string _dataFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Todo List Manager...");
        
        _dataFilePath = Path.Combine(dataFolder, "todolist.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        LoadTodoList();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddNewTask();
                    break;
                case "2":
                    ViewAllTasks();
                    break;
                case "3":
                    MarkTaskAsCompleted();
                    break;
                case "4":
                    DeleteTask();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveTodoList();
        Console.WriteLine("Todo List Manager has finished execution.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nTodo List Manager");
        Console.WriteLine("1. Add new task");
        Console.WriteLine("2. View all tasks");
        Console.WriteLine("3. Mark task as completed");
        Console.WriteLine("4. Delete task");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddNewTask()
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
        
        _todoItems.Add(new TodoItem(task, priority, deadline));
        Console.WriteLine("Task added successfully.");
    }

    private void ViewAllTasks()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }
        
        Console.WriteLine("\nAll Tasks:");
        Console.WriteLine("--------------------------------------------------------");
        Console.WriteLine("| # | Task | Priority | Deadline | Status |");
        Console.WriteLine("--------------------------------------------------------");
        
        for (int i = 0; i < _todoItems.Count; i++)
        {
            var item = _todoItems[i];
            Console.WriteLine(string.Format("| {0} | {1} | {2} | {3} | {4} |", 
                i + 1, 
                item.Task, 
                item.Priority, 
                item.Deadline.ToString("yyyy-MM-dd"), 
                item.IsCompleted ? "Completed" : "Pending"));
        }
        
        Console.WriteLine("--------------------------------------------------------");
    }

    private void MarkTaskAsCompleted()
    {
        ViewAllTasks();
        if (_todoItems.Count == 0) return;
        
        Console.Write("Enter task number to mark as completed: ");
        int taskNumber;
        while (!int.TryParse(Console.ReadLine(), out taskNumber) || taskNumber < 1 || taskNumber > _todoItems.Count)
        {
            Console.Write("Invalid input. Enter task number: ");
        }
        
        _todoItems[taskNumber - 1].IsCompleted = true;
        Console.WriteLine("Task marked as completed.");
    }

    private void DeleteTask()
    {
        ViewAllTasks();
        if (_todoItems.Count == 0) return;
        
        Console.Write("Enter task number to delete: ");
        int taskNumber;
        while (!int.TryParse(Console.ReadLine(), out taskNumber) || taskNumber < 1 || taskNumber > _todoItems.Count)
        {
            Console.Write("Invalid input. Enter task number: ");
        }
        
        _todoItems.RemoveAt(taskNumber - 1);
        Console.WriteLine("Task deleted successfully.");
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
            }
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
}