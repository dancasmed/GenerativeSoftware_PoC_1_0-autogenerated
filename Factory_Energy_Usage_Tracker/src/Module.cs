using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class EnergyUsageTracker : IGeneratedModule
{
    public string Name { get; set; } = "Factory Energy Usage Tracker";

    private string _dataFilePath;
    private List<EnergyRecord> _energyRecords;

    public EnergyUsageTracker()
    {
        _energyRecords = new List<EnergyRecord>();
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "energy_usage_data.json");
        
        Console.WriteLine("Initializing Factory Energy Usage Tracker...");
        
        LoadData();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddEnergyRecord();
                    break;
                case "2":
                    ViewEnergyRecords();
                    break;
                case "3":
                    AnalyzeEnergyUsage();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveData();
        Console.WriteLine("Energy usage data saved. Exiting module.");
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nFactory Energy Usage Tracker");
        Console.WriteLine("1. Add Energy Record");
        Console.WriteLine("2. View Energy Records");
        Console.WriteLine("3. Analyze Energy Usage");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddEnergyRecord()
    {
        Console.WriteLine("\nAdd New Energy Record");
        
        Console.Write("Machine ID: ");
        string machineId = Console.ReadLine();
        
        Console.Write("Energy Consumption (kWh): ");
        if (!double.TryParse(Console.ReadLine(), out double consumption))
        {
            Console.WriteLine("Invalid input for energy consumption.");
            return;
        }
        
        Console.Write("Timestamp (YYYY-MM-DD HH:MM): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime timestamp))
        {
            Console.WriteLine("Invalid input for timestamp.");
            return;
        }
        
        _energyRecords.Add(new EnergyRecord
        {
            MachineId = machineId,
            Consumption = consumption,
            Timestamp = timestamp
        });
        
        Console.WriteLine("Energy record added successfully.");
    }
    
    private void ViewEnergyRecords()
    {
        Console.WriteLine("\nEnergy Records:");
        
        if (_energyRecords.Count == 0)
        {
            Console.WriteLine("No records available.");
            return;
        }
        
        foreach (var record in _energyRecords)
        {
            Console.WriteLine($"Machine: {record.MachineId}, Consumption: {record.Consumption} kWh, Time: {record.Timestamp}");
        }
    }
    
    private void AnalyzeEnergyUsage()
    {
        Console.WriteLine("\nEnergy Usage Analysis:");
        
        if (_energyRecords.Count == 0)
        {
            Console.WriteLine("No records available for analysis.");
            return;
        }
        
        double totalConsumption = 0;
        var machineConsumption = new Dictionary<string, double>();
        
        foreach (var record in _energyRecords)
        {
            totalConsumption += record.Consumption;
            
            if (machineConsumption.ContainsKey(record.MachineId))
            {
                machineConsumption[record.MachineId] += record.Consumption;
            }
            else
            {
                machineConsumption[record.MachineId] = record.Consumption;
            }
        }
        
        Console.WriteLine($"Total Energy Consumption: {totalConsumption} kWh");
        Console.WriteLine("Consumption by Machine:");
        
        foreach (var kvp in machineConsumption)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value} kWh ({(kvp.Value / totalConsumption) * 100:F2}%)");
        }
    }
    
    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _energyRecords = JsonSerializer.Deserialize<List<EnergyRecord>>(json);
                Console.WriteLine("Energy usage data loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("No existing data file found. Starting with empty dataset.");
        }
    }
    
    private void SaveData()
    {
        try
        {
            string json = JsonSerializer.Serialize(_energyRecords);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }
}

public class EnergyRecord
{
    public string MachineId { get; set; }
    public double Consumption { get; set; }
    public DateTime Timestamp { get; set; }
}