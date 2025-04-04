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
        Console.WriteLine("Screen Time Tracker module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "screentime_data.json");
        LoadData();
        
        var today = DateTime.Now.Date.ToString("yyyy-MM-dd");
        var currentApp = GetCurrentAppName();
        var startTime = DateTime.Now;
        
        Console.WriteLine("Tracking screen time. Press any key to stop...");
        Console.ReadKey(true);
        
        var endTime = DateTime.Now;
        var duration = endTime - startTime;
        
        UpdateScreenTime(today, currentApp, duration);
        SaveData();
        
        DisplayDailySummary(today);
        
        return true;
    }
    
    private string GetCurrentAppName()
    {
        try
        {
            return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        }
        catch
        {
            return "Unknown App";
        }
    }
    
    private void LoadData()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                var json = File.ReadAllText(_dataFilePath);
                _screenTimeData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, TimeSpan>>>(json);
            }
            else
            {
                _screenTimeData = new Dictionary<string, Dictionary<string, TimeSpan>>();
            }
        }
        catch
        {
            _screenTimeData = new Dictionary<string, Dictionary<string, TimeSpan>>();
        }
    }
    
    private void SaveData()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_screenTimeData, options);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving data: " + ex.Message);
        }
    }
    
    private void UpdateScreenTime(string date, string appName, TimeSpan duration)
    {
        if (!_screenTimeData.ContainsKey(date))
        {
            _screenTimeData[date] = new Dictionary<string, TimeSpan>();
        }
        
        if (!_screenTimeData[date].ContainsKey(appName))
        {
            _screenTimeData[date][appName] = TimeSpan.Zero;
        }
        
        _screenTimeData[date][appName] += duration;
    }
    
    private void DisplayDailySummary(string date)
    {
        if (_screenTimeData.TryGetValue(date, out var dailyData))
        {
            Console.WriteLine("\nDaily Screen Time Summary for " + date + ":");
            Console.WriteLine("--------------------------------");
            
            var totalTime = TimeSpan.Zero;
            foreach (var entry in dailyData.OrderByDescending(e => e.Value))
            {
                Console.WriteLine(entry.Key + ": " + entry.Value.ToString(@"hh\:mm\:ss"));
                totalTime += entry.Value;
            }
            
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Total: " + totalTime.ToString(@"hh\:mm\:ss"));
        }
        else
        {
            Console.WriteLine("No screen time data available for " + date);
        }
    }
}