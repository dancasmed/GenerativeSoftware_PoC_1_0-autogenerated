using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MuseumExhibitionManager : IGeneratedModule
{
    public string Name { get; set; } = "Museum Exhibition Manager";

    private string exhibitionsFilePath;
    private string visitorsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Museum Exhibition Manager...");

        exhibitionsFilePath = Path.Combine(dataFolder, "exhibitions.json");
        visitorsFilePath = Path.Combine(dataFolder, "visitors.json");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nMuseum Exhibition Management System");
            Console.WriteLine("1. Add Exhibition");
            Console.WriteLine("2. List Exhibitions");
            Console.WriteLine("3. Add Visitor");
            Console.WriteLine("4. List Visitors");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddExhibition();
                    break;
                case "2":
                    ListExhibitions();
                    break;
                case "3":
                    AddVisitor();
                    break;
                case "4":
                    ListVisitors();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Saving data and exiting Museum Exhibition Manager...");
        return true;
    }

    private void AddExhibition()
    {
        Console.Write("Enter exhibition name: ");
        string name = Console.ReadLine();

        Console.Write("Enter start date (yyyy-MM-dd): ");
        DateTime startDate;
        while (!DateTime.TryParse(Console.ReadLine(), out startDate))
        {
            Console.Write("Invalid date format. Please enter start date (yyyy-MM-dd): ");
        }

        Console.Write("Enter end date (yyyy-MM-dd): ");
        DateTime endDate;
        while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate < startDate)
        {
            Console.Write("Invalid date format or end date before start date. Please enter end date (yyyy-MM-dd): ");
        }

        Console.Write("Enter description: ");
        string description = Console.ReadLine();

        var exhibitions = LoadExhibitions();
        exhibitions.Add(new Exhibition
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartDate = startDate,
            EndDate = endDate,
            Description = description
        });

        SaveExhibitions(exhibitions);
        Console.WriteLine("Exhibition added successfully.");
    }

    private void ListExhibitions()
    {
        var exhibitions = LoadExhibitions();

        if (exhibitions.Count == 0)
        {
            Console.WriteLine("No exhibitions found.");
            return;
        }

        Console.WriteLine("\nCurrent Exhibitions:");
        foreach (var exhibition in exhibitions)
        {
            Console.WriteLine($"ID: {exhibition.Id}");
            Console.WriteLine($"Name: {exhibition.Name}");
            Console.WriteLine($"Dates: {exhibition.StartDate:yyyy-MM-dd} to {exhibition.EndDate:yyyy-MM-dd}");
            Console.WriteLine($"Description: {exhibition.Description}");
            Console.WriteLine();
        }
    }

    private void AddVisitor()
    {
        Console.Write("Enter visitor name: ");
        string name = Console.ReadLine();

        Console.Write("Enter email: ");
        string email = Console.ReadLine();

        Console.Write("Enter visit date (yyyy-MM-dd): ");
        DateTime visitDate;
        while (!DateTime.TryParse(Console.ReadLine(), out visitDate))
        {
            Console.Write("Invalid date format. Please enter visit date (yyyy-MM-dd): ");
        }

        var exhibitions = LoadExhibitions();
        if (exhibitions.Count == 0)
        {
            Console.WriteLine("No exhibitions available to visit.");
            return;
        }

        ListExhibitions();
        Console.Write("Enter exhibition ID to visit: ");
        string exhibitionIdInput = Console.ReadLine();
        if (!Guid.TryParse(exhibitionIdInput, out Guid exhibitionId))
        {
            Console.WriteLine("Invalid exhibition ID format.");
            return;
        }

        var visitors = LoadVisitors();
        visitors.Add(new Visitor
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            VisitDate = visitDate,
            ExhibitionId = exhibitionId
        });

        SaveVisitors(visitors);
        Console.WriteLine("Visitor added successfully.");
    }

    private void ListVisitors()
    {
        var visitors = LoadVisitors();
        var exhibitions = LoadExhibitions();

        if (visitors.Count == 0)
        {
            Console.WriteLine("No visitors found.");
            return;
        }

        Console.WriteLine("\nVisitor Records:");
        foreach (var visitor in visitors)
        {
            var exhibition = exhibitions.Find(e => e.Id == visitor.ExhibitionId);
            string exhibitionName = exhibition != null ? exhibition.Name : "[Unknown Exhibition]";

            Console.WriteLine($"ID: {visitor.Id}");
            Console.WriteLine($"Name: {visitor.Name}");
            Console.WriteLine($"Email: {visitor.Email}");
            Console.WriteLine($"Visit Date: {visitor.VisitDate:yyyy-MM-dd}");
            Console.WriteLine($"Exhibition: {exhibitionName}");
            Console.WriteLine();
        }
    }

    private List<Exhibition> LoadExhibitions()
    {
        if (!File.Exists(exhibitionsFilePath))
        {
            return new List<Exhibition>();
        }

        string json = File.ReadAllText(exhibitionsFilePath);
        return JsonSerializer.Deserialize<List<Exhibition>>(json) ?? new List<Exhibition>();
    }

    private void SaveExhibitions(List<Exhibition> exhibitions)
    {
        string json = JsonSerializer.Serialize(exhibitions);
        File.WriteAllText(exhibitionsFilePath, json);
    }

    private List<Visitor> LoadVisitors()
    {
        if (!File.Exists(visitorsFilePath))
        {
            return new List<Visitor>();
        }

        string json = File.ReadAllText(visitorsFilePath);
        return JsonSerializer.Deserialize<List<Visitor>>(json) ?? new List<Visitor>();
    }

    private void SaveVisitors(List<Visitor> visitors)
    {
        string json = JsonSerializer.Serialize(visitors);
        File.WriteAllText(visitorsFilePath, json);
    }
}

public class Exhibition
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; }
}

public class Visitor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime VisitDate { get; set; }
    public Guid ExhibitionId { get; set; }
}