using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DistanceConverterModule : IGeneratedModule
{
    public string Name { get; set; }

    public DistanceConverterModule()
    {
        Name = "Distance Converter Module";
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Distance Converter Module is running");
        
        var history = LoadHistory(dataFolder);
        var converter = new DistanceConverter();
        
        while (true)
        {
            Console.WriteLine("\n1. Convert distance");
            Console.WriteLine("2. Show conversion history");
            Console.WriteLine("3. Exit");
            Console.Write("Select option: ");
            
            if (!int.TryParse(Console.ReadLine(), out var choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    PerformConversion(converter, ref history, dataFolder);
                    break;
                case 2:
                    ShowHistory(history);
                    break;
                case 3:
                    SaveHistory(history, dataFolder);
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private List<ConversionHistoryEntry> LoadHistory(string dataFolder)
    {
        var path = Path.Combine(dataFolder, "conversion_history.json");
        try
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<ConversionHistoryEntry>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading history: " + ex.Message);
        }
        return new List<ConversionHistoryEntry>();
    }

    private void SaveHistory(List<ConversionHistoryEntry> history, string dataFolder)
    {
        try
        {
            Directory.CreateDirectory(dataFolder);
            var path = Path.Combine(dataFolder, "conversion_history.json");
            var json = JsonSerializer.Serialize(history);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving history: " + ex.Message);
        }
    }

    private void PerformConversion(DistanceConverter converter, ref List<ConversionHistoryEntry> history, string dataFolder)
    {
        var request = GetConversionRequest();
        if (request == null) return;

        var result = converter.Convert(request);
        if (result == null)
        {
            Console.WriteLine("Invalid conversion request");
            return;
        }

        history.Add(new ConversionHistoryEntry
        {
            Timestamp = DateTime.Now,
            Request = request,
            Result = result
        });

        SaveHistory(history, dataFolder);
        Console.WriteLine("\nConversion result: " + 
            result.OriginalValue.ToString("0.##") + " " + result.OriginalUnit + " = " + 
            result.ConvertedValue.ToString("0.##") + " " + result.ConvertedUnit);
    }

    private ConversionRequest GetConversionRequest()
    {
        Console.Write("Enter value to convert: ");
        if (!float.TryParse(Console.ReadLine(), out float value))
        {
            Console.WriteLine("Invalid numeric value");
            return null;
        }

        var sourceUnit = GetUnitSelection("Select source unit");
        var targetUnit = GetUnitSelection("Select target unit");

        if (sourceUnit == null || targetUnit == null)
        {
            Console.WriteLine("Invalid unit selection");
            return null;
        }

        return new ConversionRequest
        {
            Value = value,
            SourceUnit = sourceUnit,
            TargetUnit = targetUnit
        };
    }

    private string GetUnitSelection(string prompt)
    {
        Console.WriteLine(prompt + ":");
        Console.WriteLine("1. Kilometers");
        Console.WriteLine("2. Miles");
        Console.WriteLine("3. Meters");
        Console.WriteLine("4. Feet");
        Console.Write("Select unit: ");
        
        return int.TryParse(Console.ReadLine(), out int choice) ? choice switch
        {
            1 => "kilometers",
            2 => "miles",
            3 => "meters",
            4 => "feet",
            _ => null
        } : null;
    }

    private void ShowHistory(List<ConversionHistoryEntry> history)
    {
        Console.WriteLine("\nConversion History:");
        foreach (var entry in history)
        {
            Console.WriteLine("[{0}] {1} {2} -> {3} {4}",
                entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                entry.Request.Value,
                entry.Request.SourceUnit,
                entry.Result.ConvertedValue.ToString("0.##"),
                entry.Result.ConvertedUnit);
        }
    }
}

public class DistanceConverter
{
    private readonly Dictionary<string, float> _conversionFactors = new()
    {
        { "kilometers", 1000 },
        { "miles", 1609.34f },
        { "meters", 1 },
        { "feet", 0.3048f }
    };

    public ConversionResult Convert(ConversionRequest request)
    {
        if (!_conversionFactors.ContainsKey(request.SourceUnit) return null;
        if (!_conversionFactors.ContainsKey(request.TargetUnit)) return null;

        var meters = request.Value * _conversionFactors[request.SourceUnit];
        var result = meters / _conversionFactors[request.TargetUnit];

        return new ConversionResult
        {
            OriginalValue = request.Value,
            OriginalUnit = request.SourceUnit,
            ConvertedValue = result,
            ConvertedUnit = request.TargetUnit
        };
    }
}

public class ConversionRequest
{
    public float Value { get; set; }
    public string SourceUnit { get; set; }
    public string TargetUnit { get; set; }
}

public class ConversionResult
{
    public float OriginalValue { get; set; }
    public string OriginalUnit { get; set; }
    public float ConvertedValue { get; set; }
    public string ConvertedUnit { get; set; }
}

public class ConversionHistoryEntry
{
    public DateTime Timestamp { get; set; }
    public ConversionRequest Request { get; set; }
    public ConversionResult Result { get; set; }
}