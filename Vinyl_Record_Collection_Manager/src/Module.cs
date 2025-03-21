using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VinylRecordCollectionManager : IGeneratedModule
{
    public string Name { get; set; } = "Vinyl Record Collection Manager";

    private string _dataFilePath;

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "vinyl_records.json");

        Console.WriteLine("Vinyl Record Collection Manager is running...");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        List<VinylRecord> records = LoadRecords();

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add a new vinyl record");
            Console.WriteLine("2. View all vinyl records");
            Console.WriteLine("3. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddRecord(records);
                    break;
                case "2":
                    ViewRecords(records);
                    break;
                case "3":
                    SaveRecords(records);
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private List<VinylRecord> LoadRecords()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<VinylRecord>>(json);
        }
        return new List<VinylRecord>();
    }

    private void SaveRecords(List<VinylRecord> records)
    {
        string json = JsonSerializer.Serialize(records);
        File.WriteAllText(_dataFilePath, json);
        Console.WriteLine("Records saved successfully.");
    }

    private void AddRecord(List<VinylRecord> records)
    {
        Console.WriteLine("Enter the title of the vinyl record:");
        string title = Console.ReadLine();

        Console.WriteLine("Enter the artist name:");
        string artist = Console.ReadLine();

        Console.WriteLine("Enter the year of release:");
        int year = int.Parse(Console.ReadLine());

        records.Add(new VinylRecord { Title = title, Artist = artist, Year = year });
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
            Console.WriteLine($"Title: {record.Title}, Artist: {record.Artist}, Year: {record.Year}");
        }
    }
}

public class VinylRecord
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public int Year { get; set; }
}