using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TaxiFareCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Taxi Fare Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Taxi Fare Calculator module is running.");

        string configPath = Path.Combine(dataFolder, "taxi_config.json");
        TaxiConfig config;

        if (!File.Exists(configPath))
        {
            config = new TaxiConfig
            {
                BaseFare = 3.00m,
                PerMileRate = 2.50m,
                PerMinuteRate = 0.50m,
                MinimumFare = 8.00m
            };

            string json = JsonSerializer.Serialize(config);
            File.WriteAllText(configPath, json);
            Console.WriteLine("Created default configuration file.");
        }
        else
        {
            string json = File.ReadAllText(configPath);
            config = JsonSerializer.Deserialize<TaxiConfig>(json);
        }

        Console.WriteLine("Current configuration:");
        Console.WriteLine("Base fare: " + config.BaseFare);
        Console.WriteLine("Per mile rate: " + config.PerMileRate);
        Console.WriteLine("Per minute rate: " + config.PerMinuteRate);
        Console.WriteLine("Minimum fare: " + config.MinimumFare);

        Console.WriteLine("\nEnter distance traveled in miles: ");
        string distanceInput = Console.ReadLine();

        Console.WriteLine("Enter time traveled in minutes: ");
        string timeInput = Console.ReadLine();

        if (decimal.TryParse(distanceInput, out decimal distance) && 
            decimal.TryParse(timeInput, out decimal time))
        {
            decimal fare = CalculateFare(distance, time, config);
            Console.WriteLine("Calculated fare: " + fare.ToString("C"));

            string receiptPath = Path.Combine(dataFolder, "receipts");
            Directory.CreateDirectory(receiptPath);
            
            string receiptFile = Path.Combine(receiptPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".json");
            var receipt = new 
            {
                Date = DateTime.Now,
                Distance = distance,
                Time = time,
                Fare = fare
            };
            
            File.WriteAllText(receiptFile, JsonSerializer.Serialize(receipt));
            Console.WriteLine("Receipt saved to " + receiptFile);
            
            return true;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter numeric values for distance and time.");
            return false;
        }
    }

    private decimal CalculateFare(decimal distance, decimal time, TaxiConfig config)
    {
        decimal fare = config.BaseFare + 
                      (distance * config.PerMileRate) + 
                      (time * config.PerMinuteRate);
        
        return Math.Max(fare, config.MinimumFare);
    }
}

public class TaxiConfig
{
    public decimal BaseFare { get; set; }
    public decimal PerMileRate { get; set; }
    public decimal PerMinuteRate { get; set; }
    public decimal MinimumFare { get; set; }
}