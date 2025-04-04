using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TodoListManager : IGeneratedModule
{
    public string Name { get; set; } = "Todo List Manager";

    private string _dataFilePath;
    private List<TodoItem> _todoItems;

    public TodoListManager()
    {
        _todoItems = new List<TodoItem>();
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "todolist.json");
        LoadTodoList();

        Console.WriteLine("Todo List Manager is running.");
        Console.WriteLine("Type 'help' for available commands.");

        bool running = true;
        while (running)
        {
            Console.Write("> ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

            switch (input)
            {
                case "help":
                    ShowHelp();
                    break;
                case "list":
                    ListTodoItems();
                    break;
                case "add":
                    AddTodoItem();
                    break;
                case "remove":
                    RemoveTodoItem();
                    break;
                case "complete":
                    MarkAsComplete();
                    break;
                case "exit":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Unknown command. Type 'help' for available commands.");
                    break;
            }
        }

        SaveTodoList();
        return true;
    }

    private void LoadTodoList()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }
    }

    private void SaveTodoList()
    {
        string json = JsonSerializer.Serialize(_todoItems);
        File.WriteAllText(_dataFilePath, json);
    }

    private void ShowHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("help - Show this help message");
        Console.WriteLine("list - List all todo items");
        Console.WriteLine("add - Add a new todo item");
        Console.WriteLine("remove - Remove a todo item");
        Console.WriteLine("complete - Mark a todo item as complete");
        Console.WriteLine("exit - Exit the Todo List Manager");
    }

    private void ListTodoItems()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No todo items found.");
            return;
        }

        Console.WriteLine("Todo Items:");
        for (int i = 0; i < _todoItems.Count; i++)
        {
            var item = _todoItems[i];
            Console.WriteLine($"{i + 1}. [{(item.IsComplete ? "X" : " ")}] {item.Title}");
            Console.WriteLine($"   Priority: {item.Priority}, Deadline: {item.Deadline:yyyy-MM-dd}");
            if (!string.IsNullOrEmpty(item.Description))
                Console.WriteLine($"   Description: {item.Description}");
        }
    }

    private void AddTodoItem()
    {
        Console.Write("Enter title: ");
        string title = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("Enter description (optional): ");
        string description = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Enter priority (1-Low, 2-Medium, 3-High): ");
        if (!int.TryParse(Console.ReadLine(), out int priority) || priority < 1 || priority > 3)
        {
            Console.WriteLine("Invalid priority. Using Medium (2) as default.");
            priority = 2;
        }

        Console.Write("Enter deadline (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime deadline))
        {
            Console.WriteLine("Invalid date. Using today as default.");
            deadline = DateTime.Today;
        }

        _todoItems.Add(new TodoItem
        {
            Title = title,
            Description = description,
            Priority = (PriorityLevel)priority,
            Deadline = deadline,
            IsComplete = false
        });

        Console.WriteLine("Todo item added successfully.");
    }

    private void RemoveTodoItem()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No todo items to remove.");
            return;
        }

        ListTodoItems();
        Console.Write("Enter the number of the item to remove: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _todoItems.Count)
        {
            _todoItems.RemoveAt(index - 1);
            Console.WriteLine("Todo item removed successfully.");
        }
        else
        {
            Console.WriteLine("Invalid item number.");
        }
    }

    private void MarkAsComplete()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No todo items to mark as complete.");
            return;
        }

        ListTodoItems();
        Console.Write("Enter the number of the item to mark as complete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _todoItems.Count)
        {
            _todoItems[index - 1].IsComplete = true;
            Console.WriteLine("Todo item marked as complete.");
        }
        else
        {
            Console.WriteLine("Invalid item number.");
        }
    }
}

public class TodoItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime Deadline { get; set; } = DateTime.Today;
    public bool IsComplete { get; set; } = false;
}

public enum PriorityLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}