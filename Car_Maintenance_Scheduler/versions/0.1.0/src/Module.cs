using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CarMaintenanceModule : IGeneratedModule
{
    public string Name { get; set; } = "Car Maintenance Scheduler";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "car_maintenance.json");
        
        Console.WriteLine("Car Maintenance Scheduler Module is running.");
        
        List<MaintenanceRecord> records = LoadRecords();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add new maintenance record");
            Console.WriteLine("2. View all maintenance records");
            Console.WriteLine("3. View upcoming services");
            Console.WriteLine("4. Exit module");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddMaintenanceRecord(records);
                    break;
                case "2":
                    DisplayAllRecords(records);
                    break;
                case "3":
                    DisplayUpcomingServices(records);
                    break;
                case "4":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveRecords(records);
        Console.WriteLine("Car Maintenance Scheduler Module is exiting.");
        return true;
    }
    
    private List<MaintenanceRecord> LoadRecords()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<MaintenanceRecord>>(json) ?? new List<MaintenanceRecord>();
        }
        return new List<MaintenanceRecord>();
    }
    
    private void SaveRecords(List<MaintenanceRecord> records)
    {
        string json = JsonSerializer.Serialize(records);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddMaintenanceRecord(List<MaintenanceRecord> records)
    {
        Console.Write("Enter car make/model: ");
        string carModel = Console.ReadLine();
        
        Console.Write("Enter maintenance description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter maintenance date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime maintenanceDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }
        
        Console.Write("Enter next service due date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime nextServiceDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }
        
        records.Add(new MaintenanceRecord
        {
            CarModel = carModel,
            Description = description,
            MaintenanceDate = maintenanceDate,
            NextServiceDate = nextServiceDate
        });
        
        Console.WriteLine("Maintenance record added successfully.");
    }
    
    private void DisplayAllRecords(List<MaintenanceRecord> records)
    {
        if (records.Count == 0)
        {
            Console.WriteLine("No maintenance records found.");
            return;
        }
        
        Console.WriteLine("\nAll Maintenance Records:");
        foreach (var record in records)
        {
            Console.WriteLine($"Car: {record.CarModel}");
            Console.WriteLine($"Description: {record.Description}");
            Console.WriteLine($"Maintenance Date: {record.MaintenanceDate:yyyy-MM-dd}");
            Console.WriteLine($"Next Service Due: {record.NextServiceDate:yyyy-MM-dd}");
            Console.WriteLine("-----");
        }
    }
    
    private void DisplayUpcomingServices(List<MaintenanceRecord> records)
    {
        var upcoming = records.FindAll(r => r.NextServiceDate >= DateTime.Today && r.NextServiceDate <= DateTime.Today.AddDays(30));
        
        if (upcoming.Count == 0)
        {
            Console.WriteLine("No upcoming services in the next 30 days.");
            return;
        }
        
        Console.WriteLine("\nUpcoming Services (next 30 days):");
        foreach (var record in upcoming)
        {
            Console.WriteLine($"Car: {record.CarModel}");
            Console.WriteLine($"Description: {record.Description}");
            Console.WriteLine($"Service Due: {record.NextServiceDate:yyyy-MM-dd}");
            Console.WriteLine($"Days remaining: {(record.NextServiceDate - DateTime.Today).Days}");
            Console.WriteLine("-----");
        }
    }
}

public class MaintenanceRecord
{
    public string CarModel { get; set; }
    public string Description { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public DateTime NextServiceDate { get; set; }
}