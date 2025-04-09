using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ElectricityUsageTracker : IGeneratedModule
{
    public string Name { get; set; } = "Electricity Usage Tracker";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Electricity Usage Tracker module is running.");
        
        string usageDataPath = Path.Combine(dataFolder, "electricity_usage_data.json");
        
        List<ElectricityUsageRecord> usageRecords = LoadUsageRecords(usageDataPath);
        
        while (true)
        {
            Console.WriteLine("\nElectricity Usage Tracker Menu:");
            Console.WriteLine("1. Add new usage record");
            Console.WriteLine("2. View all usage records");
            Console.WriteLine("3. Analyze usage data");
            Console.WriteLine("4. Save and exit");
            Console.Write("Enter your choice: ");
            
            string input = Console.ReadLine();
            
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                continue;
            }
            
            switch (choice)
            {
                case 1:
                    AddUsageRecord(usageRecords);
                    break;
                case 2:
                    DisplayUsageRecords(usageRecords);
                    break;
                case 3:
                    AnalyzeUsageData(usageRecords);
                    break;
                case 4:
                    SaveUsageRecords(usageRecords, usageDataPath);
                    Console.WriteLine("Data saved successfully. Exiting module.");
                    return true;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
                    break;
            }
        }
    }
    
    private List<ElectricityUsageRecord> LoadUsageRecords(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<ElectricityUsageRecord>>(jsonData) ?? new List<ElectricityUsageRecord>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading usage records: " + ex.Message);
        }
        
        return new List<ElectricityUsageRecord>();
    }
    
    private void SaveUsageRecords(List<ElectricityUsageRecord> records, string filePath)
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving usage records: " + ex.Message);
        }
    }
    
    private void AddUsageRecord(List<ElectricityUsageRecord> records)
    {
        Console.Write("Enter date (YYYY-MM-DD): ");
        string dateInput = Console.ReadLine();
        
        Console.Write("Enter meter reading (kWh): ");
        string readingInput = Console.ReadLine();
        
        Console.Write("Enter building area (e.g., Floor 1, Room 101): ");
        string area = Console.ReadLine();
        
        if (!DateTime.TryParse(dateInput, out DateTime date))
        {
            Console.WriteLine("Invalid date format. Record not added.");
            return;
        }
        
        if (!double.TryParse(readingInput, out double reading) || reading < 0)
        {
            Console.WriteLine("Invalid meter reading. Record not added.");
            return;
        }
        
        records.Add(new ElectricityUsageRecord
        {
            Date = date,
            MeterReading = reading,
            Area = area
        });
        
        Console.WriteLine("Usage record added successfully.");
    }
    
    private void DisplayUsageRecords(List<ElectricityUsageRecord> records)
    {
        if (records.Count == 0)
        {
            Console.WriteLine("No usage records available.");
            return;
        }
        
        Console.WriteLine("\nElectricity Usage Records:");
        Console.WriteLine("Date\t\t\tArea\t\t\tReading (kWh)");
        
        foreach (var record in records)
        {
            Console.WriteLine(record.Date.ToString("yyyy-MM-dd") + "\t\t" + 
                            record.Area + "\t\t\t" + 
                            record.MeterReading.ToString("0.00"));
        }
    }
    
    private void AnalyzeUsageData(List<ElectricityUsageRecord> records)
    {
        if (records.Count == 0)
        {
            Console.WriteLine("No data available for analysis.");
            return;
        }
        
        double totalUsage = 0;
        double minUsage = double.MaxValue;
        double maxUsage = double.MinValue;
        var usageByArea = new Dictionary<string, double>();
        
        foreach (var record in records)
        {
            totalUsage += record.MeterReading;
            
            if (record.MeterReading < minUsage)
                minUsage = record.MeterReading;
                
            if (record.MeterReading > maxUsage)
                maxUsage = record.MeterReading;
                
            if (usageByArea.ContainsKey(record.Area))
                usageByArea[record.Area] += record.MeterReading;
            else
                usageByArea[record.Area] = record.MeterReading;
        }
        
        double averageUsage = totalUsage / records.Count;
        
        Console.WriteLine("\nElectricity Usage Analysis:");
        Console.WriteLine("Total usage: " + totalUsage.ToString("0.00") + " kWh");
        Console.WriteLine("Average daily usage: " + averageUsage.ToString("0.00") + " kWh");
        Console.WriteLine("Minimum daily usage: " + minUsage.ToString("0.00") + " kWh");
        Console.WriteLine("Maximum daily usage: " + maxUsage.ToString("0.00") + " kWh");
        
        Console.WriteLine("\nUsage by area:");
        foreach (var area in usageByArea)
        {
            Console.WriteLine(area.Key + ": " + area.Value.ToString("0.00") + " kWh");
        }
    }
}

public class ElectricityUsageRecord
{
    public DateTime Date { get; set; }
    public double MeterReading { get; set; }
    public string Area { get; set; } = string.Empty;
}