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
    public string Name { get; set; } = "TodoListModule";
    private List<TodoItem> _todoItems = new List<TodoItem>();
    private string _dataFilePath;

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "todolist.json");
        LoadTodoList();

        Console.WriteLine("Todo List Module is running.");
        Console.WriteLine("Commands: add, list, complete, exit");

        bool running = true;
        while (running)
        {
            Console.Write("> ");
            string command = Console.ReadLine().Trim().ToLower();

            switch (command)
            {
                case "add":
                    AddTodoItem();
                    break;
                case "list":
                    ListTodoItems();
                    break;
                case "complete":
                    MarkAsComplete();
                    break;
                case "exit":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid command. Try again.");
                    break;
            }
        }

        SaveTodoList();
        return true;
    }

    private void AddTodoItem()
    {
        Console.Write("Enter task description: ");
        string task = Console.ReadLine().Trim();

        Console.Write("Enter priority (1-5, where 5 is highest): ");
        int priority;
        while (!int.TryParse(Console.ReadLine(), out priority) || priority < 1 || priority > 5)
        {
            Console.Write("Invalid priority. Enter a number between 1 and 5: ");
        }

        Console.Write("Enter deadline (yyyy-MM-dd): ");
        DateTime deadline;
        while (!DateTime.TryParse(Console.ReadLine(), out deadline) || deadline < DateTime.Today)
        {
            Console.Write("Invalid date. Enter a valid future date (yyyy-MM-dd): ");
        }

        _todoItems.Add(new TodoItem(task, priority, deadline));
        Console.WriteLine("Task added successfully.");
    }

    private void ListTodoItems()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No tasks in the list.");
            return;
        }

        Console.WriteLine("ID\tTask\t\tPriority\tDeadline\tStatus");
        for (int i = 0; i < _todoItems.Count; i++)
        {
            var item = _todoItems[i];
            Console.WriteLine($"{i}\t{item.Task}\t\t{item.Priority}\t\t{item.Deadline:yyyy-MM-dd}\t\t{(item.IsCompleted ? "Completed" : "Pending")}");
        }
    }

    private void MarkAsComplete()
    {
        ListTodoItems();
        if (_todoItems.Count == 0) return;

        Console.Write("Enter task ID to mark as complete: ");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id) || id < 0 || id >= _todoItems.Count)
        {
            Console.Write("Invalid ID. Enter a valid task ID: ");
        }

        _todoItems[id].IsCompleted = true;
        Console.WriteLine("Task marked as complete.");
    }

    private void LoadTodoList()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json);
        }
    }

    private void SaveTodoList()
    {
        string json = JsonSerializer.Serialize(_todoItems);
        File.WriteAllText(_dataFilePath, json);
    }
}