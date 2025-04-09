using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ForumModule : IGeneratedModule
{
    public string Name { get; set; } = "Basic Online Forum Simulator";

    private string _dataFolder;
    private List<ForumThread> _threads;
    private const string ThreadsFileName = "threads.json";

    public ForumModule()
    {
        _threads = new List<ForumThread>();
    }

    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        Console.WriteLine("Initializing Basic Online Forum Simulator...");

        LoadThreads();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nForum Menu:");
            Console.WriteLine("1. View Threads");
            Console.WriteLine("2. Create New Thread");
            Console.WriteLine("3. View Thread Details");
            Console.WriteLine("4. Add Reply to Thread");
            Console.WriteLine("5. Exit Forum");
            Console.Write("Select an option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        ViewThreads();
                        break;
                    case 2:
                        CreateNewThread();
                        break;
                    case 3:
                        ViewThreadDetails();
                        break;
                    case 4:
                        AddReplyToThread();
                        break;
                    case 5:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }

        SaveThreads();
        Console.WriteLine("Forum session ended. All data saved.");
        return true;
    }

    private void LoadThreads()
    {
        string filePath = Path.Combine(_dataFolder, ThreadsFileName);
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                _threads = JsonSerializer.Deserialize<List<ForumThread>>(json);
                Console.WriteLine("Previous forum threads loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading threads: " + ex.Message);
                _threads = new List<ForumThread>();
            }
        }
        else
        {
            Console.WriteLine("No existing threads found. Starting with empty forum.");
        }
    }

    private void SaveThreads()
    {
        string filePath = Path.Combine(_dataFolder, ThreadsFileName);
        try
        {
            string json = JsonSerializer.Serialize(_threads);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Forum threads saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving threads: " + ex.Message);
        }
    }

    private void ViewThreads()
    {
        if (_threads.Count == 0)
        {
            Console.WriteLine("No threads available.");
            return;
        }

        Console.WriteLine("\nAvailable Threads:");
        for (int i = 0; i < _threads.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_threads[i].Title} (Replies: {_threads[i].Replies.Count})");
        }
    }

    private void CreateNewThread()
    {
        Console.Write("Enter thread title: ");
        string title = Console.ReadLine();
        Console.Write("Enter your message: ");
        string message = Console.ReadLine();
        Console.Write("Enter your username: ");
        string author = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
        {
            Console.WriteLine("Title and message cannot be empty.");
            return;
        }

        var newThread = new ForumThread
        {
            Title = title,
            OriginalPost = new ForumPost
            {
                Author = string.IsNullOrWhiteSpace(author) ? "Anonymous" : author,
                Message = message,
                PostDate = DateTime.Now
            }
        };

        _threads.Add(newThread);
        Console.WriteLine("New thread created successfully!");
    }

    private void ViewThreadDetails()
    {
        ViewThreads();
        if (_threads.Count == 0) return;

        Console.Write("Enter thread number to view: ");
        if (int.TryParse(Console.ReadLine(), out int threadNum) && threadNum > 0 && threadNum <= _threads.Count)
        {
            var thread = _threads[threadNum - 1];
            Console.WriteLine($"\nThread: {thread.Title}");
            Console.WriteLine($"Posted by: {thread.OriginalPost.Author} on {thread.OriginalPost.PostDate}");
            Console.WriteLine($"Message: {thread.OriginalPost.Message}");
            Console.WriteLine("\nReplies:");

            if (thread.Replies.Count == 0)
            {
                Console.WriteLine("No replies yet.");
            }
            else
            {
                foreach (var reply in thread.Replies)
                {
                    Console.WriteLine($"{reply.Author} on {reply.PostDate}: {reply.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid thread number.");
        }
    }

    private void AddReplyToThread()
    {
        ViewThreads();
        if (_threads.Count == 0) return;

        Console.Write("Enter thread number to reply to: ");
        if (int.TryParse(Console.ReadLine(), out int threadNum) && threadNum > 0 && threadNum <= _threads.Count)
        {
            Console.Write("Enter your message: ");
            string message = Console.ReadLine();
            Console.Write("Enter your username: ");
            string author = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("Message cannot be empty.");
                return;
            }

            var reply = new ForumPost
            {
                Author = string.IsNullOrWhiteSpace(author) ? "Anonymous" : author,
                Message = message,
                PostDate = DateTime.Now
            };

            _threads[threadNum - 1].Replies.Add(reply);
            Console.WriteLine("Reply added successfully!");
        }
        else
        {
            Console.WriteLine("Invalid thread number.");
        }
    }
}

public class ForumThread
{
    public string Title { get; set; }
    public ForumPost OriginalPost { get; set; }
    public List<ForumPost> Replies { get; set; } = new List<ForumPost>();
}

public class ForumPost
{
    public string Author { get; set; }
    public string Message { get; set; }
    public DateTime PostDate { get; set; }
}