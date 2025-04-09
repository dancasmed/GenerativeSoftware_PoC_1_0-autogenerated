using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class DrivingCostCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Driving Cost Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Driving Cost Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "driving_config.json");
            string resultPath = Path.Combine(dataFolder, "driving_result.json");
            
            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please update it with your values.");
                return false;
            }
            
            var config = LoadConfig(configPath);
            if (config == null)
            {
                Console.WriteLine("Failed to load configuration file.");
                return false;
            }
            
            double cost = CalculateCost(config.DistanceMiles, config.MilesPerGallon, config.FuelCostPerGallon);
            
            var result = new DrivingResult
            {
                EstimatedCost = cost,
                CalculationDate = DateTime.Now
            };
            
            SaveResult(resultPath, result);
            
            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Estimated driving cost: " + cost.ToString("C2"));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private DrivingConfig LoadConfig(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DrivingConfig>(json);
        }
        catch
        {
            return null;
        }
    }
    
    private void SaveResult(string path, DrivingResult result)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(result, options);
        File.WriteAllText(path, json);
    }
    
    private void CreateDefaultConfig(string path)
    {
        var defaultConfig = new DrivingConfig
        {
            DistanceMiles = 100,
            MilesPerGallon = 25,
            FuelCostPerGallon = 3.50
        };
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(defaultConfig, options);
        File.WriteAllText(path, json);
    }
    
    private double CalculateCost(double distance, double mpg, double fuelCost)
    {
        if (mpg <= 0 || distance <= 0)
            throw new ArgumentException("Distance and MPG must be greater than zero.");
            
        double gallonsNeeded = distance / mpg;
        return gallonsNeeded * fuelCost;
    }
}

public class DrivingConfig
{
    public double DistanceMiles { get; set; }
    public double MilesPerGallon { get; set; }
    public double FuelCostPerGallon { get; set; }
}

public class DrivingResult
{
    public double EstimatedCost { get; set; }
    public DateTime CalculationDate { get; set; }
}