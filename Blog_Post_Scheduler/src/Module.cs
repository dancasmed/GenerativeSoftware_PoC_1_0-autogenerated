using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BlogPostScheduler : IGeneratedModule
{
    public string Name { get; set; } = "Blog Post Scheduler";
    
    private List<BlogPost> _blogPosts;
    private string _dataFilePath;
    
    public BlogPostScheduler()
    {
        _blogPosts = new List<BlogPost>();
    }
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "blogposts.json");
        
        Console.WriteLine("Blog Post Scheduler Module is running...");
        
        LoadBlogPosts();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddBlogPost();
                    break;
                case "2":
                    ViewAllBlogPosts();
                    break;
                case "3":
                    DeleteBlogPost();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveBlogPosts();
        
        Console.WriteLine("Blog Post Scheduler Module finished.");
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nBlog Post Scheduler Menu:");
        Console.WriteLine("1. Add a new blog post");
        Console.WriteLine("2. View all blog posts");
        Console.WriteLine("3. Delete a blog post");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddBlogPost()
    {
        Console.Write("Enter blog post title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter blog post topic: ");
        string topic = Console.ReadLine();
        
        Console.Write("Enter deadline (yyyy-MM-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime deadline))
        {
            _blogPosts.Add(new BlogPost { Title = title, Topic = topic, Deadline = deadline });
            Console.WriteLine("Blog post added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format. Blog post not added.");
        }
    }
    
    private void ViewAllBlogPosts()
    {
        if (_blogPosts.Count == 0)
        {
            Console.WriteLine("No blog posts available.");
            return;
        }
        
        Console.WriteLine("\nAll Blog Posts:");
        foreach (var post in _blogPosts)
        {
            Console.WriteLine("Title: " + post.Title);
            Console.WriteLine("Topic: " + post.Topic);
            Console.WriteLine("Deadline: " + post.Deadline.ToString("yyyy-MM-dd"));
            Console.WriteLine("----");
        }
    }
    
    private void DeleteBlogPost()
    {
        if (_blogPosts.Count == 0)
        {
            Console.WriteLine("No blog posts available to delete.");
            return;
        }
        
        ViewAllBlogPosts();
        
        Console.Write("Enter the title of the blog post to delete: ");
        string titleToDelete = Console.ReadLine();
        
        int removedCount = _blogPosts.RemoveAll(p => p.Title.Equals(titleToDelete, StringComparison.OrdinalIgnoreCase));
        
        if (removedCount > 0)
        {
            Console.WriteLine("Blog post deleted successfully.");
        }
        else
        {
            Console.WriteLine("No blog post found with that title.");
        }
    }
    
    private void LoadBlogPosts()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _blogPosts = JsonSerializer.Deserialize<List<BlogPost>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading blog posts: " + ex.Message);
            }
        }
    }
    
    private void SaveBlogPosts()
    {
        try
        {
            string json = JsonSerializer.Serialize(_blogPosts);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving blog posts: " + ex.Message);
        }
    }
}

public class BlogPost
{
    public string Title { get; set; }
    public string Topic { get; set; }
    public DateTime Deadline { get; set; }
}