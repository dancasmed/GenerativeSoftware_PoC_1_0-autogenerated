using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TaxiFareCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Taxi Fare Calculator";

    private const double BaseFare = 3.00;
    private const double RatePerMile = 1.50;
    private const double RatePerMinute = 0.20;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Taxi Fare Calculator module is running.");

        try
        {
            string configPath = Path.Combine(dataFolder, "taxi_config.json");
            TaxiConfig config = LoadConfig(configPath);

            Console.WriteLine("Enter distance traveled (miles): ");
            double distance = double.Parse(Console.ReadLine());

            Console.WriteLine("Enter time taken (minutes): ");
            double time = double.Parse(Console.ReadLine());

            double fare = CalculateFare(distance, time, config);
            Console.WriteLine("Calculated fare: " + fare.ToString("C2"));

            SaveFareRecord(dataFolder, distance, time, fare);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private double CalculateFare(double distance, double time, TaxiConfig config)
    {
        return config.BaseFare + 
               (distance * config.RatePerMile) + 
               (time * config.RatePerMinute);
    }

    private TaxiConfig LoadConfig(string configPath)
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<TaxiConfig>(json);
        }
        else
        {
            var defaultConfig = new TaxiConfig
            {
                BaseFare = BaseFare,
                RatePerMile = RatePerMile,
                RatePerMinute = RatePerMinute
            };

            string json = JsonSerializer.Serialize(defaultConfig);
            File.WriteAllText(configPath, json);
            return defaultConfig;
        }
    }

    private void SaveFareRecord(string dataFolder, double distance, double time, double fare)
    {
        string recordsPath = Path.Combine(dataFolder, "fare_records.json");
        var record = new FareRecord
        {
            Timestamp = DateTime.Now,
            Distance = distance,
            Time = time,
            Fare = fare
        };

        string json = JsonSerializer.Serialize(record);
        File.AppendAllText(recordsPath, json + Environment.NewLine);
    }
}

public class TaxiConfig
{
    public double BaseFare { get; set; }
    public double RatePerMile { get; set; }
    public double RatePerMinute { get; set; }
}

public class FareRecord
{
    public DateTime Timestamp { get; set; }
    public double Distance { get; set; }
    public double Time { get; set; }
    public double Fare { get; set; }
}