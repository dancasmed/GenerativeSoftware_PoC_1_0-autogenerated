using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TimeZoneConverter : IGeneratedModule
{
    public string Name { get; set; } = "TimeZoneConverter";

    private string _timeZonesFilePath;
    private Dictionary<string, TimeZoneInfo> _timeZones;

    public TimeZoneConverter()
    {
        _timeZones = new Dictionary<string, TimeZoneInfo>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("TimeZoneConverter module is running.");
        _timeZonesFilePath = Path.Combine(dataFolder, "timezones.json");

        try
        {
            LoadTimeZones();
            if (_timeZones.Count == 0)
            {
                InitializeDefaultTimeZones();
                SaveTimeZones();
            }

            Console.WriteLine("Available time zones:");
            foreach (var tz in _timeZones.Keys)
            {
                Console.WriteLine(tz);
            }

            Console.WriteLine("\nEnter source time zone:");
            string sourceTz = Console.ReadLine();

            Console.WriteLine("Enter target time zone:");
            string targetTz = Console.ReadLine();

            Console.WriteLine("Enter date and time (yyyy-MM-dd HH:mm:ss):");
            string dateTimeInput = Console.ReadLine();

            if (DateTime.TryParse(dateTimeInput, out DateTime sourceDateTime))
            {
                if (_timeZones.TryGetValue(sourceTz, out TimeZoneInfo sourceTimeZone) &&
                    _timeZones.TryGetValue(targetTz, out TimeZoneInfo targetTimeZone))
                {
                    DateTime convertedTime = TimeZoneInfo.ConvertTime(sourceDateTime, sourceTimeZone, targetTimeZone);
                    Console.WriteLine("Converted time: " + convertedTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + targetTz);
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid time zone specified.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Invalid date/time format.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void LoadTimeZones()
    {
        if (File.Exists(_timeZonesFilePath))
        {
            string json = File.ReadAllText(_timeZonesFilePath);
            var timeZoneIds = JsonSerializer.Deserialize<List<string>>(json);

            foreach (var id in timeZoneIds)
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(id);
                    _timeZones[tz.DisplayName] = tz;
                }
                catch { }
            }
        }
    }

    private void SaveTimeZones()
    {
        var timeZoneIds = new List<string>();
        foreach (var tz in _timeZones.Values)
        {
            timeZoneIds.Add(tz.Id);
        }

        string json = JsonSerializer.Serialize(timeZoneIds);
        File.WriteAllText(_timeZonesFilePath, json);
    }

    private void InitializeDefaultTimeZones()
    {
        try
        {
            AddTimeZoneIfExists("Pacific Standard Time");
            AddTimeZoneIfExists("Eastern Standard Time");
            AddTimeZoneIfExists("Central Standard Time");
            AddTimeZoneIfExists("Mountain Standard Time");
            AddTimeZoneIfExists("UTC");
            AddTimeZoneIfExists("GMT Standard Time");
            AddTimeZoneIfExists("Central European Standard Time");
            AddTimeZoneIfExists("Tokyo Standard Time");
        }
        catch { }
    }

    private void AddTimeZoneIfExists(string id)
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(id);
            _timeZones[tz.DisplayName] = tz;
        }
        catch { }
    }
}