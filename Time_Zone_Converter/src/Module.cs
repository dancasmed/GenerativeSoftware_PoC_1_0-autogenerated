using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TimeZoneConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Time Zone Converter";

    private string _timeZonesFilePath;
    private Dictionary<string, TimeZoneInfo> _timeZones;

    public TimeZoneConverterModule()
    {
        _timeZones = new Dictionary<string, TimeZoneInfo>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Time Zone Converter Module is running...");
        _timeZonesFilePath = Path.Combine(dataFolder, "timezones.json");

        try
        {
            LoadTimeZones();
            if (_timeZones.Count == 0)
            {
                InitializeDefaultTimeZones();
                SaveTimeZones();
            }

            Console.WriteLine("Available Time Zones:");
            foreach (var tz in _timeZones)
            {
                Console.WriteLine(tz.Key);
            }

            Console.WriteLine("\nEnter source time zone ID:");
            string sourceTzId = Console.ReadLine();

            Console.WriteLine("Enter target time zone ID:");
            string targetTzId = Console.ReadLine();

            Console.WriteLine("Enter date and time (yyyy-MM-dd HH:mm:ss):");
            string dateTimeInput = Console.ReadLine();

            if (DateTime.TryParse(dateTimeInput, out DateTime sourceDateTime))
            {
                if (_timeZones.TryGetValue(sourceTzId, out TimeZoneInfo sourceTz) && 
                    _timeZones.TryGetValue(targetTzId, out TimeZoneInfo targetTz))
                {
                    DateTime targetDateTime = TimeZoneInfo.ConvertTime(sourceDateTime, sourceTz, targetTz);
                    Console.WriteLine("Converted Time: " + targetDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid time zone ID(s).");
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

    private void LoadTimeZones()
    {
        if (File.Exists(_timeZonesFilePath))
        {
            string json = File.ReadAllText(_timeZonesFilePath);
            var timeZoneData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            foreach (var item in timeZoneData)
            {
                try
                {
                    _timeZones[item.Key] = TimeZoneInfo.FindSystemTimeZoneById(item.Value);
                }
                catch
                {
                    Console.WriteLine("Warning: Time zone " + item.Value + " not found.");
                }
            }
        }
    }

    private void SaveTimeZones()
    {
        var timeZoneData = new Dictionary<string, string>();
        foreach (var tz in _timeZones)
        {
            timeZoneData[tz.Key] = tz.Value.Id;
        }

        string json = JsonSerializer.Serialize(timeZoneData);
        File.WriteAllText(_timeZonesFilePath, json);
    }

    private void InitializeDefaultTimeZones()
    {
        try
        {
            _timeZones["UTC"] = TimeZoneInfo.Utc;
            _timeZones["Eastern (US)"] = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            _timeZones["Central (US)"] = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            _timeZones["Pacific (US)"] = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            _timeZones["London"] = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            _timeZones["Tokyo"] = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Warning: Could not initialize default time zones. " + ex.Message);
        }
    }
}