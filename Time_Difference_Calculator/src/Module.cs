using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TimeDifferenceCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Time Difference Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Time Difference Calculator module is running.");
        
        string configPath = Path.Combine(dataFolder, "config.json");
        string resultPath = Path.Combine(dataFolder, "result.json");

        try
        {
            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please fill in the timestamps and run again.");
                return false;
            }

            var config = LoadConfig(configPath);
            if (config == null)
            {
                Console.WriteLine("Failed to load configuration.");
                return false;
            }

            if (!DateTime.TryParse(config.StartTimestamp, out DateTime startTime) || 
                !DateTime.TryParse(config.EndTimestamp, out DateTime endTime))
            {
                Console.WriteLine("Invalid timestamp format in configuration.");
                return false;
            }

            TimeSpan difference = endTime - startTime;
            var result = new TimeDifferenceResult
            {
                StartTimestamp = config.StartTimestamp,
                EndTimestamp = config.EndTimestamp,
                DifferenceInDays = difference.TotalDays,
                DifferenceInHours = difference.TotalHours,
                DifferenceInMinutes = difference.TotalMinutes,
                DifferenceInSeconds = difference.TotalSeconds
            };

            SaveResult(resultPath, result);
            Console.WriteLine("Time difference calculated and saved successfully.");
            Console.WriteLine("Total difference: " + difference.ToString());
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void CreateDefaultConfig(string configPath)
    {
        var defaultConfig = new TimeDifferenceConfig
        {
            StartTimestamp = "2023-01-01T00:00:00",
            EndTimestamp = "2023-01-02T00:00:00"
        };

        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configPath, json);
    }

    private TimeDifferenceConfig LoadConfig(string configPath)
    {
        string json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<TimeDifferenceConfig>(json);
    }

    private void SaveResult(string resultPath, TimeDifferenceResult result)
    {
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(resultPath, json);
    }
}

public class TimeDifferenceConfig
{
    public string StartTimestamp { get; set; }
    public string EndTimestamp { get; set; }
}

public class TimeDifferenceResult
{
    public string StartTimestamp { get; set; }
    public string EndTimestamp { get; set; }
    public double DifferenceInDays { get; set; }
    public double DifferenceInHours { get; set; }
    public double DifferenceInMinutes { get; set; }
    public double DifferenceInSeconds { get; set; }
}