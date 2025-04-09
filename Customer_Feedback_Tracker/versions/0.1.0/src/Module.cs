using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CustomerFeedbackModule : IGeneratedModule
{
    public string Name { get; set; } = "Customer Feedback Tracker";

    private string feedbackFilePath;
    private List<Feedback> feedbackList;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Customer Feedback Module is running...");
        
        feedbackFilePath = Path.Combine(dataFolder, "customer_feedback.json");
        feedbackList = new List<Feedback>();

        LoadFeedbackData();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nCustomer Feedback Tracker");
            Console.WriteLine("1. Add Feedback");
            Console.WriteLine("2. View All Feedback");
            Console.WriteLine("3. Filter by Category");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddFeedback();
                    break;
                case "2":
                    ViewAllFeedback();
                    break;
                case "3":
                    FilterByCategory();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveFeedbackData();
        Console.WriteLine("Customer Feedback Module has completed its operation.");
        return true;
    }

    private void LoadFeedbackData()
    {
        try
        {
            if (File.Exists(feedbackFilePath))
            {
                string json = File.ReadAllText(feedbackFilePath);
                feedbackList = JsonSerializer.Deserialize<List<Feedback>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading feedback data: " + ex.Message);
        }
    }

    private void SaveFeedbackData()
    {
        try
        {
            string json = JsonSerializer.Serialize(feedbackList);
            File.WriteAllText(feedbackFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving feedback data: " + ex.Message);
        }
    }

    private void AddFeedback()
    {
        Console.Write("Enter customer name: ");
        string name = Console.ReadLine();

        Console.Write("Enter feedback: ");
        string text = Console.ReadLine();

        Console.Write("Enter category (Complaint/Suggestion/Praise): ");
        string category = Console.ReadLine();

        Console.Write("Enter rating (1-5): ");
        int rating = int.Parse(Console.ReadLine());

        feedbackList.Add(new Feedback
        {
            CustomerName = name,
            FeedbackText = text,
            Category = category,
            Rating = rating,
            Date = DateTime.Now
        });

        Console.WriteLine("Feedback added successfully!");
    }

    private void ViewAllFeedback()
    {
        if (feedbackList.Count == 0)
        {
            Console.WriteLine("No feedback available.");
            return;
        }

        foreach (var feedback in feedbackList)
        {
            Console.WriteLine("\n--- Feedback ---");
            Console.WriteLine("Customer: " + feedback.CustomerName);
            Console.WriteLine("Date: " + feedback.Date);
            Console.WriteLine("Category: " + feedback.Category);
            Console.WriteLine("Rating: " + feedback.Rating + "/5");
            Console.WriteLine("Feedback: " + feedback.FeedbackText);
        }
    }

    private void FilterByCategory()
    {
        Console.Write("Enter category to filter (Complaint/Suggestion/Praise): ");
        string category = Console.ReadLine();

        var filtered = feedbackList.FindAll(f => f.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (filtered.Count == 0)
        {
            Console.WriteLine("No feedback found in this category.");
            return;
        }

        foreach (var feedback in filtered)
        {
            Console.WriteLine("\n--- Feedback ---");
            Console.WriteLine("Customer: " + feedback.CustomerName);
            Console.WriteLine("Date: " + feedback.Date);
            Console.WriteLine("Category: " + feedback.Category);
            Console.WriteLine("Rating: " + feedback.Rating + "/5");
            Console.WriteLine("Feedback: " + feedback.FeedbackText);
        }
    }

    private class Feedback
    {
        public string CustomerName { get; set; }
        public string FeedbackText { get; set; }
        public string Category { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}