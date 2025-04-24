using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class VinylManager : IGeneratedModule
{
    public string Name { get; set; } = "Vinyl Collection Manager";
    
    private string recordsPath;
    private string historyPath;
    private string wishlistPath;
    private List<Record> records;
    private List<ListeningHistory> listeningHistory;
    private List<Wishlist> wishlist;

    public VinylManager()
    {
        records = new List<Record>();
        listeningHistory = new List<ListeningHistory>();
        wishlist = new List<Wishlist>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Vinyl Collection Manager...");
        
        recordsPath = Path.Combine(dataFolder, "records.json");
        historyPath = Path.Combine(dataFolder, "history.json");
        wishlistPath = Path.Combine(dataFolder, "wishlist.json");

        LoadData();

        while (ShowMainMenu())
        {
            // Main loop continues until user exits
        }

        SaveData();
        return true;
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(recordsPath))
                records = JsonConvert.DeserializeObject<List<Record>>(File.ReadAllText(recordsPath));
            
            if (File.Exists(historyPath))
                listeningHistory = JsonConvert.DeserializeObject<List<ListeningHistory>>(File.ReadAllText(historyPath));
            
            if (File.Exists(wishlistPath))
                wishlist = JsonConvert.DeserializeObject<List<Wishlist>>(File.ReadAllText(wishlistPath));
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Error loading data: {0}", ex.Message));
        }
    }

    private void SaveData()
    {
        try
        {
            File.WriteAllText(recordsPath, JsonConvert.SerializeObject(records, Formatting.Indented));
            File.WriteAllText(historyPath, JsonConvert.SerializeObject(listeningHistory, Formatting.Indented));
            File.WriteAllText(wishlistPath, JsonConvert.SerializeObject(wishlist, Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Error saving data: {0}", ex.Message));
        }
    }

    private bool ShowMainMenu()
    {
        Console.WriteLine("\nMain Menu:");
        Console.WriteLine("1. Add New Record");
        Console.WriteLine("2. Edit Record");
        Console.WriteLine("3. Delete Record");
        Console.WriteLine("4. Search/Filter Records");
        Console.WriteLine("5. View Statistics");
        Console.WriteLine("6. Manage Wishlist");
        Console.WriteLine("7. Exit");
        Console.Write("Select an option: ");

        switch (Console.ReadLine())
        {
            case "1": AddRecord(); return true;
            case "2": EditRecord(); return true;
            case "3": DeleteRecord(); return true;
            case "4": SearchRecords(); return true;
            case "5": ShowStatistics(); return true;
            case "6": ManageWishlist(); return true;
            case "7": return false;
            default: Console.WriteLine("Invalid option"); return true;
        }
    }

    private void DeleteRecord()
    {
        Console.Write("Enter record ID to delete: ");
        var id = Console.ReadLine();
        var record = records.FirstOrDefault(r => r.Id == id);
        
        if (record != null)
        {
            records.Remove(record);
            Console.WriteLine("Record deleted successfully!");
        }
        else
        {
            Console.WriteLine("Record not found!");
        }
    }

    private void ManageWishlist()
    {
        Console.WriteLine("Wishlist management feature coming soon!");
    }

    private void AddRecord()
    {
        var record = new Record();
        Console.Write("Artist: ");
        record.Artist = Console.ReadLine();
        Console.Write("Album Title: ");
        record.AlbumTitle = Console.ReadLine();
        Console.Write("Release Year: ");
        record.ReleaseYear = int.Parse(Console.ReadLine());
        Console.Write("Genre: ");
        record.Genre = Console.ReadLine();
        Console.Write("Condition: ");
        record.Condition = Console.ReadLine();
        record.Id = Guid.NewGuid().ToString();
        records.Add(record);
        Console.WriteLine("Record added successfully!");
    }

    private void EditRecord()
    {
        Console.Write("Enter record ID to edit: ");
        var id = Console.ReadLine();
        var record = records.FirstOrDefault(r => r.Id == id);
        
        if (record != null)
        {
            Console.WriteLine(String.Format("Editing: {0} - {1}", record.Artist, record.AlbumTitle));
            Console.Write("New Artist (leave blank to keep current): ");
            var artist = Console.ReadLine();
            if (!string.IsNullOrEmpty(artist)) record.Artist = artist;
            
            Console.Write("New Album Title: ");
            var title = Console.ReadLine();
            if (!string.IsNullOrEmpty(title)) record.AlbumTitle = title;
            
            Console.Write("New Release Year: ");
            var yearInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(yearInput)) record.ReleaseYear = int.Parse(yearInput);
            
            records[records.FindIndex(r => r.Id == id)] = record;
            Console.WriteLine("Record updated!");
        }
        else
        {
            Console.WriteLine("Record not found!");
        }
    }

    private void SearchRecords()
    {
        Console.Write("Search term (artist/album/genre): ");
        var term = Console.ReadLine().ToLower();
        
        var results = records.Where(r =>
            r.Artist.ToLower().Contains(term) ||
            r.AlbumTitle.ToLower().Contains(term) ||
            r.Genre.ToLower().Contains(term))
            .ToList();
        
        Console.WriteLine(String.Format("Found {0} results:", results.Count));
        foreach (var r in results)
        {
            Console.WriteLine(String.Format("{0} - {1} ({2})", r.Artist, r.AlbumTitle, r.ReleaseYear));
        }
    }

    private void ShowStatistics()
    {
        var stats = new Statistics
        {
            TotalRecords = records.Count,
            RecordsByGenre = records
                .GroupBy(r => r.Genre)
                .ToDictionary(g => g.Key, g => g.Count())
        };
        
        Console.WriteLine(String.Format("Total Records: {0}", stats.TotalRecords));
        Console.WriteLine("Records by Genre:");
        foreach (var kvp in stats.RecordsByGenre)
        {
            Console.WriteLine(String.Format("- {0}: {1}", kvp.Key, kvp.Value));
        }
    }
}

public class Record
{
    public string Id { get; set; }
    public string Artist { get; set; }
    public string AlbumTitle { get; set; }
    public int ReleaseYear { get; set; }
    public string Genre { get; set; }
    public string Condition { get; set; }
    public bool IsFavorite { get; set; }
    public string Notes { get; set; }
}

public class ListeningHistory
{
    public string RecordId { get; set; }
    public DateTime ListenDate { get; set; }
    public string Notes { get; set; }
}

public class Wishlist
{
    public string Artist { get; set; }
    public string AlbumTitle { get; set; }
    public int ReleaseYear { get; set; }
    public string Genre { get; set; }
    public string Priority { get; set; }
}

public class Statistics
{
    public int TotalRecords { get; set; }
    public Dictionary<string, int> RecordsByGenre { get; set; }
    public List<string> MostListened { get; set; }
}