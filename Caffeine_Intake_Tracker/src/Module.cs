using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CaffeineTrackerModule
{
    public string Name { get; set; } = "Caffeine Intake Tracker";
    
    private string dataFilePath;
    
    public CaffeineTrackerModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Caffeine Intake Tracker module is running...");
        
        dataFilePath = Path.Combine(dataFolder, "caffeine_intake.json");
        
        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add caffeine intake");
            Console.WriteLine("2. View today's intake");
            Console.WriteLine("3. View weekly summary");
            Console.WriteLine("4. Exit module");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            if (!int.TryParse(input, out int option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }
            
            switch (option)
            {
                case 1:
                    AddCaffeineIntake();
                    break;
                case 2:
                    ViewTodaysIntake();
                    break;
                case 3:
                    ViewWeeklySummary();
                    break;
                case 4:
                    Console.WriteLine("Exiting Caffeine Intake Tracker module...");
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private void AddCaffeineIntake()
    {
        Console.Write("Enter caffeine amount (mg): ");
        if (!int.TryParse(Console.ReadLine(), out int amount))
        {
            Console.WriteLine("Invalid amount. Please enter a number.");
            return;
        }
        
        Console.Write("Enter source (e.g., Coffee, Tea, Soda): ");
        string source = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(source))
        {
            Console.WriteLine("Source cannot be empty.");
            return;
        }
        
        var intake = new CaffeineIntake
        {
            Date = DateTime.Now,
            Amount = amount,
            Source = source
        };
        
        var allIntakes = LoadIntakes();
        allIntakes.Add(intake);
        
        SaveIntakes(allIntakes);
        
        Console.WriteLine("Caffeine intake recorded successfully.");
    }
    
    private void ViewTodaysIntake()
    {
        var today = DateTime.Now.Date;
        var allIntakes = LoadIntakes();
        var todaysIntakes = allIntakes.FindAll(i => i.Date.Date == today);
        
        if (todaysIntakes.Count == 0)
        {
            Console.WriteLine("No caffeine intake recorded for today.");
            return;
        }
        
        Console.WriteLine("\nToday's Caffeine Intake:");
        Console.WriteLine("------------------------");
        
        int total = 0;
        foreach (var intake in todaysIntakes)
        {
            Console.WriteLine("{0}: {1}mg from {2}", intake.Date.ToString("HH:mm"), intake.Amount, intake.Source);
            total += intake.Amount;
        }
        
        Console.WriteLine("\nTotal for today: {0}mg", total);
    }
    
    private void ViewWeeklySummary()
    {
        var endDate = DateTime.Now.Date;
        var startDate = endDate.AddDays(-6);
        var allIntakes = LoadIntakes();
        
        Console.WriteLine("\nWeekly Caffeine Intake Summary:");
        Console.WriteLine("--------------------------------");
        
        var dailyTotals = new Dictionary<DateTime, int>();
        
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            dailyTotals[date] = 0;
        }
        
        foreach (var intake in allIntakes)
        {
            var intakeDate = intake.Date.Date;
            if (intakeDate >= startDate && intakeDate <= endDate)
            {
                dailyTotals[intakeDate] += intake.Amount;
            }
        }
        
        foreach (var kvp in dailyTotals)
        {
            Console.WriteLine("{0}: {1}mg", kvp.Key.ToString("ddd, MMM dd"), kvp.Value);
        }
        
        int weeklyTotal = dailyTotals.Values.Sum();
        Console.WriteLine("\nTotal for the week: {0}mg", weeklyTotal);
    }
    
    private List<CaffeineIntake> LoadIntakes()
    {
        if (!File.Exists(dataFilePath))
        {
            return new List<CaffeineIntake>();
        }
        
        var json = File.ReadAllText(dataFilePath);
        return JsonSerializer.Deserialize<List<CaffeineIntake>>(json) ?? new List<CaffeineIntake>();
    }
    
    private void SaveIntakes(List<CaffeineIntake> intakes)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(intakes, options);
        File.WriteAllText(dataFilePath, json);
    }
}

public class CaffeineIntake
{
    public DateTime Date { get; set; }
    public int Amount { get; set; }
    public string Source { get; set; }
}