using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FishingTripTracker : IGeneratedModule
{
    public string Name { get; set; } = "Fishing Trip Tracker";

    private string _dataFilePath;
    private List<FishingTrip> _fishingTrips;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Fishing Trip Tracker module is running.");
        _dataFilePath = Path.Combine(dataFolder, "fishing_trips.json");
        _fishingTrips = LoadFishingTrips();

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add new fishing trip");
            Console.WriteLine("2. View all fishing trips");
            Console.WriteLine("3. Exit module");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            if (input == "1")
            {
                AddNewFishingTrip();
            }
            else if (input == "2")
            {
                ViewAllFishingTrips();
            }
            else if (input == "3")
            {
                Console.WriteLine("Exiting Fishing Trip Tracker module.");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }

    private List<FishingTrip> LoadFishingTrips()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<FishingTrip>>(json) ?? new List<FishingTrip>();
        }
        return new List<FishingTrip>();
    }

    private void SaveFishingTrips()
    {
        string json = JsonSerializer.Serialize(_fishingTrips);
        File.WriteAllText(_dataFilePath, json);
    }

    private void AddNewFishingTrip()
    {
        Console.WriteLine("\nEnter fishing trip details:");

        Console.Write("Location: ");
        string location = Console.ReadLine();

        Console.Write("Date (yyyy-MM-dd): ");
        DateTime date;
        while (!DateTime.TryParse(Console.ReadLine(), out date))
        {
            Console.Write("Invalid date format. Please enter date (yyyy-MM-dd): ");
        }

        Console.Write("Weather conditions: ");
        string weather = Console.ReadLine();

        Console.Write("Number of fish caught: ");
        int fishCount;
        while (!int.TryParse(Console.ReadLine(), out fishCount))
        {
            Console.Write("Invalid number. Please enter number of fish caught: ");
        }

        Console.Write("Total weight (kg): ");
        double totalWeight;
        while (!double.TryParse(Console.ReadLine(), out totalWeight))
        {
            Console.Write("Invalid weight. Please enter total weight (kg): ");
        }

        Console.Write("Notes: ");
        string notes = Console.ReadLine();

        var trip = new FishingTrip
        {
            Location = location,
            Date = date,
            Weather = weather,
            FishCount = fishCount,
            TotalWeight = totalWeight,
            Notes = notes
        };

        _fishingTrips.Add(trip);
        SaveFishingTrips();

        Console.WriteLine("Fishing trip added successfully!");
    }

    private void ViewAllFishingTrips()
    {
        if (_fishingTrips.Count == 0)
        {
            Console.WriteLine("No fishing trips recorded yet.");
            return;
        }

        Console.WriteLine("\nAll Fishing Trips:");
        foreach (var trip in _fishingTrips)
        {
            Console.WriteLine("----------------------------");
            Console.WriteLine("Location: " + trip.Location);
            Console.WriteLine("Date: " + trip.Date.ToString("yyyy-MM-dd"));
            Console.WriteLine("Weather: " + trip.Weather);
            Console.WriteLine("Fish Caught: " + trip.FishCount);
            Console.WriteLine("Total Weight: " + trip.TotalWeight + " kg");
            Console.WriteLine("Notes: " + trip.Notes);
        }
    }
}

public class FishingTrip
{
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public string Weather { get; set; }
    public int FishCount { get; set; }
    public double TotalWeight { get; set; }
    public string Notes { get; set; }
}