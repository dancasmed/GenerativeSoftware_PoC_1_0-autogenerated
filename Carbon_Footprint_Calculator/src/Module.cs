using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CarbonFootprintCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Carbon Footprint Calculator";

    private Dictionary<string, double> activityFactors = new Dictionary<string, double>()
    {
        { "driving", 0.404 },
        { "flying", 0.227 },
        { "electricity", 0.92 },
        { "meat_diet", 2.5 },
        { "vegetarian_diet", 1.7 },
        { "public_transport", 0.101 }
    };

    private string dataFilePath;

    public bool Main(string dataFolder)
    {
        dataFilePath = Path.Combine(dataFolder, "carbon_footprint_data.json");
        Console.WriteLine("Carbon Footprint Calculator started.");
        Console.WriteLine("This tool helps you estimate your carbon footprint based on your daily activities.");

        try
        {
            LoadPreviousData();
            var footprint = CalculateFootprint();
            SaveResults(footprint);
            DisplayResults(footprint);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadPreviousData()
    {
        if (File.Exists(dataFilePath))
        {
            string jsonData = File.ReadAllText(dataFilePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, double>>(jsonData);
            if (data != null)
            {
                activityFactors = data;
            }
        }
    }

    private Dictionary<string, double> CalculateFootprint()
    {
        Dictionary<string, double> userActivities = new Dictionary<string, double>();
        Dictionary<string, double> footprintResults = new Dictionary<string, double>();

        Console.WriteLine("Enter your activities and their durations/amounts (e.g., driving 50 km). Type 'done' when finished.");

        while (true)
        {
            Console.Write("Activity (or 'done'): ");
            string activity = Console.ReadLine().ToLower().Trim();

            if (activity == "done")
                break;

            if (!activityFactors.ContainsKey(activity))
            {
                Console.WriteLine("Unknown activity. Available activities: " + string.Join(", ", activityFactors.Keys));
                continue;
            }

            Console.Write("Amount/Quantity: ");
            if (!double.TryParse(Console.ReadLine(), out double amount))
            {
                Console.WriteLine("Invalid number. Please try again.");
                continue;
            }

            userActivities[activity] = amount;
        }

        double totalFootprint = 0;
        foreach (var activity in userActivities)
        {
            double footprint = activity.Value * activityFactors[activity.Key];
            footprintResults[activity.Key] = footprint;
            totalFootprint += footprint;
        }

        footprintResults["total"] = totalFootprint;
        return footprintResults;
    }

    private void SaveResults(Dictionary<string, double> results)
    {
        string jsonData = JsonSerializer.Serialize(results);
        File.WriteAllText(dataFilePath, jsonData);
    }

    private void DisplayResults(Dictionary<string, double> results)
    {
        Console.WriteLine("\nCarbon Footprint Results:");
        foreach (var item in results)
        {
            if (item.Key != "total")
            {
                Console.WriteLine(item.Key + ": " + item.Value.ToString("0.00") + " kg CO2");
            }
        }
        Console.WriteLine("\nTotal Carbon Footprint: " + results["total"].ToString("0.00") + " kg CO2");
    }
}