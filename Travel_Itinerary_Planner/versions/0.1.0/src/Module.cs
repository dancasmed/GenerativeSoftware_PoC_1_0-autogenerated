using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TravelItineraryPlanner : IGeneratedModule
{
    public string Name { get; set; } = "Travel Itinerary Planner";

    private string _itineraryFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Travel Itinerary Planner module...");
        _itineraryFilePath = Path.Combine(dataFolder, "travel_itineraries.json");

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Create new itinerary");
            Console.WriteLine("2. View existing itineraries");
            Console.WriteLine("3. Exit module");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            if (input == "1")
            {
                CreateNewItinerary();
            }
            else if (input == "2")
            {
                ViewExistingItineraries();
            }
            else if (input == "3")
            {
                Console.WriteLine("Exiting Travel Itinerary Planner module...");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }

    private void CreateNewItinerary()
    {
        Console.Write("Enter itinerary name: ");
        string name = Console.ReadLine();

        var itinerary = new TravelItinerary
        {
            Name = name,
            Destinations = new List<Destination>(),
            TotalBudget = 0
        };

        while (true)
        {
            Console.Write("Add destination (Y/N)? ");
            string response = Console.ReadLine().Trim().ToUpper();

            if (response != "Y")
                break;

            Console.Write("Destination name: ");
            string destName = Console.ReadLine();

            Console.Write("Days to stay: ");
            int days = int.Parse(Console.ReadLine());

            Console.Write("Estimated cost per day: ");
            decimal costPerDay = decimal.Parse(Console.ReadLine());

            var destination = new Destination
            {
                Name = destName,
                Days = days,
                CostPerDay = costPerDay
            };

            itinerary.Destinations.Add(destination);
            itinerary.TotalBudget += days * costPerDay;
        }

        SaveItinerary(itinerary);
        Console.WriteLine("Itinerary saved successfully!");
    }

    private void ViewExistingItineraries()
    {
        if (!File.Exists(_itineraryFilePath))
        {
            Console.WriteLine("No itineraries found.");
            return;
        }

        string json = File.ReadAllText(_itineraryFilePath);
        var itineraries = JsonSerializer.Deserialize<List<TravelItinerary>>(json);

        if (itineraries == null || itineraries.Count == 0)
        {
            Console.WriteLine("No itineraries found.");
            return;
        }

        Console.WriteLine("\nSaved Itineraries:");
        for (int i = 0; i < itineraries.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {itineraries[i].Name} (Budget: {itineraries[i].TotalBudget:C})");
        }

        Console.Write("\nSelect itinerary to view details (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= itineraries.Count)
        {
            DisplayItineraryDetails(itineraries[selection - 1]);
        }
    }

    private void DisplayItineraryDetails(TravelItinerary itinerary)
    {
        Console.WriteLine("\nItinerary Details:");
        Console.WriteLine($"Name: {itinerary.Name}");
        Console.WriteLine($"Total Budget: {itinerary.TotalBudget:C}");
        Console.WriteLine("Destinations:");

        foreach (var destination in itinerary.Destinations)
        {
            Console.WriteLine($"- {destination.Name}: {destination.Days} days at {destination.CostPerDay:C}/day (Total: {destination.Days * destination.CostPerDay:C})");
        }
    }

    private void SaveItinerary(TravelItinerary newItinerary)
    {
        List<TravelItinerary> itineraries;

        if (File.Exists(_itineraryFilePath))
        {
            string json = File.ReadAllText(_itineraryFilePath);
            itineraries = JsonSerializer.Deserialize<List<TravelItinerary>>(json);
        }
        else
        {
            itineraries = new List<TravelItinerary>();
        }

        itineraries.Add(newItinerary);

        var options = new JsonSerializerOptions { WriteIndented = true };
        string newJson = JsonSerializer.Serialize(itineraries, options);
        File.WriteAllText(_itineraryFilePath, newJson);
    }
}

public class TravelItinerary
{
    public string Name { get; set; }
    public List<Destination> Destinations { get; set; }
    public decimal TotalBudget { get; set; }
}

public class Destination
{
    public string Name { get; set; }
    public int Days { get; set; }
    public decimal CostPerDay { get; set; }
}