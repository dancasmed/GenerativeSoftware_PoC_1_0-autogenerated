using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CustomerFeedbackModule : IGeneratedModule
{
    public string Name { get; set; } = "Customer Feedback Manager";
    
    private string feedbackFilePath;
    
    public bool Main(string dataFolder)
    {
        feedbackFilePath = Path.Combine(dataFolder, "customer_feedback.json");
        
        Console.WriteLine("Customer Feedback Module is running.");
        Console.WriteLine("Available commands: add, list, exit");
        
        bool running = true;
        while (running)
        {
            Console.Write("> ");
            string input = Console.ReadLine().Trim().ToLower();
            
            switch (input)
            {
                case "add":
                    AddFeedback();
                    break;
                case "list":
                    ListFeedback();
                    break;
                case "exit":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid command. Try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void AddFeedback()
    {
        Console.Write("Enter customer name: ");
        string name = Console.ReadLine().Trim();
        
        Console.Write("Enter feedback (1-5 stars): ");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
        {
            Console.WriteLine("Invalid rating. Must be between 1 and 5 stars.");
            return;
        }
        
        Console.Write("Enter feedback comments: ");
        string comments = Console.ReadLine().Trim();
        
        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            CustomerName = name,
            Rating = rating,
            Comments = comments,
            Date = DateTime.Now
        };
        
        List<Feedback> feedbackList = LoadFeedback();
        feedbackList.Add(feedback);
        
        SaveFeedback(feedbackList);
        
        Console.WriteLine("Feedback added successfully.");
    }
    
    private void ListFeedback()
    {
        var feedbackList = LoadFeedback();
        
        if (feedbackList.Count == 0)
        {
            Console.WriteLine("No feedback available.");
            return;
        }
        
        Console.WriteLine("Customer Feedback:");
        Console.WriteLine("----------------");
        
        foreach (var feedback in feedbackList)
        {
            Console.WriteLine("ID: " + feedback.Id);
            Console.WriteLine("Customer: " + feedback.CustomerName);
            Console.WriteLine("Rating: " + feedback.Rating + " stars");
            Console.WriteLine("Comments: " + feedback.Comments);
            Console.WriteLine("Date: " + feedback.Date);
            Console.WriteLine();
        }
    }
    
    private List<Feedback> LoadFeedback()
    {
        if (!File.Exists(feedbackFilePath))
        {
            return new List<Feedback>();
        }
        
        string json = File.ReadAllText(feedbackFilePath);
        return JsonSerializer.Deserialize<List<Feedback>>(json);
    }
    
    private void SaveFeedback(List<Feedback> feedbackList)
    {
        string json = JsonSerializer.Serialize(feedbackList);
        File.WriteAllText(feedbackFilePath, json);
    }
}

public class Feedback
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public int Rating { get; set; }
    public string Comments { get; set; }
    public DateTime Date { get; set; }
}