using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ElectricityUsageAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Electricity Usage Analyzer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Electricity Usage Analyzer module is running.");
        
        try
        {
            string usageDataPath = Path.Combine(dataFolder, "electricity_usage.json");
            List<MonthlyUsage> monthlyUsages;
            
            if (File.Exists(usageDataPath))
            {
                string jsonData = File.ReadAllText(usageDataPath);
                monthlyUsages = JsonSerializer.Deserialize<List<MonthlyUsage>>(jsonData);
                Console.WriteLine("Loaded existing electricity usage data.");
            }
            else
            {
                monthlyUsages = new List<MonthlyUsage>();
                Console.WriteLine("No existing data found. Starting with empty dataset.");
            }
            
            bool continueRunning = true;
            while (continueRunning)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Add new monthly usage");
                Console.WriteLine("2. View usage statistics");
                Console.WriteLine("3. Save and exit");
                Console.Write("Enter your choice: ");
                
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }
                
                switch (choice)
                {
                    case 1:
                        AddMonthlyUsage(monthlyUsages);
                        break;
                    case 2:
                        ShowStatistics(monthlyUsages);
                        break;
                    case 3:
                        SaveData(monthlyUsages, usageDataPath);
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void AddMonthlyUsage(List<MonthlyUsage> usages)
    {
        Console.Write("Enter month (e.g., January): ");
        string month = Console.ReadLine();
        
        Console.Write("Enter year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year. Operation cancelled.");
            return;
        }
        
        Console.Write("Enter electricity usage in kWh: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal usage))
        {
            Console.WriteLine("Invalid usage value. Operation cancelled.");
            return;
        }
        
        Console.Write("Enter cost per kWh: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal costPerKwh))
        {
            Console.WriteLine("Invalid cost value. Operation cancelled.");
            return;
        }
        
        usages.Add(new MonthlyUsage
        {
            Month = month,
            Year = year,
            UsageKwh = usage,
            CostPerKwh = costPerKwh
        });
        
        Console.WriteLine("Monthly usage added successfully.");
    }
    
    private void ShowStatistics(List<MonthlyUsage> usages)
    {
        if (usages.Count == 0)
        {
            Console.WriteLine("No usage data available.");
            return;
        }
        
        Console.WriteLine("\nMonthly Electricity Usage Statistics:");
        Console.WriteLine("------------------------------------");
        
        decimal totalUsage = 0;
        decimal totalCost = 0;
        
        foreach (var usage in usages)
        {
            decimal monthlyCost = usage.UsageKwh * usage.CostPerKwh;
            Console.WriteLine(string.Format("{0} {1}: {2} kWh (Cost: {3:C})", 
                usage.Month, usage.Year, usage.UsageKwh, monthlyCost));
            
            totalUsage += usage.UsageKwh;
            totalCost += monthlyCost;
        }
        
        Console.WriteLine("\nSummary:");
        Console.WriteLine(string.Format("Total Usage: {0} kWh", totalUsage));
        Console.WriteLine(string.Format("Average Monthly Usage: {0} kWh", totalUsage / usages.Count));
        Console.WriteLine(string.Format("Total Cost: {0:C}", totalCost));
        Console.WriteLine(string.Format("Average Monthly Cost: {0:C}", totalCost / usages.Count));
    }
    
    private void SaveData(List<MonthlyUsage> usages, string filePath)
    {
        string jsonData = JsonSerializer.Serialize(usages);
        File.WriteAllText(filePath, jsonData);
        Console.WriteLine("Data saved successfully.");
    }
}

public class MonthlyUsage
{
    public string Month { get; set; }
    public int Year { get; set; }
    public decimal UsageKwh { get; set; }
    public decimal CostPerKwh { get; set; }
}