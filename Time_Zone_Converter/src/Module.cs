using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TimeZoneConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Time Zone Converter";

    private const string TimeZonesFileName = "timezones.json";
    private Dictionary<string, TimeZoneInfo> _timeZones;

    public TimeZoneConverterModule()
    {
        _timeZones = new Dictionary<string, TimeZoneInfo>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Time Zone Converter Module is running...");
        Console.WriteLine("Loading time zones...");

        string filePath = Path.Combine(dataFolder, TimeZonesFileName);
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            if (!File.Exists(filePath))
            {
                InitializeDefaultTimeZones(filePath);
            }

            LoadTimeZones(filePath);

            Console.WriteLine("Available time zones:");
            foreach (var tz in _timeZones)
            {
                Console.WriteLine(tz.Key);
            }

            Console.WriteLine("\nEnter source time zone:");
            string sourceTz = Console.ReadLine();

            Console.WriteLine("Enter target time zone:");
            string targetTz = Console.ReadLine();

            Console.WriteLine("Enter date and time to convert (yyyy-MM-dd HH:mm:ss):");
            string dateTimeInput = Console.ReadLine();

            if (DateTime.TryParse(dateTimeInput, out DateTime dateTime))
            {
                if (_timeZones.TryGetValue(sourceTz, out TimeZoneInfo sourceTimeZone) && 
                    _timeZones.TryGetValue(targetTz, out TimeZoneInfo targetTimeZone))
                {
                    DateTime convertedTime = TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, targetTimeZone);
                    Console.WriteLine("Converted time: " + convertedTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid time zone specified.");
                }
            }
            else
            {
                Console.WriteLine("Invalid date/time format.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return false;
    }

    private void InitializeDefaultTimeZones(string filePath)
    {
        var defaultTimeZones = new Dictionary<string, string>
        {
            { "UTC", "UTC" },
            { "EST", "Eastern Standard Time" },
            { "PST", "Pacific Standard Time" },
            { "CET", "Central European Standard Time" },
            { "GMT", "GMT Standard Time" }
        };

        var json = JsonSerializer.Serialize(defaultTimeZones);
        File.WriteAllText(filePath, json);
    }

    private void LoadTimeZones(string filePath)
    {
        string json = File.ReadAllText(filePath);
        var timeZoneNames = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        foreach (var tz in timeZoneNames)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(tz.Value);
                _timeZones.Add(tz.Key, timeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("Time zone not found: " + tz.Value);
            }
        }
    }
}