using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GadgetRepairTracker : IGeneratedModule
{
    public string Name { get; set; } = "Gadget Repair Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Gadget Repair Tracker module is running...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        _dataFilePath = Path.Combine(dataFolder, "repairs.json");
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\n1. Add new repair record");
            Console.WriteLine("2. View all repair records");
            Console.WriteLine("3. Calculate total repair costs");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }
            
            switch (option)
            {
                case 1:
                    AddRepairRecord();
                    break;
                case 2:
                    ViewRepairRecords();
                    break;
                case 3:
                    CalculateTotalCosts();
                    break;
                case 4:
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Exiting Gadget Repair Tracker...");
        return true;
    }
    
    private void AddRepairRecord()
    {
        Console.Write("Enter gadget name: ");
        string gadgetName = Console.ReadLine();
        
        Console.Write("Enter repair description: ");
        string description = Console.ReadLine();
        
        Console.Write("Enter repair cost: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal cost))
        {
            Console.WriteLine("Invalid cost value. Record not saved.");
            return;
        }
        
        Console.Write("Enter repair date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime repairDate))
        {
            repairDate = DateTime.Now;
        }
        
        var repairRecord = new RepairRecord
        {
            GadgetName = gadgetName,
            Description = description,
            Cost = cost,
            RepairDate = repairDate
        };
        
        var records = LoadRepairRecords();
        records.Add(repairRecord);
        
        SaveRepairRecords(records);
        Console.WriteLine("Repair record added successfully.");
    }
    
    private void ViewRepairRecords()
    {
        var records = LoadRepairRecords();
        
        if (records.Count == 0)
        {
            Console.WriteLine("No repair records found.");
            return;
        }
        
        Console.WriteLine("\nRepair Records:");
        Console.WriteLine("------------------------------------------------");
        foreach (var record in records)
        {
            Console.WriteLine("Gadget: " + record.GadgetName);
            Console.WriteLine("Description: " + record.Description);
            Console.WriteLine("Cost: " + record.Cost.ToString("C"));
            Console.WriteLine("Date: " + record.RepairDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("------------------------------------------------");
        }
    }
    
    private void CalculateTotalCosts()
    {
        var records = LoadRepairRecords();
        decimal total = 0;
        
        foreach (var record in records)
        {
            total += record.Cost;
        }
        
        Console.WriteLine("\nTotal repair costs: " + total.ToString("C"));
    }
    
    private List<RepairRecord> LoadRepairRecords()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<RepairRecord>();
        }
        
        string json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<RepairRecord>>(json) ?? new List<RepairRecord>();
    }
    
    private void SaveRepairRecords(List<RepairRecord> records)
    {
        string json = JsonSerializer.Serialize(records);
        File.WriteAllText(_dataFilePath, json);
    }
}

public class RepairRecord
{
    public string GadgetName { get; set; }
    public string Description { get; set; }
    public decimal Cost { get; set; }
    public DateTime RepairDate { get; set; }
}