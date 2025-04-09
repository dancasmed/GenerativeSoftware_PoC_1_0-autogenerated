using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class AverageSpeedCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Average Speed Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Average Speed Calculator Module is running...");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "journey_config.json");
            string resultPath = Path.Combine(dataFolder, "average_speed_result.json");

            if (!File.Exists(configPath))
            {
                Console.WriteLine("Configuration file not found. Creating default configuration.");
                CreateDefaultConfig(configPath);
                Console.WriteLine("Please edit the configuration file and run the module again.");
                return false;
            }

            JourneyData journey = LoadJourneyData(configPath);
            double averageSpeed = CalculateAverageSpeed(journey);

            SaveResult(resultPath, averageSpeed);
            Console.WriteLine("Average speed calculated and saved successfully.");
            Console.WriteLine("Average speed: " + averageSpeed.ToString("F2") + " km/h");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void CreateDefaultConfig(string configPath)
    {
        var defaultJourney = new JourneyData
        {
            TotalDistance = 100.0, // km
            TotalTime = 2.0 // hours
        };

        string json = JsonSerializer.Serialize(defaultJourney, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configPath, json);
    }

    private JourneyData LoadJourneyData(string configPath)
    {
        string json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<JourneyData>(json);
    }

    private double CalculateAverageSpeed(JourneyData journey)
    {
        if (journey.TotalTime <= 0)
        {
            throw new ArgumentException("Total time must be greater than zero.");
        }

        return journey.TotalDistance / journey.TotalTime;
    }

    private void SaveResult(string resultPath, double averageSpeed)
    {
        var result = new { AverageSpeed = averageSpeed, Unit = "km/h" };
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(resultPath, json);
    }

    private class JourneyData
    {
        public double TotalDistance { get; set; } // in kilometers
        public double TotalTime { get; set; } // in hours
    }
}