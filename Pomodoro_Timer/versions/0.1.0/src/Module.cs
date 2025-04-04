using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;

public class PomodoroTimer : IGeneratedModule
{
    public string Name { get; set; } = "Pomodoro Timer";
    
    private const int WorkDurationMinutes = 25;
    private const int ShortBreakDurationMinutes = 5;
    private const int LongBreakDurationMinutes = 15;
    private const int SessionsBeforeLongBreak = 4;
    
    private string _settingsPath;
    
    public PomodoroTimer()
    {
    }
    
    public bool Main(string dataFolder)
    {
        _settingsPath = Path.Combine(dataFolder, "pomodoro_settings.json");
        
        Console.WriteLine("Starting Pomodoro Timer...");
        Console.WriteLine("Work: 25 min | Short Break: 5 min | Long Break: 15 min");
        
        LoadSettings();
        
        int sessionCount = 0;
        bool running = true;
        
        while (running)
        {
            sessionCount++;
            
            Console.WriteLine("\nStarting work session #" + sessionCount);
            RunTimer(WorkDurationMinutes, "Work");
            
            if (sessionCount % SessionsBeforeLongBreak == 0)
            {
                Console.WriteLine("\nTime for a long break!");
                RunTimer(LongBreakDurationMinutes, "Long Break");
            }
            else
            {
                Console.WriteLine("\nTime for a short break!");
                RunTimer(ShortBreakDurationMinutes, "Short Break");
            }
            
            Console.WriteLine("\nPress 'q' to quit or any other key to continue...");
            if (Console.ReadKey().KeyChar == 'q')
            {
                running = false;
            }
        }
        
        SaveSettings();
        Console.WriteLine("Pomodoro Timer stopped.");
        return true;
    }
    
    private void RunTimer(int minutes, string mode)
    {
        int seconds = minutes * 60;
        DateTime endTime = DateTime.Now.AddMinutes(minutes);
        
        Console.WriteLine(mode + " session started at: " + DateTime.Now.ToString("HH:mm:ss"));
        Console.WriteLine("Session will end at: " + endTime.ToString("HH:mm:ss"));
        
        while (seconds > 0)
        {
            TimeSpan remaining = TimeSpan.FromSeconds(seconds);
            Console.Write("\r" + mode + " time remaining: " + remaining.ToString("mm':'ss"));
            Thread.Sleep(1000);
            seconds--;
            
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nSession interrupted!");
                return;
            }
        }
        
        Console.WriteLine("\n" + mode + " session completed!");
        Console.Beep();
    }
    
    private void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                // In future versions we could deserialize settings here
                Console.WriteLine("Previous session settings loaded.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading settings: " + ex.Message);
        }
    }
    
    private void SaveSettings()
    {
        try
        {
            var settings = new
            {
                LastRun = DateTime.Now
            };
            
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving settings: " + ex.Message);
        }
    }
}