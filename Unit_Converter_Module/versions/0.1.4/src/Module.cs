using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class UnitConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Unit Converter Module";

    private const string DataFileName = "conversion_history.json";
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Unit Converter Module is running...");
        Console.WriteLine("Available units: kilometers (km), miles (mi), meters (m), feet (ft)");
        
        try
        {
            string dataFilePath = Path.Combine(dataFolder, DataFileName);
            
            while (true)
            {
                Console.WriteLine("\nEnter conversion (e.g., '5 km to mi') or 'exit' to quit:");
                string input = Console.ReadLine();
                
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                
                try
                {
                    if (TryParseInput(input, out double value, out string fromUnit, out string toUnit))
                    {
                        double result = ConvertUnit(value, fromUnit, toUnit);
                        Console.WriteLine($"{value} {fromUnit} = {result} {toUnit}");
                        
                        SaveConversion(dataFilePath, value, fromUnit, toUnit, result);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input format. Please use format like '5 km to mi'.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            return false;
        }
    }
    
    private bool TryParseInput(string input, out double value, out string fromUnit, out string toUnit)
    {
        value = 0;
        fromUnit = string.Empty;
        toUnit = string.Empty;
        
        string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length != 4 || !parts[2].Equals("to", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        
        if (!double.TryParse(parts[0], out value))
        {
            return false;
        }
        
        fromUnit = parts[1].ToLower();
        toUnit = parts[3].ToLower();
        
        string[] validUnits = { "km", "mi", "m", "ft" };
        
        bool fromUnitValid = false;
        bool toUnitValid = false;
        
        foreach (var unit in validUnits)
        {
            if (unit.Equals(fromUnit, StringComparison.OrdinalIgnoreCase))
                fromUnitValid = true;
            if (unit.Equals(toUnit, StringComparison.OrdinalIgnoreCase))
                toUnitValid = true;
        }
        
        return fromUnitValid && toUnitValid;
    }
    
    private double ConvertUnit(double value, string fromUnit, string toUnit)
    {
        // Convert to meters first (base unit)
        double meters = 0;
        
        switch (fromUnit)
        {
            case "km":
                meters = value * 1000;
                break;
            case "mi":
                meters = value * 1609.344;
                break;
            case "m":
                meters = value;
                break;
            case "ft":
                meters = value * 0.3048;
                break;
        }
        
        // Convert from meters to target unit
        switch (toUnit)
        {
            case "km":
                return meters / 1000;
            case "mi":
                return meters / 1609.344;
            case "m":
                return meters;
            case "ft":
                return meters / 0.3048;
            default:
                throw new ArgumentException("Invalid target unit");
        }
    }
    
    private void SaveConversion(string filePath, double value, string fromUnit, string toUnit, double result)
    {
        try
        {
            var conversionData = new
            {
                Timestamp = DateTime.UtcNow,
                Value = value,
                FromUnit = fromUnit,
                ToUnit = toUnit,
                Result = result
            };
            
            string jsonData = JsonSerializer.Serialize(conversionData);
            
            // Append to file or create new
            File.AppendAllText(filePath, jsonData + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not save conversion history: {ex.Message}");
        }
    }
}