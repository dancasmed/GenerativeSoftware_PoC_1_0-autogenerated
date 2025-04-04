using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class ScreenTimeTracker : IGeneratedModule
{
    public string Name { get; set; } = "Screen Time Tracker";
    
    private string _dataFilePath;
    private Dictionary<string, Dictionary<string, TimeSpan>> _screenTimeData;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Screen Time Tracker...");
        
        _dataFilePath = Path.Combine(dataFolder, "screentime_data.json");
        LoadData();
        
        DateTime currentDate = DateTime.Now.Date;
        string dateKey = currentDate.ToString("yyyy-MM-dd");
        
        Console.WriteLine("Tracking screen time for " + dateKey);
        Console.WriteLine("Press any key to stop tracking...");
        
        DateTime startTime = DateTime.Now;
        string currentApp = GetForegroundApplication();
        
        while (!Console.KeyAvailable)
        {
            string newApp = GetForegroundApplication();
            if (newApp != currentApp)
            {
                TimeSpan duration = DateTime.Now - startTime;
                UpdateAppUsage(dateKey, currentApp, duration);
                
                currentApp = newApp;
                startTime = DateTime.Now;
            }
            
            System.Threading.Thread.Sleep(1000);
        }
        
        // Record final app usage
        TimeSpan finalDuration = DateTime.Now - startTime;
        UpdateAppUsage(dateKey, currentApp, finalDuration);
        
        SaveData();
        
        Console.WriteLine("Screen time tracking stopped. Data saved.");
        return true;
    }
    
    private void LoadData()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _screenTimeData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, TimeSpan>>>(json);
            }
            else
            {
                _screenTimeData = new Dictionary<string, Dictionary<string, TimeSpan>>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading screen time data: " + ex.Message);
            _screenTimeData = new Dictionary<string, Dictionary<string, TimeSpan>>();
        }
    }
    
    private void SaveData()
    {
        try
        {
            string json = JsonSerializer.Serialize(_screenTimeData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving screen time data: " + ex.Message);
        }
    }
    
    private void UpdateAppUsage(string dateKey, string appName, TimeSpan duration)
    {
        if (!_screenTimeData.ContainsKey(dateKey))
        {
            _screenTimeData[dateKey] = new Dictionary<string, TimeSpan>();
        }
        
        if (_screenTimeData[dateKey].ContainsKey(appName))
        {
            _screenTimeData[dateKey][appName] += duration;
        }
        else
        {
            _screenTimeData[dateKey][appName] = duration;
        }
        
        Console.WriteLine("Recorded " + duration.ToString("hh\:mm\:ss") + " for " + appName);
    }
    
    private string GetForegroundApplication()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                return "Windows App";
            }
            else if (OperatingSystem.IsMacOS())
            {
                return "MacOS App";
            }
            else if (OperatingSystem.IsLinux())
            {
                return "Linux App";
            }
            else
            {
                return "Unknown Platform App";
            }
        }
        catch
        {
            return "Unknown App";
        }
    }
}