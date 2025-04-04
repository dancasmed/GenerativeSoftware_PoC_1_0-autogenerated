using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class EventGuestManager : IGeneratedModule
{
    public string Name { get; set; } = "Event Guest Manager";
    
    private string _guestListPath;
    private List<Guest> _guests;
    
    public EventGuestManager()
    {
        _guests = new List<Guest>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Event Guest Manager...");
        
        _guestListPath = Path.Combine(dataFolder, "guests.json");
        
        try
        {
            LoadGuests();
            
            Console.WriteLine("Guest list loaded successfully.");
            Console.WriteLine("Current guest count: " + _guests.Count);
            
            // Example operations (in a real app, you'd have a menu system)
            AddGuest("John Doe", true);
            AddGuest("Jane Smith", false);
            
            SaveGuests();
            
            Console.WriteLine("Guest management operations completed.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void LoadGuests()
    {
        if (File.Exists(_guestListPath))
        {
            string json = File.ReadAllText(_guestListPath);
            _guests = JsonSerializer.Deserialize<List<Guest>>(json);
        }
    }
    
    private void SaveGuests()
    {
        string json = JsonSerializer.Serialize(_guests);
        File.WriteAllText(_guestListPath, json);
    }
    
    public void AddGuest(string name, bool isAttending)
    {
        _guests.Add(new Guest { Name = name, IsAttending = isAttending });
        Console.WriteLine("Added guest: " + name + " - RSVP: " + (isAttending ? "Yes" : "No"));
    }
    
    public class Guest
    {
        public string Name { get; set; }
        public bool IsAttending { get; set; }
    }
}