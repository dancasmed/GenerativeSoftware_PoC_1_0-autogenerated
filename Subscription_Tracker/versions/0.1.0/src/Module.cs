using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SubscriptionTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Subscription Tracker";

    private string _subscriptionsFilePath;

    public bool Main(string dataFolder)
    {
        _subscriptionsFilePath = Path.Combine(dataFolder, "subscriptions.json");

        Console.WriteLine("Subscription Tracker Module is running.");
        Console.WriteLine("Data will be stored in: " + _subscriptionsFilePath);

        InitializeDataFile();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddSubscription();
                    break;
                case "2":
                    ViewSubscriptions();
                    break;
                case "3":
                    CheckRenewals();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        return true;
    }

    private void InitializeDataFile()
    {
        if (!File.Exists(_subscriptionsFilePath))
        {
            File.WriteAllText(_subscriptionsFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nSubscription Tracker Menu:");
        Console.WriteLine("1. Add Subscription");
        Console.WriteLine("2. View Subscriptions");
        Console.WriteLine("3. Check Upcoming Renewals");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddSubscription()
    {
        Console.Write("Enter subscription name: ");
        string name = Console.ReadLine();

        Console.Write("Enter monthly cost: ");
        decimal cost;
        while (!decimal.TryParse(Console.ReadLine(), out cost))
        {
            Console.WriteLine("Invalid input. Please enter a valid decimal number.");
            Console.Write("Enter monthly cost: ");
        }

        Console.Write("Enter renewal date (yyyy-MM-dd): ");
        DateTime renewalDate;
        while (!DateTime.TryParse(Console.ReadLine(), out renewalDate))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd format.");
            Console.Write("Enter renewal date (yyyy-MM-dd): ");
        }

        var subscriptions = LoadSubscriptions();
        subscriptions.Add(new Subscription
        {
            Name = name,
            MonthlyCost = cost,
            RenewalDate = renewalDate
        });

        SaveSubscriptions(subscriptions);
        Console.WriteLine("Subscription added successfully.");
    }

    private void ViewSubscriptions()
    {
        var subscriptions = LoadSubscriptions();

        if (subscriptions.Count == 0)
        {
            Console.WriteLine("No subscriptions found.");
            return;
        }

        Console.WriteLine("\nCurrent Subscriptions:");
        foreach (var sub in subscriptions)
        {
            Console.WriteLine("Name: " + sub.Name);
            Console.WriteLine("Cost: " + sub.MonthlyCost.ToString("C"));
            Console.WriteLine("Renewal Date: " + sub.RenewalDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("-----");
        }
    }

    private void CheckRenewals()
    {
        var subscriptions = LoadSubscriptions();
        var today = DateTime.Today;
        var upcomingRenewals = new List<Subscription>();

        foreach (var sub in subscriptions)
        {
            if (sub.RenewalDate >= today && sub.RenewalDate <= today.AddDays(30))
            {
                upcomingRenewals.Add(sub);
            }
        }

        if (upcomingRenewals.Count == 0)
        {
            Console.WriteLine("No upcoming renewals in the next 30 days.");
            return;
        }

        Console.WriteLine("\nUpcoming Renewals (next 30 days):");
        foreach (var sub in upcomingRenewals)
        {
            Console.WriteLine("Name: " + sub.Name);
            Console.WriteLine("Cost: " + sub.MonthlyCost.ToString("C"));
            Console.WriteLine("Renewal Date: " + sub.RenewalDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("Days until renewal: " + (sub.RenewalDate - today).Days);
            Console.WriteLine("-----");
        }
    }

    private List<Subscription> LoadSubscriptions()
    {
        try
        {
            var json = File.ReadAllText(_subscriptionsFilePath);
            return JsonSerializer.Deserialize<List<Subscription>>(json) ?? new List<Subscription>();
        }
        catch
        {
            return new List<Subscription>();
        }
    }

    private void SaveSubscriptions(List<Subscription> subscriptions)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(subscriptions, options);
        File.WriteAllText(_subscriptionsFilePath, json);
    }

    private class Subscription
    {
        public string Name { get; set; }
        public decimal MonthlyCost { get; set; }
        public DateTime RenewalDate { get; set; }
    }
}