using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class SpeedConverterModule
{
    public string Name { get; set; }
    
    public SpeedConverterModule()
    {
        Name = "Speed Converter";
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Speed Converter Module is running.");
        Console.WriteLine("This module converts speeds between mph, kph, and knots.");
        
        string configPath = Path.Combine(dataFolder, "speed_converter_config.json");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            if (!File.Exists(configPath))
            {
                var defaultConfig = new Config { LastUsedUnit = "kph" };
                string json = JsonSerializer.Serialize(defaultConfig);
                File.WriteAllText(configPath, json);
            }
            
            string jsonConfig = File.ReadAllText(configPath);
            Config config = JsonSerializer.Deserialize<Config>(jsonConfig);
            
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Convert speed");
                Console.WriteLine("2. Change default unit (current: " + config.LastUsedUnit + ")");
                Console.WriteLine("3. Exit module");
                Console.Write("Select an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        ConvertSpeed(config.LastUsedUnit);
                        break;
                    case "2":
                        config.LastUsedUnit = ChangeDefaultUnit();
                        jsonConfig = JsonSerializer.Serialize(config);
                        File.WriteAllText(configPath, jsonConfig);
                        break;
                    case "3":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void ConvertSpeed(string defaultUnit)
    {
        Console.Write("Enter speed value: ");
        if (!double.TryParse(Console.ReadLine(), out double speed))
        {
            Console.WriteLine("Invalid speed value.");
            return;
        }
        
        Console.Write("Enter source unit (mph/kph/knots): ");
        string sourceUnit = Console.ReadLine().ToLower();
        
        if (sourceUnit != "mph" && sourceUnit != "kph" && sourceUnit != "knots")
        {
            Console.WriteLine("Invalid source unit.");
            return;
        }
        
        Console.Write("Enter target unit (mph/kph/knots): ");
        string targetUnit = Console.ReadLine().ToLower();
        
        if (targetUnit != "mph" && targetUnit != "kph" && targetUnit != "knots")
        {
            Console.WriteLine("Invalid target unit.");
            return;
        }
        
        double convertedSpeed = ConvertSpeedValue(speed, sourceUnit, targetUnit);
        Console.WriteLine(speed + " " + sourceUnit + " = " + convertedSpeed + " " + targetUnit);
    }
    
    private double ConvertSpeedValue(double speed, string sourceUnit, string targetUnit)
    {
        if (sourceUnit == targetUnit)
            return speed;
            
        // Convert to kph first
        double kph;
        switch (sourceUnit)
        {
            case "mph":
                kph = speed * 1.60934;
                break;
            case "knots":
                kph = speed * 1.852;
                break;
            default:
                kph = speed;
                break;
        }
        
        // Convert from kph to target unit
        switch (targetUnit)
        {
            case "mph":
                return kph / 1.60934;
            case "knots":
                return kph / 1.852;
            default:
                return kph;
        }
    }
    
    private string ChangeDefaultUnit()
    {
        while (true)
        {
            Console.Write("Enter default unit (mph/kph/knots): ");
            string unit = Console.ReadLine().ToLower();
            
            if (unit == "mph" || unit == "kph" || unit == "knots")
                return unit;
                
            Console.WriteLine("Invalid unit. Please enter mph, kph, or knots.");
        }
    }
}

public class Config
{
    public string LastUsedUnit { get; set; }
}