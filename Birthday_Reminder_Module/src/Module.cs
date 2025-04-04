using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class BirthdayReminderModule : IGeneratedModule
{
    public string Name { get; set; } = "Birthday Reminder Module";
    
    private string _dataFilePath;
    
    private List<Birthday> _birthdays;
    
    public BirthdayReminderModule()
    {
        _birthdays = new List<Birthday>();
    }
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "birthdays.json");
        
        Console.WriteLine("Birthday Reminder Module is running.");
        Console.WriteLine("Loading birthdays...");
        
        LoadBirthdays();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add a birthday");
            Console.WriteLine("2. List all birthdays");
            Console.WriteLine("3. Check upcoming birthdays");
            Console.WriteLine("4. Exit");
            
            Console.Write("Enter your choice: ");
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddBirthday();
                    break;
                case "2":
                    ListBirthdays();
                    break;
                case "3":
                    CheckUpcomingBirthdays();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveBirthdays();
        Console.WriteLine("Birthday Reminder Module is shutting down.");
        
        return true;
    }
    
    private void LoadBirthdays()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _birthdays = JsonSerializer.Deserialize<List<Birthday>>(json);
            Console.WriteLine("Birthdays loaded successfully.");
        }
        else
        {
            Console.WriteLine("No existing birthday data found.");
        }
    }
    
    private void SaveBirthdays()
    {
        string json = JsonSerializer.Serialize(_birthdays);
        File.WriteAllText(_dataFilePath, json);
        Console.WriteLine("Birthdays saved successfully.");
    }
    
    private void AddBirthday()
    {
        Console.Write("Enter name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter birth date (yyyy-MM-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime birthDate))
        {
            _birthdays.Add(new Birthday { Name = name, BirthDate = birthDate });
            Console.WriteLine("Birthday added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format. Birthday not added.");
        }
    }
    
    private void ListBirthdays()
    {
        if (_birthdays.Count == 0)
        {
            Console.WriteLine("No birthdays stored.");
            return;
        }
        
        Console.WriteLine("\nBirthdays:");
        foreach (var birthday in _birthdays.OrderBy(b => b.BirthDate))
        {
            Console.WriteLine($"{birthday.Name}: {birthday.BirthDate:yyyy-MM-dd}");
        }
    }
    
    private void CheckUpcomingBirthdays()
    {
        DateTime today = DateTime.Today;
        DateTime nextWeek = today.AddDays(7);
        
        var upcoming = _birthdays
            .Where(b => b.BirthDate.Month == today.Month && b.BirthDate.Day >= today.Day && b.BirthDate.Day <= nextWeek.Day)
            .OrderBy(b => b.BirthDate)
            .ToList();
            
        if (upcoming.Count == 0)
        {
            Console.WriteLine("No upcoming birthdays in the next week.");
        }
        else
        {
            Console.WriteLine("\nUpcoming birthdays in the next week:");
            foreach (var birthday in upcoming)
            {
                Console.WriteLine($"{birthday.Name}: {birthday.BirthDate:yyyy-MM-dd}");
            }
        }
    }
}

public class Birthday
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
}