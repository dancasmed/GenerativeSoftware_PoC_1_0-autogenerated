using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BlogPostScheduleManager : IGeneratedModule
{
    public string Name { get; set; } = "Blog Post Schedule Manager";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Blog Post Schedule Manager...");
        
        _dataFilePath = Path.Combine(dataFolder, "blogPosts.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<BlogPost> blogPosts = LoadBlogPosts();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nBlog Post Schedule Manager");
            Console.WriteLine("1. Add new blog post");
            Console.WriteLine("2. View all blog posts");
            Console.WriteLine("3. Mark post as completed");
            Console.WriteLine("4. Delete blog post");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddBlogPost(blogPosts);
                    break;
                case "2":
                    ViewAllBlogPosts(blogPosts);
                    break;
                case "3":
                    MarkPostAsCompleted(blogPosts);
                    break;
                case "4":
                    DeleteBlogPost(blogPosts);
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveBlogPosts(blogPosts);
        }
        
        Console.WriteLine("Blog Post Schedule Manager finished.");
        return true;
    }
    
    private List<BlogPost> LoadBlogPosts()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<BlogPost>>(json);
        }
        return new List<BlogPost>();
    }
    
    private void SaveBlogPosts(List<BlogPost> blogPosts)
    {
        string json = JsonSerializer.Serialize(blogPosts);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddBlogPost(List<BlogPost> blogPosts)
    {
        Console.Write("Enter blog post title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter blog post topic: ");
        string topic = Console.ReadLine();
        
        Console.Write("Enter deadline (yyyy-MM-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime deadline))
        {
            blogPosts.Add(new BlogPost
            {
                Id = Guid.NewGuid(),
                Title = title,
                Topic = topic,
                Deadline = deadline,
                IsCompleted = false
            });
            
            Console.WriteLine("Blog post added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format. Blog post not added.");
        }
    }
    
    private void ViewAllBlogPosts(List<BlogPost> blogPosts)
    {
        if (blogPosts.Count == 0)
        {
            Console.WriteLine("No blog posts found.");
            return;
        }
        
        Console.WriteLine("\nAll Blog Posts:");
        foreach (var post in blogPosts)
        {
            string status = post.IsCompleted ? "[Completed]" : "[Pending]";
            Console.WriteLine($"{post.Id} - {post.Title} ({post.Topic}) - Due: {post.Deadline:yyyy-MM-dd} {status}");
        }
    }
    
    private void MarkPostAsCompleted(List<BlogPost> blogPosts)
    {
        ViewAllBlogPosts(blogPosts);
        
        if (blogPosts.Count == 0) return;
        
        Console.Write("Enter the ID of the post to mark as completed: ");
        if (Guid.TryParse(Console.ReadLine(), out Guid postId))
        {
            var post = blogPosts.Find(p => p.Id == postId);
            if (post != null)
            {
                post.IsCompleted = true;
                Console.WriteLine("Post marked as completed.");
            }
            else
            {
                Console.WriteLine("Post not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
    
    private void DeleteBlogPost(List<BlogPost> blogPosts)
    {
        ViewAllBlogPosts(blogPosts);
        
        if (blogPosts.Count == 0) return;
        
        Console.Write("Enter the ID of the post to delete: ");
        if (Guid.TryParse(Console.ReadLine(), out Guid postId))
        {
            var post = blogPosts.Find(p => p.Id == postId);
            if (post != null)
            {
                blogPosts.Remove(post);
                Console.WriteLine("Post deleted successfully.");
            }
            else
            {
                Console.WriteLine("Post not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
}

public class BlogPost
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Topic { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsCompleted { get; set; }
}