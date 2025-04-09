using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CalendarEventGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Calendar Event Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Calendar Event Generator module is running.");
        Console.WriteLine("This module helps you generate calendar events from text-based input.");

        try
        {
            string inputFilePath = Path.Combine(dataFolder, "input_events.txt");
            string outputFilePath = Path.Combine(dataFolder, "generated_events.json");

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found. Creating a sample input file.");
                CreateSampleInputFile(inputFilePath);
                Console.WriteLine("Please edit the input file and run the module again.");
                return false;
            }

            List<CalendarEvent> events = ParseInputFile(inputFilePath);

            if (events.Count == 0)
            {
                Console.WriteLine("No valid events found in the input file.");
                return false;
            }

            SaveEventsToJson(events, outputFilePath);
            Console.WriteLine("Successfully generated " + events.Count + " calendar events.");
            Console.WriteLine("Output saved to: " + outputFilePath);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void CreateSampleInputFile(string filePath)
    {
        string sampleContent = "Meeting with Team|2023-12-15|14:00|16:00|Discuss project progress\n" +
                             "Dentist Appointment|2023-12-20|09:30|10:30|Regular checkup\n" +
                             "Birthday Party|2023-12-25|19:00|23:00|John's 30th birthday";

        File.WriteAllText(filePath, sampleContent);
    }

    private List<CalendarEvent> ParseInputFile(string filePath)
    {
        List<CalendarEvent> events = new List<CalendarEvent>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');
            if (parts.Length >= 5)
            {
                try
                {
                    CalendarEvent calendarEvent = new CalendarEvent
                    {
                        Title = parts[0].Trim(),
                        Date = DateTime.Parse(parts[1].Trim()),
                        StartTime = TimeSpan.Parse(parts[2].Trim()),
                        EndTime = TimeSpan.Parse(parts[3].Trim()),
                        Description = parts[4].Trim()
                    };

                    events.Add(calendarEvent);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Skipping invalid event format: " + line);
                }
            }
            else
            {
                Console.WriteLine("Skipping incomplete event: " + line);
            }
        }

        return events;
    }

    private void SaveEventsToJson(List<CalendarEvent> events, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(events, options);
        File.WriteAllText(filePath, jsonString);
    }
}

public class CalendarEvent
{
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Description { get; set; }
}