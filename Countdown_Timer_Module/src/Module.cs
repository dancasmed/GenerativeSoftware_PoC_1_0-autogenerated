using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;

public class CountdownTimerModule : IGeneratedModule
{
    public string Name { get; set; } = "Countdown Timer Module";

    private const string TimerDataFile = "countdown_timer.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Countdown Timer Module is running...");
        Console.WriteLine("This module helps you track important events with a countdown timer.");

        string timerDataPath = Path.Combine(dataFolder, TimerDataFile);
        TimerData timerData = LoadTimerData(timerDataPath);

        if (timerData == null || !timerData.IsValid())
        {
            timerData = CreateNewTimerData();
            SaveTimerData(timerDataPath, timerData);
        }

        Console.WriteLine("Current countdown timer:");
        Console.WriteLine("Event: " + timerData.EventName);
        Console.WriteLine("Target Date: " + timerData.TargetDate.ToString("yyyy-MM-dd HH:mm:ss"));

        StartCountdown(timerData.TargetDate);

        return true;
    }

    private TimerData LoadTimerData(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<TimerData>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading timer data: " + ex.Message);
        }
        return null;
    }

    private void SaveTimerData(string filePath, TimerData timerData)
    {
        try
        {
            string json = JsonSerializer.Serialize(timerData);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving timer data: " + ex.Message);
        }
    }

    private TimerData CreateNewTimerData()
    {
        Console.WriteLine("No valid timer found. Creating a new one...");
        Console.Write("Enter event name: ");
        string eventName = Console.ReadLine();

        DateTime targetDate;
        while (true)
        {
            Console.Write("Enter target date (yyyy-MM-dd HH:mm:ss): ");
            if (DateTime.TryParse(Console.ReadLine(), out targetDate))
            {
                if (targetDate > DateTime.Now)
                {
                    break;
                }
                Console.WriteLine("Target date must be in the future.");
            }
            else
            {
                Console.WriteLine("Invalid date format. Please try again.");
            }
        }

        return new TimerData
        {
            EventName = eventName,
            TargetDate = targetDate
        };
    }

    private void StartCountdown(DateTime targetDate)
    {
        Console.WriteLine("Countdown started. Press any key to exit...");
        Console.WriteLine();

        while (!Console.KeyAvailable)
        {
            TimeSpan remaining = targetDate - DateTime.Now;

            if (remaining.TotalSeconds <= 0)
            {
                Console.WriteLine("Event time has arrived!");
                break;
            }

            Console.Write("Time remaining: ");
            Console.Write(remaining.Days + " days, ");
            Console.Write(remaining.Hours + " hours, ");
            Console.Write(remaining.Minutes + " minutes, ");
            Console.Write(remaining.Seconds + " seconds");
            Console.SetCursorPosition(0, Console.CursorTop);

            Thread.Sleep(1000);
        }
    }

    private class TimerData
    {
        public string EventName { get; set; }
        public DateTime TargetDate { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EventName) && TargetDate > DateTime.Now;
        }
    }
}