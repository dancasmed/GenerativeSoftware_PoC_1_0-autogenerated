using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HikingLogModule : IGeneratedModule
{
    public string Name { get; set; } = "Hiking Log Manager";

    private string logFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Hiking Log Manager module is running.");
        logFilePath = Path.Combine(dataFolder, "hiking_logs.json");

        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddHikingLog();
                    break;
                case "2":
                    ViewAllLogs();
                    break;
                case "3":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Exiting Hiking Log Manager.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nHiking Log Manager");
        Console.WriteLine("1. Add a new hiking log");
        Console.WriteLine("2. View all hiking logs");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
    }

    private void AddHikingLog()
    {
        Console.Write("Enter location: ");
        string location = Console.ReadLine();

        Console.Write("Enter distance (km): ");
        if (!double.TryParse(Console.ReadLine(), out double distance))
        {
            Console.WriteLine("Invalid distance value.");
            return;
        }

        Console.Write("Enter date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        var log = new HikingLog
        {
            Location = location,
            Distance = distance,
            Date = date
        };

        List<HikingLog> logs = LoadLogs();
        logs.Add(log);
        SaveLogs(logs);

        Console.WriteLine("Hiking log added successfully.");
    }

    private void ViewAllLogs()
    {
        List<HikingLog> logs = LoadLogs();

        if (logs.Count == 0)
        {
            Console.WriteLine("No hiking logs found.");
            return;
        }

        Console.WriteLine("\nAll Hiking Logs:");
        foreach (var log in logs)
        {
            Console.WriteLine($"{log.Date:yyyy-MM-dd} - {log.Location}: {log.Distance} km");
        }
    }

    private List<HikingLog> LoadLogs()
    {
        if (!File.Exists(logFilePath))
        {
            return new List<HikingLog>();
        }

        string json = File.ReadAllText(logFilePath);
        return JsonSerializer.Deserialize<List<HikingLog>>(json) ?? new List<HikingLog>();
    }

    private void SaveLogs(List<HikingLog> logs)
    {
        string json = JsonSerializer.Serialize(logs);
        File.WriteAllText(logFilePath, json);
    }
}

public class HikingLog
{
    public string Location { get; set; }
    public double Distance { get; set; }
    public DateTime Date { get; set; }
}