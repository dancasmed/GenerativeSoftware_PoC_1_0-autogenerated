namespace GenerativeSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class HostelManager : IGeneratedModule
{
    public string Name { get; set; } = "HostelManager";
    
    public bool Main(string dataFolder)
    {
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        Console.WriteLine("Hostel Management System\n---------------------------");

        while (true)
        {
            Console.WriteLine("1. Register Guest\n2. Allocate Room\n3. View Guests\n4. Release Room\n5. Exit");
            Console.Write("Choose an option: ");
            int option = Convert.ToInt32(Console.ReadLine());

            switch (option)
            {
                case 1:
                    RegisterGuest(dataFolder);
                    break;
                case 2:
                    AllocateRoom(dataFolder);
                    break;
                case 3:
                    ViewGuests(dataFolder);
                    break;
                case 4:
                    ReleaseRoom(dataFolder);
                    break;
                case 5:
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
        }
    }

    private void RegisterGuest(string dataFolder)
    {
        Console.Write("Enter guest name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter guest email: ");
        string email = Console.ReadLine();
        
        Console.Write("Enter room number (if known): ");
        string roomNumber = Console.ReadLine() ?? "";

        Guest newGuest = new Guest
        {
            Name = name,
            Email = email,
            RoomNumber = roomNumber,
            CheckInDate = DateTime.Now.ToString("yyyy-MM-dd")
        };

        string json = JsonSerializer.Serialize(newGuest);
        
        if (!string.IsNullOrEmpty(roomNumber))
        {
            string filePath = Path.Combine(dataFolder, "guests.json"));
            List<Guest> guests = File.Exists(filePath) ? JsonSerializer.Deserialize<List<Guest>>(File.ReadAllText(filePath)) : new List<Guest>();
            guests.Add(newGuest);
            File.WriteAllText(filePath, JsonSerializer.Serialize(guests, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    private void AllocateRoom(string dataFolder)
    {
        Console.Write("Enter guest email: ");
        string email = Console.ReadLine();
        
        string filePath = Path.Combine(dataFolder, "guests.json"));
        if (!File.Exists(filePath))
        {
            Console.WriteLine("No guests registered.");
            return;
        }

        List<Guest> guests = JsonSerializer.Deserialize<List<Guest>>(File.ReadAllText(filePath));
        var guest = guests.FirstOrDefault(g => g.Email == email);

        if (guest == null)
        {
            Console.WriteLine("Guest not found.");
            return;
        }

        if (!string.IsNullOrEmpty(guest.RoomNumber))
        {
            Console.WriteLine("Guest already has a room allocated.");
            return;
        }

        Console.Write("Enter room number to allocate: ");
        string newRoom = Console.ReadLine();

        guest.RoomNumber = newRoom;
        guests = guests.Select(g => g.Email == email ? guest : g).ToList();

        File.WriteAllText(filePath, JsonSerializer.Serialize(guests, new JsonSerializerOptions { WriteIndented = true }));
        
        Console.WriteLine("Room allocated successfully.");
    }

    private void ViewGuests(string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, "guests.json"));
        if (!File.Exists(filePath))
        {
            Console.WriteLine("No guests registered.");
            return;
        }

        List<Guest> guests = JsonSerializer.Deserialize<List<Guest>>(File.ReadAllText(filePath));
        
        foreach (var guest in guests)
        {
            Console.WriteLine($"Name: {guest.Name}");
            Console.WriteLine($"Email: {guest.Email}");
            Console.WriteLine($"Room Number: {guest.RoomNumber}");
            Console.WriteLine($"Check In Date: {guest.CheckInDate}");
            Console.WriteLine("------------------------");
        }
    }

    private void ReleaseRoom(string dataFolder)
    {
        Console.Write("Enter guest email: ");
        string email = Console.ReadLine();
        
        string filePath = Path.Combine(dataFolder, "guests.json"));
        if (!File.Exists(filePath))
        {
            Console.WriteLine("No guests registered.");
            return;
        }

        List<Guest> guests = JsonSerializer.Deserialize<List<Guest>>(File.ReadAllText(filePath));
        var guest = guests.FirstOrDefault(g => g.Email == email);

        if (guest == null)
        {
            Console.WriteLine("Guest not found.");
            return;
        }

        if (string.IsNullOrEmpty(guest.RoomNumber))
        {
            Console.WriteLine("Guest does not have a room allocated.");
            return;
        }

        guest.RoomNumber = string.Empty;
        guests = guests.Select(g => g.Email == email ? guest : g).ToList();

        File.WriteAllText(filePath, JsonSerializer.Serialize(guests, new JsonSerializerOptions { WriteIndented = true }));
        
        Console.WriteLine("Room released successfully.");
    }
}

public class Guest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string RoomNumber { get; set; }
    public string CheckInDate { get; set; }
}