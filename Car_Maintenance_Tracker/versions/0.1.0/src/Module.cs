using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CarMaintenanceTracker : IGeneratedModule
{
    public string Name { get; set; } = "Car Maintenance Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Car Maintenance Tracker module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "maintenance_records.json");
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
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
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nCar Maintenance Tracker");
        Console.WriteLine("1. Add Maintenance Record");
        Console.WriteLine("2. View All Records");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddMaintenanceRecord()
    {
        Console.Write("Enter service description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter service date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime serviceDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }
        
        Console.Write("Enter service cost: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal cost))
        {
            Console.WriteLine("Invalid cost format.");
            return;
        }
        
        var records = LoadRecords();
        records.Add(new MaintenanceRecord
        {
            Description = description,
            Date = serviceDate,
            Cost = cost
        });
        
        SaveRecords(records);
        Console.WriteLine("Record added successfully.");
    }
    
    private void ViewAllRecords()
    {
        var records = LoadRecords();
        
        if (records.Count == 0)
        {
            Console.WriteLine("No maintenance records found.");
            return;
        }
        
        Console.WriteLine("\nMaintenance Records:");
        foreach (var record in records)
        {
            Console.WriteLine($"{record.Date:yyyy-MM-dd} - {record.Description} - Cost: {record.Cost:C}");
        }
    }
    
    private List<MaintenanceRecord> LoadRecords()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<MaintenanceRecord>();
        }
        
        string json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<MaintenanceRecord>>(json) ?? new List<MaintenanceRecord>();
    }
    
    private void SaveRecords(List<MaintenanceRecord> records)
    {
        string json = JsonSerializer.Serialize(records);
        File.WriteAllText(_dataFilePath, json);
    }
}

public class MaintenanceRecord
{
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal Cost { get; set; }
}