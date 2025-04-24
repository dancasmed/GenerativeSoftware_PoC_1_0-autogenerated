using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class VinylCollectionManager : IGeneratedModule
{
    public string Name { get; set; } = "Vinyl Collection Manager";

    private List<VinylRecord> vinylRecords = new List<VinylRecord>();
    private List<ListeningSession> listeningSessions = new List<ListeningSession>();
    private List<Recommendation> recommendations = new List<Recommendation>();
    private string recordsPath;
    private string sessionsPath;
    private string recommendationsPath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Vinyl Collection Manager...");

        recordsPath = Path.Combine(dataFolder, "records.json");
        sessionsPath = Path.Combine(dataFolder, "sessions.json");
        recommendationsPath = Path.Combine(dataFolder, "recommendations.json");

        LoadData();

        while (ShowMainMenu()) { }

        SaveData();
        Console.WriteLine("Module execution completed successfully.");
        return true;
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(recordsPath))
                vinylRecords = JsonSerializer.Deserialize<List<VinylRecord>>(File.ReadAllText(recordsPath));

            if (File.Exists(sessionsPath))
                listeningSessions = JsonSerializer.Deserialize<List<ListeningSession>>(File.ReadAllText(sessionsPath));

            if (File.Exists(recommendationsPath))
                recommendations = JsonSerializer.Deserialize<List<Recommendation>>(File.ReadAllText(recommendationsPath));
        }
        catch { }
    }

    private void SaveData()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(recordsPath, JsonSerializer.Serialize(vinylRecords, options));
        File.WriteAllText(sessionsPath, JsonSerializer.Serialize(listeningSessions, options));
        File.WriteAllText(recommendationsPath, JsonSerializer.Serialize(recommendations, options));
    }

    private bool ShowMainMenu()
    {
        Console.WriteLine("\nMain Menu:");
        Console.WriteLine("1. Add New Record");
        Console.WriteLine("2. Edit Record");
        Console.WriteLine("3. Remove Record");
        Console.WriteLine("4. Search Records");
        Console.WriteLine("5. Record Listening Session");
        Console.WriteLine("6. Show Statistics");
        Console.WriteLine("7. Generate Recommendations");
        Console.WriteLine("8. Exit");

        switch (Console.ReadLine())
        {
            case "1": AddRecord(); return true;
            case "2": EditRecord(); return true;
            case "3": RemoveRecord(); return true;
            case "4": SearchRecords(); return true;
            case "5": AddListeningSession(); return true;
            case "6": ShowStatistics(); return true;
            case "7": GenerateRecommendations(); return true;
            case "8": return false;
            default: return true;
        }
    }

    private void AddRecord()
    {
        var record = new VinylRecord
        {
            Id = Guid.NewGuid().ToString(),
            Title = ReadInput("Title"),
            Artist = ReadInput("Artist"),
            Genre = ReadInput("Genre"),
            Year = int.Parse(ReadInput("Year")),
            Label = ReadInput("Label"),
            Condition = ReadInput("Condition"),
            Notes = ReadInput("Notes")
        };

        vinylRecords.Add(record);
        Console.WriteLine("Record added successfully.");
    }

    private void EditRecord()
    {
        var id = ReadInput("Enter record ID to edit");
        var record = vinylRecords.FirstOrDefault(r => r.Id == id);

        if (record == null)
        {
            Console.WriteLine("Record not found.");
            return;
        }

        record.Title = ReadInput("Title (current: " + record.Title + ")");
        record.Artist = ReadInput("Artist (current: " + record.Artist + ")");
        record.Genre = ReadInput("Genre (current: " + record.Genre + ")");
        record.Year = int.Parse(ReadInput("Year (current: " + record.Year + ")"));
        Console.WriteLine("Record updated successfully.");
    }

    private void RemoveRecord()
    {
        var id = ReadInput("Enter record ID to remove");
        var record = vinylRecords.FirstOrDefault(r => r.Id == id);

        if (record != null)
        {
            vinylRecords.Remove(record);
            Console.WriteLine("Record removed successfully.");
        }
        else
        {
            Console.WriteLine("Record not found.");
        }
    }

    private void SearchRecords()
    {
        var query = ReadInput("Search by Artist, Genre, or Year");
        var results = vinylRecords.Where(r =>
            r.Artist.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            r.Genre.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            r.Year.ToString().Contains(query))
            .ToList();

        Console.WriteLine("Found " + results.Count + " records:");
        results.ForEach(r => Console.WriteLine(r.Artist + " - " + r.Title));
    }

    private void AddListeningSession()
    {
        var session = new ListeningSession
        {
            Id = Guid.NewGuid().ToString(),
            RecordId = ReadInput("Enter record ID"),
            Date = DateTime.Now,
            Duration = int.Parse(ReadInput("Duration in minutes")),
            Notes = ReadInput("Session notes")
        };

        listeningSessions.Add(session);
        Console.WriteLine("Listening session recorded.");
    }

    private void ShowStatistics()
    {
        Console.WriteLine("Collection Statistics:");
        Console.WriteLine("Total Records: " + vinylRecords.Count);
        Console.WriteLine("Total Listening Sessions: " + listeningSessions.Count);

        var genreStats = vinylRecords
            .GroupBy(r => r.Genre)
            .Select(g => new { Genre = g.Key, Count = g.Count() });

        Console.WriteLine("\nGenre Distribution:");
        foreach (var stat in genreStats)
            Console.WriteLine(stat.Genre + ": " + stat.Count);
    }

    private void GenerateRecommendations()
    {
        var topGenre = vinylRecords
            .GroupBy(r => r.Genre)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        if (topGenre != null)
        {
            var recommendation = new Recommendation
            {
                Id = Guid.NewGuid().ToString(),
                RecordId = "",
                Reason = "Popular genre in your collection: " + topGenre,
                Date = DateTime.Now
            };

            recommendations.Add(recommendation);
            Console.WriteLine("New recommendation generated: " + recommendation.Reason);
        }
    }

    private string ReadInput(string prompt)
    {
        Console.Write(prompt + ": ");
        return Console.ReadLine();
    }
}

public class VinylRecord
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
    public string Label { get; set; }
    public string Condition { get; set; }
    public string Notes { get; set; }
}

public class ListeningSession
{
    public string Id { get; set; }
    public string RecordId { get; set; }
    public DateTime Date { get; set; }
    public double Duration { get; set; }
    public string Notes { get; set; }
}

public class Recommendation
{
    public string Id { get; set; }
    public string RecordId { get; set; }
    public string UserId { get; set; }
    public string Reason { get; set; }
    public DateTime Date { get; set; }
}
