using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class VacationPackageCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Vacation Package Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Vacation Package Calculator is running...");

        try
        {
            string configPath = Path.Combine(dataFolder, "vacation_config.json");
            string resultPath = Path.Combine(dataFolder, "vacation_result.json");

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please edit it and restart the module.");
                return false;
            }

            VacationConfig config = LoadConfig(configPath);
            VacationResult result = CalculateVacationCost(config);

            SaveResult(resultPath, result);
            Console.WriteLine("Vacation package cost calculated successfully!");
            Console.WriteLine("Total cost: " + result.TotalCost.ToString("C"));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void CreateDefaultConfig(string path)
    {
        var defaultConfig = new VacationConfig
        {
            FlightCost = 500,
            HotelCostPerNight = 150,
            NumberOfNights = 7,
            Activities = new Activity[]
            {
                new Activity { Name = "City Tour", Cost = 75 },
                new Activity { Name = "Museum Tickets", Cost = 25 }
            }
        };

        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    private VacationConfig LoadConfig(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<VacationConfig>(json);
    }

    private VacationResult CalculateVacationCost(VacationConfig config)
    {
        decimal activitiesCost = 0;
        foreach (var activity in config.Activities)
        {
            activitiesCost += activity.Cost;
        }

        decimal totalCost = config.FlightCost + 
                          (config.HotelCostPerNight * config.NumberOfNights) + 
                          activitiesCost;

        return new VacationResult
        {
            FlightCost = config.FlightCost,
            HotelCost = config.HotelCostPerNight * config.NumberOfNights,
            ActivitiesCost = activitiesCost,
            TotalCost = totalCost
        };
    }

    private void SaveResult(string path, VacationResult result)
    {
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}

public class VacationConfig
{
    public decimal FlightCost { get; set; }
    public decimal HotelCostPerNight { get; set; }
    public int NumberOfNights { get; set; }
    public Activity[] Activities { get; set; }
}

public class Activity
{
    public string Name { get; set; }
    public decimal Cost { get; set; }
}

public class VacationResult
{
    public decimal FlightCost { get; set; }
    public decimal HotelCost { get; set; }
    public decimal ActivitiesCost { get; set; }
    public decimal TotalCost { get; set; }
}