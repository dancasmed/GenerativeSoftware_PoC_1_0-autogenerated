using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TemperatureConverter : IGeneratedModule
{
    public string Name { get; set; } = "Temperature Converter";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Temperature Converter Module is running.");
        Console.WriteLine("This module converts temperatures between Celsius, Fahrenheit, and Kelvin.");

        try
        {
            string configPath = Path.Combine(dataFolder, "temperature_config.json");
            if (!File.Exists(configPath))
            {
                var defaultConfig = new
                {
                    LastUsedScale = "Celsius",
                    DefaultTargetScale = "Fahrenheit"
                };
                File.WriteAllText(configPath, JsonSerializer.Serialize(defaultConfig));
            }

            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));

            while (true)
            {
                Console.WriteLine("\nEnter temperature value (or 'q' to quit):");
                string input = Console.ReadLine();
                if (input.ToLower() == "q") break;

                if (!double.TryParse(input, out double temperature))
                {
                    Console.WriteLine("Invalid input. Please enter a numeric value.");
                    continue;
                }

                Console.WriteLine("Enter source scale (Celsius, Fahrenheit, Kelvin):");
                string sourceScale = Console.ReadLine().Trim();

                Console.WriteLine("Enter target scale (Celsius, Fahrenheit, Kelvin):");
                string targetScale = Console.ReadLine().Trim();

                double convertedTemp = ConvertTemperature(temperature, sourceScale, targetScale);
                Console.WriteLine(string.Format("{0} {1} is {2} {3}", temperature, sourceScale, convertedTemp, targetScale));

                // Update config
                config.LastUsedScale = sourceScale;
                config.DefaultTargetScale = targetScale;
                File.WriteAllText(configPath, JsonSerializer.Serialize(config));
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("An error occurred: {0}", ex.Message));
            return false;
        }
    }

    private double ConvertTemperature(double value, string fromScale, string toScale)
    {
        fromScale = fromScale.ToLower();
        toScale = toScale.ToLower();

        // First convert to Celsius
        double celsius;
        switch (fromScale)
        {
            case "celsius":
                celsius = value;
                break;
            case "fahrenheit":
                celsius = (value - 32) * 5 / 9;
                break;
            case "kelvin":
                celsius = value - 273.15;
                break;
            default:
                throw new ArgumentException("Invalid source temperature scale");
        }

        // Then convert from Celsius to target scale
        switch (toScale)
        {
            case "celsius":
                return celsius;
            case "fahrenheit":
                return celsius * 9 / 5 + 32;
            case "kelvin":
                return celsius + 273.15;
            default:
                throw new ArgumentException("Invalid target temperature scale");
        }
    }

    private class Config
    {
        public string LastUsedScale { get; set; }
        public string DefaultTargetScale { get; set; }
    }
}