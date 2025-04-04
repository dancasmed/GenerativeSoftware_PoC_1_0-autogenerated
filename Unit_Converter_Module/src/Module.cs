using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class UnitConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Unit Converter Module";

    private const string ConfigFileName = "unit_converter_config.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Unit Converter Module started");
        Console.WriteLine("Available units: kilometers (km), miles (mi), meters (m), feet (ft)");

        try
        {
            var configPath = Path.Combine(dataFolder, ConfigFileName);
            UnitConverterConfig config = LoadOrCreateConfig(configPath);

            while (true)
            {
                Console.WriteLine("\nEnter conversion (e.g., '5 km to mi') or 'exit' to quit:");
                string input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (input == "exit")
                    break;

                ProcessConversion(input, config, configPath);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private UnitConverterConfig LoadOrCreateConfig(string configPath)
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<UnitConverterConfig>(json) ?? new UnitConverterConfig();
        }
        else
        {
            var config = new UnitConverterConfig();
            SaveConfig(config, configPath);
            return config;
        }
    }

    private void SaveConfig(UnitConverterConfig config, string configPath)
    {
        string json = JsonSerializer.Serialize(config);
        File.WriteAllText(configPath, json);
    }

    private void ProcessConversion(string input, UnitConverterConfig config, string configPath)
    {
        string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 4 || parts[2] != "to")
        {
            Console.WriteLine("Invalid format. Use: 'value fromUnit to toUnit' (e.g., '5 km to mi')");
            return;
        }

        if (!double.TryParse(parts[0], out double value))
        {
            Console.WriteLine("Invalid numeric value");
            return;
        }

        string fromUnit = parts[1];
        string toUnit = parts[3];

        double result = ConvertUnit(value, fromUnit, toUnit);

        if (double.IsNaN(result))
        {
            Console.WriteLine("Invalid unit conversion. Supported units: km, mi, m, ft");
            return;
        }

        Console.WriteLine(string.Format("{0} {1} = {2:0.######} {3}", value, fromUnit, result, toUnit));

        // Save to history
        config.ConversionHistory.Add(new ConversionRecord
        {
            Value = value,
            FromUnit = fromUnit,
            ToUnit = toUnit,
            Result = result,
            Timestamp = DateTime.Now
        });

        SaveConfig(config, configPath);
    }

    private double ConvertUnit(double value, string fromUnit, string toUnit)
    {
        // First convert to meters (base unit)
        double inMeters;

        switch (fromUnit)
        {
            case "km":
                inMeters = value * 1000;
                break;
            case "mi":
                inMeters = value * 1609.344;
                break;
            case "m":
                inMeters = value;
                break;
            case "ft":
                inMeters = value * 0.3048;
                break;
            default:
                return double.NaN;
        }

        // Then convert from meters to target unit
        switch (toUnit)
        {
            case "km":
                return inMeters / 1000;
            case "mi":
                return inMeters / 1609.344;
            case "m":
                return inMeters;
            case "ft":
                return inMeters / 0.3048;
            default:
                return double.NaN;
        }
    }

    private class UnitConverterConfig
    {
        public List<ConversionRecord> ConversionHistory { get; set; } = new List<ConversionRecord>();
    }

    private class ConversionRecord
    {
        public double Value { get; set; }
        public string FromUnit { get; set; } = string.Empty;
        public string ToUnit { get; set; } = string.Empty;
        public double Result { get; set; }
        public DateTime Timestamp { get; set; }
    }
}