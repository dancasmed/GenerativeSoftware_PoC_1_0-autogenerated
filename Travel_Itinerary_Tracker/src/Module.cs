using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TravelItineraryTracker : IGeneratedModule
{
    public string Name { get; set; } = "Travel Itinerary Tracker";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Travel Itinerary Tracker module is running.");
        
        string dataFilePath = Path.Combine(dataFolder, "travel_itineraries.json");
        
        List<TravelItinerary> itineraries = LoadItineraries(dataFilePath);
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add new itinerary");
            Console.WriteLine("2. View all itineraries");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddItinerary(itineraries);
                    break;
                case "2":
                    DisplayItineraries(itineraries);
                    break;
                case "3":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveItineraries(itineraries, dataFilePath);
        
        return true;
    }
    
    private List<TravelItinerary> LoadItineraries(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<TravelItinerary>>(json) ?? new List<TravelItinerary>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading itineraries: " + ex.Message);
        }
        
        return new List<TravelItinerary>();
    }
    
    private void SaveItineraries(List<TravelItinerary> itineraries, string filePath)
    {
        try
        {
            string json = JsonSerializer.Serialize(itineraries);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving itineraries: " + ex.Message);
        }
    }
    
    private void AddItinerary(List<TravelItinerary> itineraries)
    {
        Console.Write("Enter traveler name: ");
        string travelerName = Console.ReadLine();
        
        Console.Write("Enter departure date (YYYY-MM-DD): ");
        DateTime departureDate = DateTime.Parse(Console.ReadLine());
        
        Console.Write("Enter return date (YYYY-MM-DD): ");
        DateTime returnDate = DateTime.Parse(Console.ReadLine());
        
        Console.Write("Enter flight number: ");
        string flightNumber = Console.ReadLine();
        
        Console.Write("Enter airline: ");
        string airline = Console.ReadLine();
        
        Console.Write("Enter hotel name: ");
        string hotelName = Console.ReadLine();
        
        Console.Write("Enter hotel confirmation number: ");
        string hotelConfirmation = Console.ReadLine();
        
        var itinerary = new TravelItinerary
        {
            TravelerName = travelerName,
            DepartureDate = departureDate,
            ReturnDate = returnDate,
            FlightNumber = flightNumber,
            Airline = airline,
            HotelName = hotelName,
            HotelConfirmation = hotelConfirmation
        };
        
        itineraries.Add(itinerary);
        Console.WriteLine("Itinerary added successfully.");
    }
    
    private void DisplayItineraries(List<TravelItinerary> itineraries)
    {
        if (itineraries.Count == 0)
        {
            Console.WriteLine("No itineraries found.");
            return;
        }
        
        Console.WriteLine("\nTravel Itineraries:");
        foreach (var itinerary in itineraries)
        {
            Console.WriteLine("Traveler: " + itinerary.TravelerName);
            Console.WriteLine("Dates: " + itinerary.DepartureDate.ToString("yyyy-MM-dd") + " to " + itinerary.ReturnDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("Flight: " + itinerary.Airline + " " + itinerary.FlightNumber);
            Console.WriteLine("Hotel: " + itinerary.HotelName + " (Confirmation: " + itinerary.HotelConfirmation + ")");
            Console.WriteLine("-----");
        }
    }
}

public class TravelItinerary
{
    public string TravelerName { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public string FlightNumber { get; set; }
    public string Airline { get; set; }
    public string HotelName { get; set; }
    public string HotelConfirmation { get; set; }
}