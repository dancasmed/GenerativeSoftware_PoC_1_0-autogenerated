using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WaterConservationTracker : IGeneratedModule
{
    public string Name { get; set; } = "Water Conservation Tracker";

    private string _dataFilePath;
    private List<WaterConservationRecord> _records;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Water Conservation Tracker...");
        
        _dataFilePath = Path.Combine(dataFolder, "water_conservation_records.json");
        _records = LoadRecords();

        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddNewRecord();
                    break;
                case "2":
                    ViewAllRecords();
                    break;
                case "3":
                    AnalyzeConservation();
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveRecords();
        Console.WriteLine("Water Conservation Tracker has finished execution.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nWater Conservation Tracker");
        Console.WriteLine("1. Add new conservation record");
        Console.WriteLine("2. View all records");
        Console.WriteLine("3. Analyze conservation efforts");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddNewRecord()
    {
        Console.WriteLine("\nAdd New Conservation Record");
        
        Console.Write("Enter date (YYYY-MM-DD): ");
        string dateInput = Console.ReadLine();
        
        Console.Write("Enter water saved (liters): ");
        string litersInput = Console.ReadLine();
        
        Console.Write("Enter conservation method: ");
        string method = Console.ReadLine();
        
        if (DateTime.TryParse(dateInput, out DateTime date) && double.TryParse(litersInput, out double liters))
        {
            _records.Add(new WaterConservationRecord { Date = date, LitersSaved = liters, Method = method });
            Console.WriteLine("Record added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid input. Record not added.");
        }
    }

    private void ViewAllRecords()
    {
        Console.WriteLine("\nAll Conservation Records");
        Console.WriteLine("Date\t\tLiters Saved\tMethod");
        
        foreach (var record in _records)
        {
            Console.WriteLine(record.ToString());
        }
        
        Console.WriteLine("Total records: " + _records.Count);
    }

    private void AnalyzeConservation()
    {
        if (_records.Count == 0)
        {
            Console.WriteLine("No records available for analysis.");
            return;
        }

        double totalSaved = 0;
        var methodStats = new Dictionary<string, double>();
        
        foreach (var record in _records)
        {
            totalSaved += record.LitersSaved;
            
            if (methodStats.ContainsKey(record.Method))
            {
                methodStats[record.Method] += record.LitersSaved;
            }
            else
            {
                methodStats[record.Method] = record.LitersSaved;
            }
        }

        Console.WriteLine("\nConservation Analysis");
        Console.WriteLine("Total water saved: " + totalSaved + " liters");
        Console.WriteLine("Average per day: " + (totalSaved / _records.Count).ToString("F2") + " liters");
        
        Console.WriteLine("\nBy Method:");
        foreach (var kvp in methodStats)
        {
            Console.WriteLine(kvp.Key + ": " + kvp.Value + " liters (" + (kvp.Value / totalSaved * 100).ToString("F1") + "%)");
        }
    }

    private List<WaterConservationRecord> LoadRecords()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<WaterConservationRecord>>(json) ?? new List<WaterConservationRecord>();
        }
        return new List<WaterConservationRecord>();
    }

    private void SaveRecords()
    {
        string json = JsonSerializer.Serialize(_records);
        File.WriteAllText(_dataFilePath, json);
    }

    private class WaterConservationRecord
    {
        public DateTime Date { get; set; }
        public double LitersSaved { get; set; }
        public string Method { get; set; }

        public override string ToString()
        {
            return Date.ToString("yyyy-MM-dd") + "\t" + LitersSaved + "\t\t" + Method;
        }
    }
}