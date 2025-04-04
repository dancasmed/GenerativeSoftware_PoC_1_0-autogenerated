using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CalendarModule : IGeneratedModule
{
    public string Name { get; set; } = "Simple Calendar Module";
    
    private string _dataFilePath;
    
    public CalendarModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "calendar_events.json");
        
        Console.WriteLine("Simple Calendar Module is running");
        Console.WriteLine("Type 'help' for available commands");
        
        LoadEvents();
        
        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine().Trim();
            
            if (string.IsNullOrEmpty(input))
                continue;
                
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;
                
            ProcessCommand(input);
        }
        
        return true;
    }
    
    private List<CalendarEvent> _events = new List<CalendarEvent>();
    
    private void ProcessCommand(string command)
    {
        string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        switch (parts[0].ToLower())
        {
            case "help":
                ShowHelp();
                break;
                
            case "add":
                if (parts.Length >= 3)
                {
                    if (DateTime.TryParse(parts[1], out DateTime date))
                    {
                        string description = string.Join(' ', parts, 2, parts.Length - 2);
                        AddEvent(date, description);
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Use yyyy-mm-dd");
                    }
                }
                else
                {
                    Console.WriteLine("Usage: add <date> <description>");
                }
                break;
                
            case "list":
                ListEvents();
                break;
                
            case "today":
                ShowEventsForDate(DateTime.Today);
                break;
                
            case "delete":
                if (parts.Length == 2 && int.TryParse(parts[1], out int id))
                {
                    DeleteEvent(id);
                }
                else
                {
                    Console.WriteLine("Usage: delete <event_id>");
                }
                break;
                
            default:
                Console.WriteLine("Unknown command. Type 'help' for available commands.");
                break;
        }
    }
    
    private void ShowHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  add <date> <description> - Add a new event");
        Console.WriteLine("  list - List all events");
        Console.WriteLine("  today - Show today's events");
        Console.WriteLine("  delete <event_id> - Delete an event");
        Console.WriteLine("  exit - Exit the calendar");
    }
    
    private void AddEvent(DateTime date, string description)
    {
        var newEvent = new CalendarEvent
        {
            Id = _events.Count > 0 ? _events.Max(e => e.Id) + 1 : 1,
            Date = date,
            Description = description
        };
        
        _events.Add(newEvent);
        SaveEvents();
        Console.WriteLine("Event added successfully");
    }
    
    private void ListEvents()
    {
        if (_events.Count == 0)
        {
            Console.WriteLine("No events found");
            return;
        }
        
        foreach (var evt in _events.OrderBy(e => e.Date))
        {
            Console.WriteLine($"{evt.Id}. {evt.Date:yyyy-MM-dd}: {evt.Description}");
        }
    }
    
    private void ShowEventsForDate(DateTime date)
    {
        var events = _events.Where(e => e.Date.Date == date.Date).OrderBy(e => e.Date);
        
        if (!events.Any())
        {
            Console.WriteLine("No events for " + date.ToString("yyyy-MM-dd"));
            return;
        }
        
        Console.WriteLine("Events for " + date.ToString("yyyy-MM-dd") + ":");
        foreach (var evt in events)
        {
            Console.WriteLine($"{evt.Id}. {evt.Description}");
        }
    }
    
    private void DeleteEvent(int id)
    {
        var evt = _events.FirstOrDefault(e => e.Id == id);
        
        if (evt == null)
        {
            Console.WriteLine("Event not found");
            return;
        }
        
        _events.Remove(evt);
        SaveEvents();
        Console.WriteLine("Event deleted successfully");
    }
    
    private void LoadEvents()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _events = JsonSerializer.Deserialize<List<CalendarEvent>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading events: " + ex.Message);
        }
    }
    
    private void SaveEvents()
    {
        try
        {
            string json = JsonSerializer.Serialize(_events);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving events: " + ex.Message);
        }
    }
}

public class CalendarEvent
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
}