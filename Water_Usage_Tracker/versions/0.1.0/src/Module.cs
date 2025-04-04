using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WaterUsageTracker : IGeneratedModule
{
    public string Name { get; set; } = "Water Usage Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Water Usage Tracker module...");
        
        _dataFilePath = Path.Combine(dataFolder, "water_usage_data.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        WaterUsageData data = LoadData();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nWater Usage Tracker");
            Console.WriteLine("1. Record water usage");
            Console.WriteLine("2. Set monthly goal");
            Console.WriteLine("3. View current month usage");
            Console.WriteLine("4. View all records");
            Console.WriteLine("5. Exit module");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    RecordUsage(ref data);
                    break;
                case "2":
                    SetMonthlyGoal(ref data);
                    break;
                case "3":
                    DisplayCurrentMonthUsage(data);
                    break;
                case "4":
                    DisplayAllRecords(data);
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveData(data);
        }
        
        Console.WriteLine("Water Usage Tracker module finished.");
        return true;
    }
    
    private WaterUsageData LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<WaterUsageData>(json);
        }
        
        return new WaterUsageData
        {
            MonthlyRecords = new Dictionary<string, MonthlyRecord>(),
            CurrentMonthGoal = 0
        };
    }
    
    private void SaveData(WaterUsageData data)
    {
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void RecordUsage(ref WaterUsageData data)
    {
        string currentMonth = DateTime.Now.ToString("yyyy-MM");
        
        if (!data.MonthlyRecords.ContainsKey(currentMonth))
        {
            data.MonthlyRecords[currentMonth] = new MonthlyRecord
            {
                Month = currentMonth,
                Usage = 0,
                DaysRecorded = 0
            };
        }
        
        Console.Write("Enter today's water usage in liters: ");
        if (double.TryParse(Console.ReadLine(), out double usage))
        {
            data.MonthlyRecords[currentMonth].Usage += usage;
            data.MonthlyRecords[currentMonth].DaysRecorded++;
            Console.WriteLine("Usage recorded successfully.");
            
            if (data.CurrentMonthGoal > 0)
            {
                double percentage = (data.MonthlyRecords[currentMonth].Usage / data.CurrentMonthGoal) * 100;
                Console.WriteLine("Current month usage: " + data.MonthlyRecords[currentMonth].Usage + " liters (" + percentage.ToString("0.00") + "% of goal)");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Usage not recorded.");
        }
    }
    
    private void SetMonthlyGoal(ref WaterUsageData data)
    {
        Console.Write("Enter monthly water usage goal in liters: ");
        if (double.TryParse(Console.ReadLine(), out double goal) && goal > 0)
        {
            data.CurrentMonthGoal = goal;
            Console.WriteLine("Monthly goal set to " + goal + " liters.");
        }
        else
        {
            Console.WriteLine("Invalid input. Goal must be a positive number.");
        }
    }
    
    private void DisplayCurrentMonthUsage(WaterUsageData data)
    {
        string currentMonth = DateTime.Now.ToString("yyyy-MM");
        
        if (data.MonthlyRecords.TryGetValue(currentMonth, out MonthlyRecord record))
        {
            Console.WriteLine("\nCurrent Month (" + currentMonth + ")");
            Console.WriteLine("Total usage: " + record.Usage + " liters");
            Console.WriteLine("Days recorded: " + record.DaysRecorded);
            
            if (data.CurrentMonthGoal > 0)
            {
                double percentage = (record.Usage / data.CurrentMonthGoal) * 100;
                Console.WriteLine("Goal progress: " + percentage.ToString("0.00") + "%");
                
                if (percentage > 100)
                {
                    Console.WriteLine("Warning: You have exceeded your monthly goal by " + (percentage - 100).ToString("0.00") + "%");
                }
            }
            else
            {
                Console.WriteLine("No monthly goal set.");
            }
        }
        else
        {
            Console.WriteLine("No usage recorded for this month yet.");
        }
    }
    
    private void DisplayAllRecords(WaterUsageData data)
    {
        Console.WriteLine("\nAll Water Usage Records:");
        
        if (data.MonthlyRecords.Count == 0)
        {
            Console.WriteLine("No records available.");
            return;
        }
        
        foreach (var record in data.MonthlyRecords.Values)
        {
            Console.WriteLine("\nMonth: " + record.Month);
            Console.WriteLine("Total usage: " + record.Usage + " liters");
            Console.WriteLine("Days recorded: " + record.DaysRecorded);
            
            if (data.CurrentMonthGoal > 0)
            {
                double percentage = (record.Usage / data.CurrentMonthGoal) * 100;
                Console.WriteLine("Percentage of goal: " + percentage.ToString("0.00") + "%");
            }
        }
    }
}

public class WaterUsageData
{
    public Dictionary<string, MonthlyRecord> MonthlyRecords { get; set; }
    public double CurrentMonthGoal { get; set; }
}

public class MonthlyRecord
{
    public string Month { get; set; }
    public double Usage { get; set; }
    public int DaysRecorded { get; set; }
}