using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class WebsiteTrafficAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Website Traffic Analyzer";

    private string _dataFilePath;
    private List<WebsiteVisit> _visits;

    public WebsiteTrafficAnalyzer()
    {
        _visits = new List<WebsiteVisit>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Website Traffic Analyzer module is running...");
        _dataFilePath = Path.Combine(dataFolder, "website_traffic_data.json");

        try
        {
            LoadData();
            SimulateTraffic();
            AnalyzeTraffic();
            SaveData();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            var jsonData = File.ReadAllText(_dataFilePath);
            _visits = JsonSerializer.Deserialize<List<WebsiteVisit>>(jsonData) ?? new List<WebsiteVisit>();
            Console.WriteLine("Loaded " + _visits.Count + " existing visit records.");
        }
        else
        {
            Console.WriteLine("No existing data found. Starting with empty dataset.");
        }
    }

    private void SimulateTraffic()
    {
        var random = new Random();
        int newVisits = random.Next(1, 10);
        
        for (int i = 0; i < newVisits; i++)
        {
            _visits.Add(new WebsiteVisit
            {
                Timestamp = DateTime.Now.AddMinutes(-random.Next(0, 1440)),
                PageUrl = "/page" + random.Next(1, 10),
                Referrer = "https://search.example.com",
                IpAddress = "192.168." + random.Next(1, 255) + "." + random.Next(1, 255),
                UserAgent = "Mozilla/5.0 (compatible;)"
            });
        }
        
        Console.WriteLine("Simulated " + newVisits + " new website visits.");
    }

    private void AnalyzeTraffic()
    {
        if (_visits.Count == 0)
        {
            Console.WriteLine("No traffic data to analyze.");
            return;
        }

        var today = DateTime.Today;
        var dailyVisits = _visits.Count(v => v.Timestamp.Date == today);
        var uniqueVisitors = _visits.Select(v => v.IpAddress).Distinct().Count();
        var popularPages = _visits
            .GroupBy(v => v.PageUrl)
            .OrderByDescending(g => g.Count())
            .Take(3);

        Console.WriteLine("Traffic Analysis Results:");
        Console.WriteLine("Total visits: " + _visits.Count);
        Console.WriteLine("Today's visits: " + dailyVisits);
        Console.WriteLine("Unique visitors: " + uniqueVisitors);
        Console.WriteLine("Most popular pages:");
        
        foreach (var page in popularPages)
        {
            Console.WriteLine(page.Key + ": " + page.Count() + " visits");
        }
    }

    private void SaveData()
    {
        var jsonData = JsonSerializer.Serialize(_visits);
        File.WriteAllText(_dataFilePath, jsonData);
        Console.WriteLine("Data saved to " + _dataFilePath);
    }

    private class WebsiteVisit
    {
        public DateTime Timestamp { get; set; }
        public string PageUrl { get; set; }
        public string Referrer { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}