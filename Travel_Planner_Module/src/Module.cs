using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TravelPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Travel Planner Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Travel Planner Module is running...");
        Console.WriteLine("Planning travel itineraries with destinations and budgets.");

        string filePath = Path.Combine(dataFolder, "travel_itineraries.json");
        List<TravelItinerary> itineraries = new List<TravelItinerary>();

        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                itineraries = JsonSerializer.Deserialize<List<TravelItinerary>>(jsonData);
                Console.WriteLine("Loaded existing travel itineraries.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading travel itineraries: " + ex.Message);
                return false;
            }
        }

        bool continuePlanning = true;
        while (continuePlanning)
        {
            Console.WriteLine("\n1. Add new travel itinerary");
            Console.WriteLine("2. View all travel itineraries");
            Console.WriteLine("3. Save and exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddNewItinerary(itineraries);
                    break;
                case "2":
                    ViewAllItineraries(itineraries);
                    break;
                case "3":
                    continuePlanning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        try
        {
            string jsonData = JsonSerializer.Serialize(itineraries);
            File.WriteAllText(filePath, jsonData);
            Console.WriteLine("Travel itineraries saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving travel itineraries: " + ex.Message);
            return false;
        }
    }

    private void AddNewItinerary(List<TravelItinerary> itineraries)
    {
        Console.Write("Enter destination name: ");
        string destination = Console.ReadLine();

        Console.Write("Enter start date (YYYY-MM-DD): ");
        string startDateInput = Console.ReadLine();

        Console.Write("Enter end date (YYYY-MM-DD): ");
        string endDateInput = Console.ReadLine();

        Console.Write("Enter budget: ");
        string budgetInput = Console.ReadLine();

        if (DateTime.TryParse(startDateInput, out DateTime startDate) && 
            DateTime.TryParse(endDateInput, out DateTime endDate) && 
            decimal.TryParse(budgetInput, out decimal budget))
        {
            TravelItinerary newItinerary = new TravelItinerary
            {
                Destination = destination,
                StartDate = startDate,
                EndDate = endDate,
                Budget = budget
            };

            itineraries.Add(newItinerary);
            Console.WriteLine("New travel itinerary added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid input. Please try again.");
        }
    }

    private void ViewAllItineraries(List<TravelItinerary> itineraries)
    {
        if (itineraries.Count == 0)
        {
            Console.WriteLine("No travel itineraries found.");
            return;
        }

        Console.WriteLine("\n--- Travel Itineraries ---");
        foreach (var itinerary in itineraries)
        {
            Console.WriteLine("Destination: " + itinerary.Destination);
            Console.WriteLine("Dates: " + itinerary.StartDate.ToString("yyyy-MM-dd") + " to " + itinerary.EndDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("Budget: " + itinerary.Budget.ToString("C"));
            Console.WriteLine("----------------------------");
        }
    }
}

public class TravelItinerary
{
    public string Destination { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
}