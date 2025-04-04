using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CarMaintenanceModule : IGeneratedModule
{
    public string Name { get; set; } = "Car Maintenance Logger";

    private string _dataFilePath;
    private List<MaintenanceRecord> _records;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Car Maintenance Module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "maintenance_records.json");
        
        try
        {
            LoadRecords();
            DisplayMenu();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void LoadRecords()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _records = JsonSerializer.Deserialize<List<MaintenanceRecord>>(json);
        }
        else
        {
            _records = new List<MaintenanceRecord>();
        }
    }

    private void SaveRecords()
    {
        string json = JsonSerializer.Serialize(_records);
        File.WriteAllText(_dataFilePath, json);
    }

    private void DisplayMenu()
    {
        bool exit = false;
        
        while (!exit)
        {
            Console.WriteLine("\nCar Maintenance Menu");
            Console.WriteLine("1. Add Maintenance Record");
            Console.WriteLine("2. View All Records");
            Console.WriteLine("3. View Upcoming Services");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddMaintenanceRecord();
                    break;
                case "2":
                    ViewAllRecords();
                    break;
                case "3":
                    ViewUpcomingServices();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void AddMaintenanceRecord()
    {
        Console.Write("Enter vehicle make: ");
        string make = Console.ReadLine();
        
        Console.Write("Enter vehicle model: ");
        string model = Console.ReadLine();
        
        Console.Write("Enter maintenance type (e.g., Oil Change, Tire Rotation): ");
        string maintenanceType = Console.ReadLine();
        
        Console.Write("Enter maintenance date (yyyy-MM-dd): ");
        DateTime maintenanceDate;
        while (!DateTime.TryParse(Console.ReadLine(), out maintenanceDate))
        {
            Console.Write("Invalid date format. Please enter date (yyyy-MM-dd): ");
        }
        
        Console.Write("Enter next service due date (yyyy-MM-dd): ");
        DateTime nextServiceDate;
        while (!DateTime.TryParse(Console.ReadLine(), out nextServiceDate))
        {
            Console.Write("Invalid date format. Please enter date (yyyy-MM-dd): ");
        }
        
        Console.Write("Enter service cost: ");
        decimal cost;
        while (!decimal.TryParse(Console.ReadLine(), out cost))
        {
            Console.Write("Invalid cost format. Please enter a number: ");
        }
        
        var record = new MaintenanceRecord
        {
            Make = make,
            Model = model,
            MaintenanceType = maintenanceType,
            MaintenanceDate = maintenanceDate,
            NextServiceDate = nextServiceDate,
            Cost = cost
        };
        
        _records.Add(record);
        SaveRecords();
        
        Console.WriteLine("Maintenance record added successfully.");
    }

    private void ViewAllRecords()
    {
        if (_records.Count == 0)
        {
            Console.WriteLine("No maintenance records found.");
            return;
        }
        
        Console.WriteLine("\nAll Maintenance Records:");
        foreach (var record in _records)
        {
            Console.WriteLine($"{record.Make} {record.Model} - {record.MaintenanceType}");
            Console.WriteLine($"Last Service: {record.MaintenanceDate.ToShortDateString()}");
            Console.WriteLine($"Next Service: {record.NextServiceDate.ToShortDateString()}");
            Console.WriteLine($"Cost: {record.Cost:C}");
            Console.WriteLine();
        }
    }

    private void ViewUpcomingServices()
    {
        DateTime today = DateTime.Today;
        var upcoming = _records.FindAll(r => r.NextServiceDate >= today && r.NextServiceDate <= today.AddDays(30));
        
        if (upcoming.Count == 0)
        {
            Console.WriteLine("No upcoming services in the next 30 days.");
            return;
        }
        
        Console.WriteLine("\nUpcoming Services (next 30 days):");
        foreach (var record in upcoming)
        {
            Console.WriteLine($"{record.Make} {record.Model} - {record.MaintenanceType}");
            Console.WriteLine($"Due: {record.NextServiceDate.ToShortDateString()}");
            Console.WriteLine();
        }
    }
}

public class MaintenanceRecord
{
    public string Make { get; set; }
    public string Model { get; set; }
    public string MaintenanceType { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public DateTime NextServiceDate { get; set; }
    public decimal Cost { get; set; }
}