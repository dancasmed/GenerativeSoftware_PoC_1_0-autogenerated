using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VinylRecordManager : IGeneratedModule
{
    public string Name { get; set; } = "Vinyl Record Manager";

    private string _recordsFilePath;
    private List<VinylRecord> _records;

    public VinylRecordManager()
    {
        _records = new List<VinylRecord>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Vinyl Record Manager module is running...");
        Console.WriteLine("Managing your vinyl collection in: " + dataFolder);

        _recordsFilePath = Path.Combine(dataFolder, "vinyl_records.json");

        try
        {
            LoadRecords();
            ShowMainMenu();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void LoadRecords()
    {
        if (File.Exists(_recordsFilePath))
        {
            string json = File.ReadAllText(_recordsFilePath);
            _records = JsonSerializer.Deserialize<List<VinylRecord>>(json) ?? new List<VinylRecord>();
            Console.WriteLine("Loaded " + _records.Count + " records from collection.");
        }
        else
        {
            Console.WriteLine("No existing collection found. Starting new collection.");
        }
    }

    private void SaveRecords()
    {
        string json = JsonSerializer.Serialize(_records, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_recordsFilePath, json);
        Console.WriteLine("Collection saved successfully.");
    }

    private void ShowMainMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nVinyl Record Collection Manager");
            Console.WriteLine("1. View Collection");
            Console.WriteLine("2. Add Record");
            Console.WriteLine("3. Remove Record");
            Console.WriteLine("4. Search Records");
            Console.WriteLine("5. Save and Exit");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewCollection();
                    break;
                case "2":
                    AddRecord();
                    break;
                case "3":
                    RemoveRecord();
                    break;
                case "4":
                    SearchRecords();
                    break;
                case "5":
                    SaveRecords();
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void ViewCollection()
    {
        if (_records.Count == 0)
        {
            Console.WriteLine("Your collection is empty.");
            return;
        }

        Console.WriteLine("\nYour Vinyl Collection:");
        Console.WriteLine("----------------------");

        for (int i = 0; i < _records.Count; i++)
        {
            Console.WriteLine("Record #" + (i + 1));
            Console.WriteLine("Artist: " + _records[i].Artist);
            Console.WriteLine("Album: " + _records[i].Album);
            Console.WriteLine("Year: " + _records[i].Year);
            Console.WriteLine("Genre: " + _records[i].Genre);
            Console.WriteLine("Condition: " + _records[i].Condition);
            Console.WriteLine("----------------------");
        }
    }

    private void AddRecord()
    {
        Console.WriteLine("\nAdd New Vinyl Record");
        Console.WriteLine("----------------------");

        var record = new VinylRecord();

        Console.Write("Artist: ");
        record.Artist = Console.ReadLine();

        Console.Write("Album: ");
        record.Album = Console.ReadLine();

        Console.Write("Year: ");
        if (int.TryParse(Console.ReadLine(), out int year))
        {
            record.Year = year;
        }
        else
        {
            Console.WriteLine("Invalid year. Setting to 0.");
            record.Year = 0;
        }

        Console.Write("Genre: ");
        record.Genre = Console.ReadLine();

        Console.Write("Condition (Mint/Near Mint/Very Good/Good/Fair/Poor): ");
        record.Condition = Console.ReadLine();

        _records.Add(record);
        Console.WriteLine("Record added successfully!");
    }

    private void RemoveRecord()
    {
        if (_records.Count == 0)
        {
            Console.WriteLine("Your collection is empty. Nothing to remove.");
            return;
        }

        ViewCollection();
        Console.Write("\nEnter the number of the record to remove: ");

        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _records.Count)
        {
            var record = _records[index - 1];
            _records.RemoveAt(index - 1);
            Console.WriteLine("Removed: " + record.Artist + " - " + record.Album);
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }

    private void SearchRecords()
    {
        if (_records.Count == 0)
        {
            Console.WriteLine("Your collection is empty. Nothing to search.");
            return;
        }

        Console.Write("\nEnter search term (artist, album, or genre): ");
        string term = Console.ReadLine().ToLower();

        var results = _records.FindAll(r =>
            r.Artist.ToLower().Contains(term) ||
            r.Album.ToLower().Contains(term) ||
            r.Genre.ToLower().Contains(term));

        if (results.Count == 0)
        {
            Console.WriteLine("No matching records found.");
            return;
        }

        Console.WriteLine("\nSearch Results (" + results.Count + " matches):");
        Console.WriteLine("----------------------");

        foreach (var record in results)
        {
            Console.WriteLine("Artist: " + record.Artist);
            Console.WriteLine("Album: " + record.Album);
            Console.WriteLine("Year: " + record.Year);
            Console.WriteLine("Genre: " + record.Genre);
            Console.WriteLine("----------------------");
        }
    }
}

public class VinylRecord
{
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
}