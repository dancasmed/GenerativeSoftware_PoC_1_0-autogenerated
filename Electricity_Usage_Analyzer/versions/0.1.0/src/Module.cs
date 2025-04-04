using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ElectricityUsageAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Electricity Usage Analyzer";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Electricity Usage Analyzer module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "electricity_usage.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        if (!File.Exists(_dataFilePath))
        {
            InitializeDefaultData();
        }
        
        var usageData = LoadUsageData();
        
        DisplayUsageStatistics(usageData);
        
        return true;
    }
    
    private void InitializeDefaultData()
    {
        var defaultData = new List<MonthlyUsage>
        {
            new MonthlyUsage { Month = "January", UsageKWh = 450, Cost = 90.00m },
            new MonthlyUsage { Month = "February", UsageKWh = 420, Cost = 84.00m },
            new MonthlyUsage { Month = "March", UsageKWh = 400, Cost = 80.00m },
            new MonthlyUsage { Month = "April", UsageKWh = 380, Cost = 76.00m },
            new MonthlyUsage { Month = "May", UsageKWh = 350, Cost = 70.00m },
            new MonthlyUsage { Month = "June", UsageKWh = 320, Cost = 64.00m },
            new MonthlyUsage { Month = "July", UsageKWh = 300, Cost = 60.00m },
            new MonthlyUsage { Month = "August", UsageKWh = 310, Cost = 62.00m },
            new MonthlyUsage { Month = "September", UsageKWh = 330, Cost = 66.00m },
            new MonthlyUsage { Month = "October", UsageKWh = 360, Cost = 72.00m },
            new MonthlyUsage { Month = "November", UsageKWh = 390, Cost = 78.00m },
            new MonthlyUsage { Month = "December", UsageKWh = 430, Cost = 86.00m }
        };
        
        SaveUsageData(defaultData);
    }
    
    private List<MonthlyUsage> LoadUsageData()
    {
        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<MonthlyUsage>>(json);
    }
    
    private void SaveUsageData(List<MonthlyUsage> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void DisplayUsageStatistics(List<MonthlyUsage> usageData)
    {
        Console.WriteLine("\nMonthly Electricity Usage Analysis:");
        Console.WriteLine("---------------------------------");
        
        decimal totalCost = 0;
        int totalUsage = 0;
        
        foreach (var month in usageData)
        {
            Console.WriteLine(string.Format("{0,-10}: {1,5} kWh | ${2,6}", month.Month, month.UsageKWh, month.Cost));
            totalCost += month.Cost;
            totalUsage += month.UsageKWh;
        }
        
        Console.WriteLine("\nSummary:");
        Console.WriteLine(string.Format("Total Usage: {0} kWh", totalUsage));
        Console.WriteLine(string.Format("Total Cost: ${0}", totalCost));
        Console.WriteLine(string.Format("Average Monthly Usage: {0} kWh", totalUsage / usageData.Count));
        Console.WriteLine(string.Format("Average Monthly Cost: ${0}", totalCost / usageData.Count));
        
        var highestMonth = FindHighestUsageMonth(usageData);
        Console.WriteLine(string.Format("Highest Usage Month: {0} ({1} kWh)", highestMonth.Month, highestMonth.UsageKWh));
        
        var lowestMonth = FindLowestUsageMonth(usageData);
        Console.WriteLine(string.Format("Lowest Usage Month: {0} ({1} kWh)", lowestMonth.Month, lowestMonth.UsageKWh));
    }
    
    private MonthlyUsage FindHighestUsageMonth(List<MonthlyUsage> data)
    {
        MonthlyUsage highest = data[0];
        foreach (var month in data)
        {
            if (month.UsageKWh > highest.UsageKWh)
            {
                highest = month;
            }
        }
        return highest;
    }
    
    private MonthlyUsage FindLowestUsageMonth(List<MonthlyUsage> data)
    {
        MonthlyUsage lowest = data[0];
        foreach (var month in data)
        {
            if (month.UsageKWh < lowest.UsageKWh)
            {
                lowest = month;
            }
        }
        return lowest;
    }
}

public class MonthlyUsage
{
    public string Month { get; set; }
    public int UsageKWh { get; set; }
    public decimal Cost { get; set; }
}