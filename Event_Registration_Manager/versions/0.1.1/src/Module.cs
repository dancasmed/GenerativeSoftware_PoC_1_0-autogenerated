using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class EventRegistrationModule : IGeneratedModule
{
    public string Name { get; set; } = "Event Registration Manager";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Event Registration Module is running...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        string eventsFilePath = Path.Combine(dataFolder, "events.json");
        string attendeesFilePath = Path.Combine(dataFolder, "attendees.json");

        List<Event> events = LoadEvents(eventsFilePath);
        List<Attendee> attendees = LoadAttendees(attendeesFilePath);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nEvent Registration System");
            Console.WriteLine("1. Create New Event");
            Console.WriteLine("2. Register Attendee");
            Console.WriteLine("3. List All Events");
            Console.WriteLine("4. List Attendees for Event");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                switch (choice)
                {
                    case 1:
                        CreateEvent(events, eventsFilePath);
                        break;
                    case 2:
                        RegisterAttendee(events, attendees, attendeesFilePath);
                        break;
                    case 3:
                        ListEvents(events);
                        break;
                    case 4:
                        ListEventAttendees(events, attendees);
                        break;
                    case 5:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }

        Console.WriteLine("Event Registration Module completed.");
        return true;
    }

    private List<Event> LoadEvents(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
        }
        return new List<Event>();
    }

    private List<Attendee> LoadAttendees(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Attendee>>(json) ?? new List<Attendee>();
        }
        return new List<Attendee>();
    }

    private void SaveEvents(List<Event> events, string filePath)
    {
        string json = JsonSerializer.Serialize(events);
        File.WriteAllText(filePath, json);
    }

    private void SaveAttendees(List<Attendee> attendees, string filePath)
    {
        string json = JsonSerializer.Serialize(attendees);
        File.WriteAllText(filePath, json);
    }

    private void CreateEvent(List<Event> events, string filePath)
    {
        Console.Write("Enter event name: ");
        string name = Console.ReadLine();

        Console.Write("Enter event date (yyyy-MM-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.Write("Enter event location: ");
            string location = Console.ReadLine();

            Console.Write("Enter maximum attendees: ");
            if (int.TryParse(Console.ReadLine(), out int maxAttendees))
            {
                Event newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Date = date,
                    Location = location,
                    MaxAttendees = maxAttendees
                };

                events.Add(newEvent);
                SaveEvents(events, filePath);
                Console.WriteLine("Event created successfully.");
            }
            else
            {
                Console.WriteLine("Invalid maximum attendees value.");
            }
        }
        else
        {
            Console.WriteLine("Invalid date format.");
        }
    }

    private void RegisterAttendee(List<Event> events, List<Attendee> attendees, string filePath)
    {
        if (events.Count == 0)
        {
            Console.WriteLine("No events available for registration.");
            return;
        }

        ListEvents(events);
        Console.Write("Select event number: ");
        if (int.TryParse(Console.ReadLine(), out int eventIndex) && eventIndex > 0 && eventIndex <= events.Count)
        {
            Event selectedEvent = events[eventIndex - 1];

            Console.Write("Enter attendee name: ");
            string name = Console.ReadLine();

            Console.Write("Enter attendee email: ");
            string email = Console.ReadLine();

            int currentAttendees = attendees.Count(a => a.EventId == selectedEvent.Id);
            if (currentAttendees >= selectedEvent.MaxAttendees)
            {
                Console.WriteLine("Event is full. Cannot register more attendees.");
                return;
            }

            Attendee newAttendee = new Attendee
            {
                Id = Guid.NewGuid(),
                EventId = selectedEvent.Id,
                Name = name,
                Email = email,
                RegistrationDate = DateTime.Now
            };

            attendees.Add(newAttendee);
            SaveAttendees(attendees, filePath);
            Console.WriteLine("Attendee registered successfully.");
        }
        else
        {
            Console.WriteLine("Invalid event selection.");
        }
    }

    private void ListEvents(List<Event> events)
    {
        if (events.Count == 0)
        {
            Console.WriteLine("No events available.");
            return;
        }

        Console.WriteLine("\nAvailable Events:");
        for (int i = 0; i < events.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {events[i].Name} - {events[i].Date:yyyy-MM-dd} at {events[i].Location} (Max: {events[i].MaxAttendees})");
        }
    }

    private void ListEventAttendees(List<Event> events, List<Attendee> attendees)
    {
        if (events.Count == 0)
        {
            Console.WriteLine("No events available.");
            return;
        }

        ListEvents(events);
        Console.Write("Select event number: ");
        if (int.TryParse(Console.ReadLine(), out int eventIndex) && eventIndex > 0 && eventIndex <= events.Count)
        {
            Event selectedEvent = events[eventIndex - 1];
            var eventAttendees = attendees.Where(a => a.EventId == selectedEvent.Id).ToList();

            if (eventAttendees.Count == 0)
            {
                Console.WriteLine("No attendees registered for this event.");
                return;
            }

            Console.WriteLine($"\nAttendees for {selectedEvent.Name}:");
            foreach (var attendee in eventAttendees)
            {
                Console.WriteLine($"- {attendee.Name} ({attendee.Email}) registered on {attendee.RegistrationDate:yyyy-MM-dd}");
            }
        }
        else
        {
            Console.WriteLine("Invalid event selection.");
        }
    }
}

public class Event
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public int MaxAttendees { get; set; }
}

public class Attendee
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
}