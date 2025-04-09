using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ITSupportTicketManager : IGeneratedModule
{
    public string Name { get; set; } = "IT Support Ticket Manager";

    private string _ticketsFilePath;
    private List<SupportTicket> _tickets;

    public ITSupportTicketManager()
    {
        _tickets = new List<SupportTicket>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("IT Support Ticket Manager module is running...");
        
        _ticketsFilePath = Path.Combine(dataFolder, "support_tickets.json");
        
        try
        {
            LoadTickets();
            
            bool exitRequested = false;
            while (!exitRequested)
            {
                DisplayMenu();
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        CreateNewTicket();
                        break;
                    case "2":
                        ViewAllTickets();
                        break;
                    case "3":
                        UpdateTicketStatus();
                        break;
                    case "4":
                        exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveTickets();
            Console.WriteLine("All changes have been saved. Exiting IT Support Ticket Manager.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadTickets()
    {
        if (File.Exists(_ticketsFilePath))
        {
            var json = File.ReadAllText(_ticketsFilePath);
            _tickets = JsonSerializer.Deserialize<List<SupportTicket>>(json);
            Console.WriteLine("Tickets loaded successfully.");
        }
        else
        {
            Console.WriteLine("No existing tickets found. Starting with empty ticket list.");
        }
    }

    private void SaveTickets()
    {
        var json = JsonSerializer.Serialize(_tickets, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_ticketsFilePath, json);
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nIT Support Ticket Manager");
        Console.WriteLine("1. Create new ticket");
        Console.WriteLine("2. View all tickets");
        Console.WriteLine("3. Update ticket status");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void CreateNewTicket()
    {
        Console.Write("Enter ticket description: ");
        var description = Console.ReadLine();
        
        Console.Write("Enter requester name: ");
        var requester = Console.ReadLine();
        
        var newTicket = new SupportTicket
        {
            Id = Guid.NewGuid().ToString(),
            Description = description,
            Requester = requester,
            Status = "Open",
            CreatedDate = DateTime.Now
        };
        
        _tickets.Add(newTicket);
        Console.WriteLine("Ticket created successfully with ID: " + newTicket.Id);
    }

    private void ViewAllTickets()
    {
        if (_tickets.Count == 0)
        {
            Console.WriteLine("No tickets found.");
            return;
        }
        
        Console.WriteLine("\nAll Support Tickets:");
        foreach (var ticket in _tickets)
        {
            Console.WriteLine("ID: " + ticket.Id);
            Console.WriteLine("Description: " + ticket.Description);
            Console.WriteLine("Requester: " + ticket.Requester);
            Console.WriteLine("Status: " + ticket.Status);
            Console.WriteLine("Created: " + ticket.CreatedDate);
            Console.WriteLine("----------------------------");
        }
    }

    private void UpdateTicketStatus()
    {
        Console.Write("Enter ticket ID to update: ");
        var ticketId = Console.ReadLine();
        
        var ticket = _tickets.Find(t => t.Id == ticketId);
        if (ticket == null)
        {
            Console.WriteLine("Ticket not found.");
            return;
        }
        
        Console.WriteLine("Current status: " + ticket.Status);
        Console.Write("Enter new status (Open/In Progress/Resolved/Closed): ");
        var newStatus = Console.ReadLine();
        
        ticket.Status = newStatus;
        Console.WriteLine("Ticket status updated successfully.");
    }
}

public class SupportTicket
{
    public string Id { get; set; }
    public string Description { get; set; }
    public string Requester { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
}