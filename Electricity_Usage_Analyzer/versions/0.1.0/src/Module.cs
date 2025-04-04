using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ElectricityUsageAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Electricity Usage Analyzer";

    private string _dataFilePath;
    private List<MonthlyUsage> _monthlyUsages;

    public ElectricityUsageAnalyzer()
    {
        _monthlyUsages = new List<MonthlyUsage>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Electricity Usage Analyzer module is running.");
        _dataFilePath = Path.Combine(dataFolder, "electricity_usage.json");

        try
        {
            LoadData();
            DisplayUsageSummary();
            AnalyzeUsageTrends();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            string jsonData = File.ReadAllText(_dataFilePath);
            _monthlyUsages = JsonSerializer.Deserialize<List<MonthlyUsage>>(jsonData);
            Console.WriteLine("Existing usage data loaded successfully.");
        }
        else
        {
            Console.WriteLine("No existing data found. Starting with empty dataset.");
        }
    }

    private void DisplayUsageSummary()
    {
        if (_monthlyUsages.Count == 0)
        {
            Console.WriteLine("No usage data available.");
            return;
        }

        Console.WriteLine("\nMonthly Electricity Usage Summary:");
        Console.WriteLine("--------------------------------");

        foreach (var usage in _monthlyUsages)
        {
            Console.WriteLine("Month: " + usage.Month + ", Year: " + usage.Year + ", Usage: " + usage.KilowattHours + " kWh");
        }
    }

    private void AnalyzeUsageTrends()
    {
        if (_monthlyUsages.Count < 2)
        {
            Console.WriteLine("\nNot enough data points for trend analysis.");
            return;
        }

        _monthlyUsages.Sort((a, b) => 
        {
            int yearCompare = a.Year.CompareTo(b.Year);
            return yearCompare != 0 ? yearCompare : a.Month.CompareTo(b.Month);
        });

        double totalUsage = 0;
        double highestUsage = double.MinValue;
        double lowestUsage = double.MaxValue;
        string highestMonth = "";
        string lowestMonth = "";

        foreach (var usage in _monthlyUsages)
        {
            totalUsage += usage.KilowattHours;

            if (usage.KilowattHours > highestUsage)
            {
                highestUsage = usage.KilowattHours;
                highestMonth = usage.Month + "/" + usage.Year;
            }

            if (usage.KilowattHours < lowestUsage)
            {
                lowestUsage = usage.KilowattHours;
                lowestMonth = usage.Month + "/" + usage.Year;
            }
        }

        double averageUsage = totalUsage / _monthlyUsages.Count;

        Console.WriteLine("\nUsage Analysis:");
        Console.WriteLine("--------------------------------");
        Console.WriteLine("Average monthly usage: " + averageUsage.ToString("0.00") + " kWh");
        Console.WriteLine("Highest usage month: " + highestMonth + " with " + highestUsage + " kWh");
        Console.WriteLine("Lowest usage month: " + lowestMonth + " with " + lowestUsage + " kWh");

        // Calculate month-to-month changes
        Console.WriteLine("\nMonth-to-Month Changes:");
        Console.WriteLine("--------------------------------");

        for (int i = 1; i < _monthlyUsages.Count; i++)
        {
            var prev = _monthlyUsages[i - 1];
            var current = _monthlyUsages[i];
            double change = current.KilowattHours - prev.KilowattHours;
            double percentChange = (change / prev.KilowattHours) * 100;

            Console.WriteLine(prev.Month + "/" + prev.Year + " to " + current.Month + "/" + current.Year + ": " + 
                             change.ToString("+0.00;-0.00") + " kWh (" + percentChange.ToString("+0.00;-0.00") + "%)");
        }
    }

    private class MonthlyUsage
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public double KilowattHours { get; set; }
    }
}