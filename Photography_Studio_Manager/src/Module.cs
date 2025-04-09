using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PhotographyStudioManager : IGeneratedModule
{
    public string Name { get; set; } = "Photography Studio Manager";

    private string bookingsFilePath;
    private string clientsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Photography Studio Manager...");

        bookingsFilePath = Path.Combine(dataFolder, "bookings.json");
        clientsFilePath = Path.Combine(dataFolder, "clients.json");

        EnsureDataFilesExist();

        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddNewClient();
                    break;
                case "2":
                    AddNewBooking();
                    break;
                case "3":
                    ViewAllClients();
                    break;
                case "4":
                    ViewAllBookings();
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Photography Studio Manager is shutting down...");
        return true;
    }

    private void EnsureDataFilesExist()
    {
        if (!File.Exists(bookingsFilePath))
        {
            File.WriteAllText(bookingsFilePath, "[]");
        }

        if (!File.Exists(clientsFilePath))
        {
            File.WriteAllText(clientsFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPhotography Studio Manager");
        Console.WriteLine("1. Add New Client");
        Console.WriteLine("2. Add New Booking");
        Console.WriteLine("3. View All Clients");
        Console.WriteLine("4. View All Bookings");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice: ");
    }

    private void AddNewClient()
    {
        Console.Write("Enter client name: ");
        var name = Console.ReadLine();

        Console.Write("Enter client email: ");
        var email = Console.ReadLine();

        Console.Write("Enter client phone: ");
        var phone = Console.ReadLine();

        var client = new Client
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Email = email,
            Phone = phone,
            RegistrationDate = DateTime.Now
        };

        var clients = LoadClients();
        clients.Add(client);
        SaveClients(clients);

        Console.WriteLine("Client added successfully.");
    }

    private void AddNewBooking()
    {
        ViewAllClients();
        Console.Write("Enter client ID: ");
        var clientId = Console.ReadLine();

        Console.Write("Enter booking date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out var bookingDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        Console.Write("Enter session type: ");
        var sessionType = Console.ReadLine();

        Console.Write("Enter session duration (hours): ");
        if (!decimal.TryParse(Console.ReadLine(), out var duration))
        {
            Console.WriteLine("Invalid duration.");
            return;
        }

        Console.Write("Enter notes: ");
        var notes = Console.ReadLine();

        var booking = new Booking
        {
            Id = Guid.NewGuid().ToString(),
            ClientId = clientId,
            BookingDate = bookingDate,
            SessionType = sessionType,
            Duration = duration,
            Notes = notes,
            CreatedDate = DateTime.Now
        };

        var bookings = LoadBookings();
        bookings.Add(booking);
        SaveBookings(bookings);

        Console.WriteLine("Booking added successfully.");
    }

    private void ViewAllClients()
    {
        var clients = LoadClients();
        Console.WriteLine("\nAll Clients:");
        foreach (var client in clients)
        {
            Console.WriteLine($"ID: {client.Id}, Name: {client.Name}, Email: {client.Email}, Phone: {client.Phone}, Registered: {client.RegistrationDate:yyyy-MM-dd}");
        }
    }

    private void ViewAllBookings()
    {
        var bookings = LoadBookings();
        var clients = LoadClients();

        Console.WriteLine("\nAll Bookings:");
        foreach (var booking in bookings)
        {
            var client = clients.Find(c => c.Id == booking.ClientId);
            var clientName = client != null ? client.Name : "Unknown Client";

            Console.WriteLine($"ID: {booking.Id}, Client: {clientName}, Date: {booking.BookingDate:yyyy-MM-dd}, Type: {booking.SessionType}, Duration: {booking.Duration} hours, Notes: {booking.Notes}");
        }
    }

    private List<Client> LoadClients()
    {
        var json = File.ReadAllText(clientsFilePath);
        return JsonSerializer.Deserialize<List<Client>>(json) ?? new List<Client>();
    }

    private void SaveClients(List<Client> clients)
    {
        var json = JsonSerializer.Serialize(clients);
        File.WriteAllText(clientsFilePath, json);
    }

    private List<Booking> LoadBookings()
    {
        var json = File.ReadAllText(bookingsFilePath);
        return JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
    }

    private void SaveBookings(List<Booking> bookings)
    {
        var json = JsonSerializer.Serialize(bookings);
        File.WriteAllText(bookingsFilePath, json);
    }
}

public class Client
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime RegistrationDate { get; set; }
}

public class Booking
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public DateTime BookingDate { get; set; }
    public string SessionType { get; set; }
    public decimal Duration { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedDate { get; set; }
}