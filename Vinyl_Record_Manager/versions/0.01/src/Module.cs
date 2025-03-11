namespace GenerativeSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VinylRecordManager : IGeneratedModule
{
    public string Name { get; set; } = "Vinyl Record Manager";

    public bool Main(string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, "vinylRecords.json");
        List<VinylRecord> records = LoadRecords(filePath);

        while (true)
        {
            Console.WriteLine("\nVinyl Record Manager");
            Console.WriteLine("1. Add a Vinyl Record");
            Console.WriteLine("2. View All Vinyl Records");
            Console.WriteLine("3. Search for a Vinyl Record");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddVinylRecord(records);
                    break;
                case "2":
                    ViewAllRecords(records);
                    break;
                case "3":
                    SearchRecord(records);
                    break;
                case "4":
                    SaveRecords(filePath, records);
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
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

    private void SaveRecords(string filePath, List<VinylRecord> records)
    {
        string json = JsonSerializer.Serialize(records);
        File.WriteAllText(filePath, json);
    }

    private void AddVinylRecord(List<VinylRecord> records)
    {
        Console.Write("Enter Artist: ");
        string artist = Console.ReadLine();
        Console.Write("Enter Album Title: ");
        string title = Console.ReadLine();
        Console.Write("Enter Year: ");
        int year = int.Parse(Console.ReadLine());
        Console.Write("Enter Genre: ");
        string genre = Console.ReadLine();

        records.Add(new VinylRecord { Artist = artist, Title = title, Year = year, Genre = genre });
        Console.WriteLine("Vinyl record added successfully!");
    }

    private void ViewAllRecords(List<VinylRecord> records)
    {
        if (records.Count == 0)
        {
            Console.WriteLine("No records found.");
            return;
        }

        foreach (var record in records)
        {
            Console.WriteLine($"Artist: {record.Artist}, Title: {record.Title}, Year: {record.Year}, Genre: {record.Genre}");
        }
    }

    private void SearchRecord(List<VinylRecord> records)
    {
        Console.Write("Enter Artist or Album Title to search: ");
        string searchTerm = Console.ReadLine();

        var results = records.FindAll(r => r.Artist.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || r.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        if (results.Count == 0)
        {
            Console.WriteLine("No matching records found.");
        }
        else
        {
            foreach (var record in results)
            {
                Console.WriteLine($"Artist: {record.Artist}, Title: {record.Title}, Year: {record.Year}, Genre: {record.Genre}");
            }
        }
    }
}

public class VinylRecord
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Genre { get; set; }
}