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
    private List<TodoItem> _todoItems;
    private string _dataFilePath;

    public TodoListModule()
    {
        _todoItems = new List<TodoItem>();
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "todolist.json");
        LoadTodoList();

        Console.WriteLine("Todo List Manager is running.");
        Console.WriteLine("Type 'add' to add a new task, 'list' to view tasks, 'complete' to mark a task as done, or 'exit' to quit.");

        string command;
        do
        {
            Console.Write("> ");
            command = Console.ReadLine().Trim().ToLower();

            switch (command)
            {
                case "add":
                    AddNewTask();
                    break;
                case "list":
                    ListTasks();
                    break;
                case "complete":
                    MarkTaskAsComplete();
                    break;
                case "exit":
                    SaveTodoList();
                    Console.WriteLine("Todo list saved. Exiting...");
                    break;
                default:
                    Console.WriteLine("Unknown command. Try again.");
                    break;
            }
        } while (command != "exit");

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
            }
        }
        else
        {
            Console.WriteLine("No existing todo list found. Starting with an empty list.");
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

    private void AddNewTask()
    {
        Console.Write("Enter task description: ");
        string task = Console.ReadLine().Trim();

        Console.Write("Enter priority (1-5, where 5 is highest): ");
        if (!int.TryParse(Console.ReadLine(), out int priority) || priority < 1 || priority > 5)
        {
            Console.WriteLine("Invalid priority. Please enter a number between 1 and 5.");
            return;
        }

        Console.Write("Enter deadline (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime deadline))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd format.");
            return;
        }

        _todoItems.Add(new TodoItem(task, priority, deadline));
        Console.WriteLine("Task added successfully.");
    }

    private void ListTasks()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No tasks in the list.");
            return;
        }

        Console.WriteLine("Tasks:");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("| # | Task                | Priority | Deadline   | Done |");
        Console.WriteLine("--------------------------------------------------");

        for (int i = 0; i < _todoItems.Count; i++)
        {
            var item = _todoItems[i];
            Console.WriteLine(string.Format("| {0} | {1,-20} | {2,-8} | {3:yyyy-MM-dd} | {4,-4} |",
                i + 1, item.Task, item.Priority, item.Deadline, item.IsCompleted ? "Yes" : "No"));
        }

        Console.WriteLine("--------------------------------------------------");
    }

    private void MarkTaskAsComplete()
    {
        if (_todoItems.Count == 0)
        {
            Console.WriteLine("No tasks to mark as complete.");
            return;
        }

        ListTasks();
        Console.Write("Enter task number to mark as complete: ");

        if (!int.TryParse(Console.ReadLine(), out int taskNumber) || taskNumber < 1 || taskNumber > _todoItems.Count)
        {
            Console.WriteLine("Invalid task number.");
            return;
        }

        _todoItems[taskNumber - 1].IsCompleted = true;
        Console.WriteLine("Task marked as complete.");
    }
}