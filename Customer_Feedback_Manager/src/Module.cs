using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CustomerFeedbackManager : IGeneratedModule
{
    public string Name { get; set; } = "Customer Feedback Manager";
    
    private string feedbackFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Customer Feedback Manager module is running.");
        
        feedbackFilePath = Path.Combine(dataFolder, "customer_feedback.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        bool exitRequested = false;
        
        while (!exitRequested)
        {
            Console.WriteLine("\nCustomer Feedback Manager");
            Console.WriteLine("1. Add Feedback");
            Console.WriteLine("2. View All Feedback");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            if (int.TryParse(input, out int choice))
            {
                switch (choice)
                {
                    case 1:
                        AddFeedback();
                        break;
                    case 2:
                        ViewAllFeedback();
                        break;
                    case 3:
                        exitRequested = true;
                        Console.WriteLine("Exiting Customer Feedback Manager.");
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
        
        return true;
    }
    
    private void AddFeedback()
    {
        Console.Write("Enter customer name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter feedback (1-5 stars): ");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
        {
            Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
            return;
        }
        
        Console.Write("Enter comments: ");
        string comments = Console.ReadLine();
        
        var feedback = new Feedback
        {
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
    
    private void ViewAllFeedback()
    {
        var feedbackList = LoadFeedback();
        
        if (feedbackList.Count == 0)
        {
            Console.WriteLine("No feedback available.");
            return;
        }
        
        Console.WriteLine("\nAll Customer Feedback:");
        foreach (var feedback in feedbackList)
        {
            Console.WriteLine($"Date: {feedback.Date}");
            Console.WriteLine($"Customer: {feedback.CustomerName}");
            Console.WriteLine($"Rating: {feedback.Rating} stars");
            Console.WriteLine($"Comments: {feedback.Comments}");
            Console.WriteLine("----------------------------");
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
    public string CustomerName { get; set; }
    public int Rating { get; set; }
    public string Comments { get; set; }
    public DateTime Date { get; set; }
}