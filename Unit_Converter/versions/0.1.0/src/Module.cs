using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class UnitConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Unit Converter";

    private const string ConfigFileName = "unit_converter_config.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Unit Converter Module is running...");
        Console.WriteLine("Available units: kilometers (km), miles (mi), meters (m), feet (ft)");

        try
        {
            string configPath = Path.Combine(dataFolder, ConfigFileName);
            UnitConverterConfig config = LoadConfig(configPath);

            while (true)
            {
                Console.WriteLine("\nEnter conversion (e.g., '5 km to mi') or 'exit' to quit:");
                string input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    SaveConfig(config, configPath);
                    Console.WriteLine("Unit Converter Module finished.");
                    return true;
                }

                if (TryParseInput(input, out double value, out string fromUnit, out string toUnit))
                {
                    try
                    {
                        double result = ConvertUnit(value, fromUnit, toUnit);
                        Console.WriteLine($"{value} {fromUnit} = {result} {toUnit}");
                        config.LastConversion = $"{value} {fromUnit} to {toUnit}";
                        config.LastResult = result;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input format. Please use format like '5 km to mi'.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    private UnitConverterConfig LoadConfig(string configPath)
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<UnitConverterConfig>(json) ?? new UnitConverterConfig();
        }
        return new UnitConverterConfig();
    }

    private void SaveConfig(UnitConverterConfig config, string configPath)
    {
        string json = JsonSerializer.Serialize(config);
        File.WriteAllText(configPath, json);
    }

    private bool TryParseInput(string input, out double value, out string fromUnit, out string toUnit)
    {
        value = 0;
        fromUnit = string.Empty;
        toUnit = string.Empty;

        string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 4 || !parts[2].Equals("to", StringComparison.OrdinalIgnoreCase))
            return false;

        if (!double.TryParse(parts[0], out value))
            return false;

        fromUnit = parts[1].ToLower();
        toUnit = parts[3].ToLower();

        return true;
    }

    private double ConvertUnit(double value, string fromUnit, string toUnit)
    {
        double meters = fromUnit switch
        {
            "km" => value * 1000,
            "mi" => value * 1609.344,
            "m" => value,
            "ft" => value * 0.3048,
            _ => throw new ArgumentException($"Unknown unit: {fromUnit}")
        };

        return toUnit switch
        {
            "km" => meters / 1000,
            "mi" => meters / 1609.344,
            "m" => meters,
            "ft" => meters / 0.3048,
            _ => throw new ArgumentException($"Unknown unit: {toUnit}")
        };
    }

    private class UnitConverterConfig
    {
        public string LastConversion { get; set; } = string.Empty;
        public double LastResult { get; set; } = 0;
    }
}