using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public class VinylRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("artist")]
    public string Artist { get; set; }
    [JsonPropertyName("genre")]
    public string Genre { get; set; }
    [JsonPropertyName("release_year")]
    public int ReleaseYear { get; set; }
    [JsonPropertyName("condition")]
    public string Condition { get; set; }
    [JsonPropertyName("notes")]
    public string Notes { get; set; }
    [JsonPropertyName("cover_image_url")]
    public string CoverImageUrl { get; set; }
}

public class ListeningSession
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonPropertyName("record_id")]
    public string RecordId { get; set; }
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    [JsonPropertyName("notes")]
    public string Notes { get; set; }
}

public class DataService
{
    private readonly string _dataFolder;

    public DataService(string dataFolder)
    {
        _dataFolder = dataFolder;
        Directory.CreateDirectory(dataFolder);
    }

    public List<VinylRecord> LoadRecords() =>
        LoadData<VinylRecord>("records.json");

    public void SaveRecords(List<VinylRecord> records) =>
        SaveData("records.json", records);

    public List<ListeningSession> LoadSessions() =>
        LoadData<ListeningSession>("sessions.json");

    public void SaveSessions(List<ListeningSession> sessions) =>
        SaveData("sessions.json", sessions);

    private List<T> LoadData<T>(string fileName)
    {
        var path = Path.Combine(_dataFolder, fileName);
        return File.Exists(path)
            ? JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path))
            : new List<T>();
    }

    private void SaveData<T>(string fileName, List<T> data)
    {
        var path = Path.Combine(_dataFolder, fileName);
        File.WriteAllText(path, JsonSerializer.Serialize(data));
    }
}

public class VinylCollectionModule : IGeneratedModule
{
    public string Name { get; set; } = "Vinyl Collection Manager";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Vinyl Collection Manager...");
        var dataService = new DataService(dataFolder);
        var records = dataService.LoadRecords();
        var sessions = dataService.LoadSessions();

        while (true)
        {
            Console.WriteLine("\n1. Add Record\n2. Update Record\n3. Delete Record\n4. List Records\n5. Search Records\n6. Add Listening Session\n7. View Statistics\n8. Get Recommendations\n9. Exit");
            Console.Write("Select option: ");
            switch (Console.ReadLine())
            {
                case "1":
                    AddRecord(records, dataService);
                    break;
                case "2":
                    UpdateRecord(records, dataService);
                    break;
                case "3":
                    DeleteRecord(records, dataService);
                    break;
                case "4":
                    ListRecords(records);
                    break;
                case "5":
                    SearchRecords(records);
                    break;
                case "6":
                    AddSession(sessions, records, dataService);
                    break;
                case "7":
                    ShowStatistics(records, sessions);
                    break;
                case "8":
                    GenerateRecommendations(records);
                    break;
                case "9":
                    return true;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private void AddRecord(List<VinylRecord> records, DataService dataService)
    {
        var record = new VinylRecord();
        Console.Write("Title: ");
        record.Title = Console.ReadLine();
        Console.Write("Artist: ");
        record.Artist = Console.ReadLine();
        Console.Write("Genre: ");
        record.Genre = Console.ReadLine();
        Console.Write("Release Year: ");
        record.ReleaseYear = int.Parse(Console.ReadLine());
        Console.Write("Condition: ");
        record.Condition = Console.ReadLine();
        Console.Write("Notes: ");
        record.Notes = Console.ReadLine();
        Console.Write("Cover Image URL: ");
        record.CoverImageUrl = Console.ReadLine();
        records.Add(record);
        dataService.SaveRecords(records);
        Console.WriteLine("Record added");
    }

    private void UpdateRecord(List<VinylRecord> records, DataService dataService)
    {
        Console.Write("Enter record ID: ");
        var record = records.FirstOrDefault(r => r.Id == Console.ReadLine());
        if (record == null) Console.WriteLine("Record not found");
        else
        {
            Console.Write("New Title (Enter to skip): ");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input)) record.Title = input;

            Console.Write("New Artist (Enter to skip): ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input)) record.Artist = input;

            Console.Write("New Genre (Enter to skip): ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input)) record.Genre = input;

            Console.Write("New Release Year (Enter to skip): ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int year))
                record.ReleaseYear = year;

            Console.Write("New Condition (Enter to skip): ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input)) record.Condition = input;

            Console.Write("New Notes (Enter to skip): ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input)) record.Notes = input;

            Console.Write("New Cover Image URL (Enter to skip): ");
            input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input)) record.CoverImageUrl = input;

            dataService.SaveRecords(records);
            Console.WriteLine("Record updated");
        }
    }

    private void DeleteRecord(List<VinylRecord> records, DataService dataService)
    {
        Console.Write("Enter record ID: ");
        var record = records.FirstOrDefault(r => r.Id == Console.ReadLine());
        if (record != null)
        {
            records.Remove(record);
            dataService.SaveRecords(records);
            Console.WriteLine("Record deleted");
        }
        else Console.WriteLine("Record not found");
    }

    private void ListRecords(List<VinylRecord> records)
    {
        Console.WriteLine($"Total records: {records.Count}");
        foreach (var record in records)
        {
            Console.WriteLine($"ID: {record.Id}");
            Console.WriteLine($"Title: {record.Title}");
            Console.WriteLine($"Artist: {record.Artist}");
            Console.WriteLine($"Genre: {record.Genre}");
            Console.WriteLine($"Release Year: {record.ReleaseYear}");
            Console.WriteLine($"Condition: {record.Condition}");
            Console.WriteLine($"Notes: {record.Notes}");
            Console.WriteLine($"Cover Image URL: {record.CoverImageUrl}");
            Console.WriteLine("-----------------------");
        }
    }

    private void SearchRecords(List<VinylRecord> records)
    {
        Console.Write("Search term: ");
        var term = Console.ReadLine().ToLower();
        var results = records.Where(r =>
            r.Title.ToLower().Contains(term) ||
            r.Artist.ToLower().Contains(term) ||
            r.Genre.ToLower().Contains(term)
        ).ToList();
        Console.WriteLine("Found " + results.Count + " records:");
        results.ForEach(r => Console.WriteLine(r.Title + " - " + r.Artist));
    }

    private void AddSession(List<ListeningSession> sessions, List<VinylRecord> records, DataService dataService)
    {
        Console.Write("Record ID: ");
        var record = records.FirstOrDefault(r => r.Id == Console.ReadLine());
        if (record == null) Console.WriteLine("Record not found");
        else
        {
            var session = new ListeningSession { RecordId = record.Id };
            Console.Write("Duration (minutes): ");
            session.Duration = int.Parse(Console.ReadLine());
            sessions.Add(session);
            dataService.SaveSessions(sessions);
            Console.WriteLine("Session recorded");
        }
    }

    private void ShowStatistics(List<VinylRecord> records, List<ListeningSession> sessions)
    {
        Console.WriteLine("Total records: " + records.Count);
        Console.WriteLine("\nGenres:");
        records.GroupBy(r => r.Genre)
            .ToList()
            .ForEach(g => Console.WriteLine(g.Key + ": " + g.Count()));

        Console.WriteLine("\nMost played records:");
        sessions.GroupBy(s => s.RecordId)
            .Select(g => new { Id = g.Key, Plays = g.Count() })
            .OrderByDescending(g => g.Plays)
            .Take(3)
            .ToList()
            .ForEach(g =>
            {
                var record = records.FirstOrDefault(r => r.Id == g.Id);
                Console.WriteLine(record?.Title + " - " + g.Plays + " plays");
            });
    }

    private void GenerateRecommendations(List<VinylRecord> records)
    {
        var topGenre = records
            .GroupBy(r => r.Genre)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        Console.WriteLine("Recommended " + topGenre + " records:");
        Console.WriteLine("1. Essential compilation albums in " + topGenre);
        Console.WriteLine("2. Classic " + topGenre + " releases from the 70s");
    }
}
