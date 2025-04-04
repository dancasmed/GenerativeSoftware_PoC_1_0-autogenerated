using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GuestListManager : IGeneratedModule
{
    public string Name { get; set; } = "Guest List Manager";
    
    private string guestListFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Guest List Manager module is running.");
        
        guestListFilePath = Path.Combine(dataFolder, "guestlist.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddGuest();
                    break;
                case "2":
                    UpdateRSVP();
                    break;
                case "3":
                    ListGuests();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Guest List Manager module is shutting down.");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nGuest List Manager");
        Console.WriteLine("1. Add Guest");
        Console.WriteLine("2. Update RSVP Status");
        Console.WriteLine("3. List All Guests");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private List<Guest> LoadGuestList()
    {
        if (!File.Exists(guestListFilePath))
        {
            return new List<Guest>();
        }
        
        string json = File.ReadAllText(guestListFilePath);
        return JsonSerializer.Deserialize<List<Guest>>(json) ?? new List<Guest>();
    }
    
    private void SaveGuestList(List<Guest> guests)
    {
        string json = JsonSerializer.Serialize(guests);
        File.WriteAllText(guestListFilePath, json);
    }
    
    private void AddGuest()
    {
        Console.Write("Enter guest name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter guest email: ");
        string email = Console.ReadLine();
        
        var guests = LoadGuestList();
        guests.Add(new Guest { Name = name, Email = email, RSVPStatus = "Pending" });
        SaveGuestList(guests);
        
        Console.WriteLine("Guest added successfully.");
    }
    
    private void UpdateRSVP()
    {
        var guests = LoadGuestList();
        if (guests.Count == 0)
        {
            Console.WriteLine("No guests in the list.");
            return;
        }
        
        Console.WriteLine("Select a guest to update:");
        for (int i = 0; i < guests.Count; i++)
        {
            Console.WriteLine(string.Format("{0}. {1} ({2}) - {3}", i + 1, guests[i].Name, guests[i].Email, guests[i].RSVPStatus));
        }
        
        if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= guests.Count)
        {
            Console.Write("Enter new RSVP status (Confirmed/Declined/Pending): ");
            string newStatus = Console.ReadLine();
            
            guests[selection - 1].RSVPStatus = newStatus;
            SaveGuestList(guests);
            Console.WriteLine("RSVP status updated successfully.");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }
    
    private void ListGuests()
    {
        var guests = LoadGuestList();
        if (guests.Count == 0)
        {
            Console.WriteLine("No guests in the list.");
            return;
        }
        
        Console.WriteLine("\nGuest List:");
        foreach (var guest in guests)
        {
            Console.WriteLine(string.Format("Name: {0}, Email: {1}, RSVP: {2}", guest.Name, guest.Email, guest.RSVPStatus));
        }
    }
}

public class Guest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string RSVPStatus { get; set; }
}