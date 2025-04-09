using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class JourneyTimeCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Journey Time Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Journey Time Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "journey_config.json");
            JourneyConfig config;
            
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<JourneyConfig>(json);
                Console.WriteLine("Loaded existing journey configuration.");
            }
            else
            {
                config = new JourneyConfig
                {
                    Distance = 100,
                    AverageSpeed = 60
                };
                
                string json = JsonSerializer.Serialize(config);
                File.WriteAllText(configPath, json);
                Console.WriteLine("Created default journey configuration.");
            }
            
            double time = CalculateJourneyTime(config.Distance, config.AverageSpeed);
            Console.WriteLine("Journey Time: " + time.ToString("F2") + " hours");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private double CalculateJourneyTime(double distance, double averageSpeed)
    {
        if (averageSpeed <= 0)
        {
            throw new ArgumentException("Average speed must be greater than zero.");
        }
        
        return distance / averageSpeed;
    }
}

public class JourneyConfig
{
    public double Distance { get; set; }
    public double AverageSpeed { get; set; }
}