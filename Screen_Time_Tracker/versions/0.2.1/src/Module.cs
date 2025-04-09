using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ScreenTimeTracker : IGeneratedModule
{
    public string Name { get; set; } = "Screen Time Tracker";
    
    private string _dataFilePath;
    private Dictionary<DateTime, TimeSpan> _screenTimeData;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Screen Time Tracker module is running.");
        
        _dataFilePath = Path.Combine(dataFolder, "screentime.json");
        LoadScreenTimeData();
        
        DateTime startTime = DateTime.Now;
        Console.WriteLine("Tracking started at: " + startTime.ToString());
        
        Console.WriteLine("Press any key to stop tracking...");
        Console.ReadKey();
        
        DateTime endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;
        
        UpdateScreenTimeData(startTime.Date, duration);
        SaveScreenTimeData();
        
        Console.WriteLine("Tracking stopped at: " + endTime.ToString());
        Console.WriteLine("Total screen time for " + startTime.Date.ToString("d") + ": " + duration.ToString());
        
        return true;
    }
    
    private void LoadScreenTimeData()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _screenTimeData = JsonSerializer.Deserialize<Dictionary<DateTime, TimeSpan>>(jsonData);
            }
            else
            {
                _screenTimeData = new Dictionary<DateTime, TimeSpan>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading screen time data: " + ex.Message);
            _screenTimeData = new Dictionary<DateTime, TimeSpan>();
        }
    }
    
    private void SaveScreenTimeData()
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(_screenTimeData);
            File.WriteAllText(_dataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving screen time data: " + ex.Message);
        }
    }
    
    private void UpdateScreenTimeData(DateTime date, TimeSpan duration)
    {
        if (_screenTimeData.ContainsKey(date))
        {
            _screenTimeData[date] += duration;
        }
        else
        {
            _screenTimeData[date] = duration;
        }
    }
}