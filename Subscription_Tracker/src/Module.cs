using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SubscriptionTracker : IGeneratedModule
{
    public string Name { get; set; } = "Subscription Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Subscription Tracker module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "subscriptions.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<Subscription> subscriptions = LoadSubscriptions();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddSubscription(subscriptions);
                    break;
                case "2":
                    ViewSubscriptions(subscriptions);
                    break;
                case "3":
                    CheckRenewals(subscriptions);
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveSubscriptions(subscriptions);
        }
        
        return true;
    }
    
    private List<Subscription> LoadSubscriptions()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<Subscription>();
        }
        
        string json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<Subscription>>(json) ?? new List<Subscription>();
    }
    
    private void SaveSubscriptions(List<Subscription> subscriptions)
    {
        string json = JsonSerializer.Serialize(subscriptions);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nSubscription Tracker");
        Console.WriteLine("1. Add Subscription");
        Console.WriteLine("2. View Subscriptions");
        Console.WriteLine("3. Check Upcoming Renewals");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddSubscription(List<Subscription> subscriptions)
    {
        Console.Write("Enter subscription name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter monthly cost: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal cost))
        {
            Console.WriteLine("Invalid cost. Please enter a valid number.");
            return;
        }
        
        Console.Write("Enter renewal day of month (1-31): ");
        if (!int.TryParse(Console.ReadLine(), out int renewalDay) || renewalDay < 1 || renewalDay > 31)
        {
            Console.WriteLine("Invalid day. Please enter a number between 1 and 31.");
            return;
        }
        
        subscriptions.Add(new Subscription
        {
            Name = name,
            MonthlyCost = cost,
            RenewalDay = renewalDay
        });
        
        Console.WriteLine("Subscription added successfully.");
    }
    
    private void ViewSubscriptions(List<Subscription> subscriptions)
    {
        if (subscriptions.Count == 0)
        {
            Console.WriteLine("No subscriptions found.");
            return;
        }
        
        Console.WriteLine("\nYour Subscriptions:");
        foreach (var sub in subscriptions)
        {
            Console.WriteLine($"{sub.Name} - ${sub.MonthlyCost} - Renews on {sub.RenewalDay}th");
        }
    }
    
    private void CheckRenewals(List<Subscription> subscriptions)
    {
        if (subscriptions.Count == 0)
        {
            Console.WriteLine("No subscriptions found.");
            return;
        }
        
        int currentDay = DateTime.Now.Day;
        Console.WriteLine("\nUpcoming Renewals:");
        
        foreach (var sub in subscriptions)
        {
            if (sub.RenewalDay >= currentDay && sub.RenewalDay <= currentDay + 7)
            {
                Console.WriteLine($"{sub.Name} renews on {sub.RenewalDay}th (${sub.MonthlyCost})");
            }
        }
    }
}

public class Subscription
{
    public string Name { get; set; }
    public decimal MonthlyCost { get; set; }
    public int RenewalDay { get; set; }
}