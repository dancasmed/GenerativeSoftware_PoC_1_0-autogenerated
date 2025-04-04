using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class UnitConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Unit Converter Module";

    private const string ConversionFactorsFile = "conversion_factors.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Unit Converter Module is running...");
        Console.WriteLine("Available conversions: kilometers-miles, celsius-fahrenheit, kilograms-pounds");

        string filePath = Path.Combine(dataFolder, ConversionFactorsFile);
        
        try
        {
            if (!File.Exists(filePath))
            {
                InitializeDefaultConversionFactors(filePath);
            }

            var conversionFactors = LoadConversionFactors(filePath);

            while (true)
            {
                Console.WriteLine("\nEnter conversion type (e.g., 'km mi' for kilometers to miles) or 'exit' to quit:");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "exit")
                {
                    break;
                }

                string[] parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Invalid format. Please use format 'fromUnit toUnit' (e.g., 'km mi').");
                    continue;
                }

                string fromUnit = parts[0];
                string toUnit = parts[1];
                string key = $"{fromUnit}_{toUnit}";

                if (!conversionFactors.ContainsKey(key))
                {
                    Console.WriteLine("Conversion not supported. Available conversions:");
                    foreach (var pair in conversionFactors)
                    {
                        string[] units = pair.Key.Split('_');
                        Console.WriteLine($"{units[0]} to {units[1]}");
                    }
                    continue;
                }

                Console.WriteLine("Enter value to convert:");
                if (!double.TryParse(Console.ReadLine(), out double value))
                {
                    Console.WriteLine("Invalid number format.");
                    continue;
                }

                double result = value * conversionFactors[key];
                Console.WriteLine($"{value} {fromUnit} = {result} {toUnit}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    private void InitializeDefaultConversionFactors(string filePath)
    {
        var defaultFactors = new Dictionary<string, double>
        {
            { "km_mi", 0.621371 },
            { "mi_km", 1.60934 },
            { "c_f", (9.0/5.0) },
            { "f_c", (5.0/9.0) },
            { "kg_lb", 2.20462 },
            { "lb_kg", 0.453592 }
        };

        string json = JsonSerializer.Serialize(defaultFactors);
        File.WriteAllText(filePath, json);
    }

    private Dictionary<string, double> LoadConversionFactors(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Dictionary<string, double>>(json);
    }
}