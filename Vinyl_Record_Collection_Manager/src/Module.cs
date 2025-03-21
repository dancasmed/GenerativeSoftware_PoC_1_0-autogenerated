using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VinylRecordCollectionManager : IGeneratedModule
{
    public string Name { get; set; } = "Vinyl Record Collection Manager";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Vinyl Record Collection Manager...");

        string recordsFilePath = Path.Combine(dataFolder, "vinyl_records.json");
        List<VinylRecord> records = LoadRecords(recordsFilePath);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add a new vinyl record");
            Console.WriteLine("2. View all vinyl records");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddRecord(records);
                    break;
                case "2":
                    ViewRecords(records);
                    break;
                case "3":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveRecords(records, recordsFilePath);
        Console.WriteLine("Exiting Vinyl Record Collection Manager...");
        return true;
    }

    private List<VinylRecord> LoadRecords(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<VinylRecord>>(json);
        }
        return new List<VinylRecord>();
    }

    private void SaveRecords(List<VinylRecord> records, string filePath)
    {
        string json = JsonSerializer.Serialize(records);
        File.WriteAllText(filePath, json);
    }

    private void AddRecord(List<VinylRecord> records)
    {
        Console.Write("Enter the artist name: ");
        string artist = Console.ReadLine();
        Console.Write("Enter the album title: ");
        string title = Console.ReadLine();
        Console.Write("Enter the release year: ");
        int year = int.Parse(Console.ReadLine());

        records.Add(new VinylRecord { Artist = artist, Title = title, Year = year });
        Console.WriteLine("Record added successfully.");
    }

    private void ViewRecords(List<VinylRecord> records)
    {
        if (records.Count == 0)
        {
            Console.WriteLine("No records found.");
            return;
        }

        foreach (var record in records)
        {
            Console.WriteLine($"Artist: {record.Artist}, Title: {record.Title}, Year: {record.Year}");
        }
    }
}

public class VinylRecord
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
}