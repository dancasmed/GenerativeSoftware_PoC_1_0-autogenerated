using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HomeApplianceMaintenanceTracker : IGeneratedModule
{
    public string Name { get; set; } = "Home Appliance Maintenance Tracker";

    private string _appliancesFilePath;
    private List<Appliance> _appliances;

    public HomeApplianceMaintenanceTracker()
    {
        _appliances = new List<Appliance>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Home Appliance Maintenance Tracker module is running.");
        _appliancesFilePath = Path.Combine(dataFolder, "appliances.json");

        LoadAppliances();
        CheckMaintenanceReminders();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add new appliance");
            Console.WriteLine("2. View all appliances");
            Console.WriteLine("3. Update maintenance date");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddAppliance();
                    break;
                case "2":
                    ViewAppliances();
                    break;
                case "3":
                    UpdateMaintenanceDate();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveAppliances();
        return true;
    }

    private void LoadAppliances()
    {
        if (File.Exists(_appliancesFilePath))
        {
            string json = File.ReadAllText(_appliancesFilePath);
            _appliances = JsonSerializer.Deserialize<List<Appliance>>(json) ?? new List<Appliance>();
        }
    }

    private void SaveAppliances()
    {
        string json = JsonSerializer.Serialize(_appliances);
        File.WriteAllText(_appliancesFilePath, json);
    }

    private void CheckMaintenanceReminders()
    {
        DateTime today = DateTime.Today;
        bool hasReminders = false;

        foreach (var appliance in _appliances)
        {
            if (appliance.NextMaintenanceDate <= today)
            {
                Console.WriteLine($"Reminder: {appliance.Name} needs maintenance! Last maintained on: {appliance.LastMaintenanceDate.ToShortDateString()}");
                hasReminders = true;
            }
        }

        if (!hasReminders)
        {
            Console.WriteLine("No maintenance reminders at this time.");
        }
    }

    private void AddAppliance()
    {
        Console.Write("Enter appliance name: ");
        string name = Console.ReadLine();

        Console.Write("Enter maintenance interval in months: ");
        if (!int.TryParse(Console.ReadLine(), out int interval) || interval <= 0)
        {
            Console.WriteLine("Invalid interval. Please enter a positive number.");
            return;
        }

        var appliance = new Appliance
        {
            Name = name,
            MaintenanceIntervalMonths = interval,
            LastMaintenanceDate = DateTime.Today,
            NextMaintenanceDate = DateTime.Today.AddMonths(interval)
        };

        _appliances.Add(appliance);
        Console.WriteLine("Appliance added successfully.");
    }

    private void ViewAppliances()
    {
        if (_appliances.Count == 0)
        {
            Console.WriteLine("No appliances registered.");
            return;
        }

        Console.WriteLine("\nRegistered Appliances:");
        foreach (var appliance in _appliances)
        {
            Console.WriteLine($"Name: {appliance.Name}");
            Console.WriteLine($"Last Maintenance: {appliance.LastMaintenanceDate.ToShortDateString()}");
            Console.WriteLine($"Next Maintenance: {appliance.NextMaintenanceDate.ToShortDateString()}");
            Console.WriteLine($"Maintenance Interval: {appliance.MaintenanceIntervalMonths} months");
            Console.WriteLine();
        }
    }

    private void UpdateMaintenanceDate()
    {
        if (_appliances.Count == 0)
        {
            Console.WriteLine("No appliances registered.");
            return;
        }

        Console.WriteLine("Select an appliance to update:");
        for (int i = 0; i < _appliances.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_appliances[i].Name}");
        }

        if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 1 || selection > _appliances.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var appliance = _appliances[selection - 1];
        appliance.LastMaintenanceDate = DateTime.Today;
        appliance.NextMaintenanceDate = DateTime.Today.AddMonths(appliance.MaintenanceIntervalMonths);

        Console.WriteLine($"Maintenance date updated for {appliance.Name}. Next maintenance due on {appliance.NextMaintenanceDate.ToShortDateString()}.");
    }

    private class Appliance
    {
        public string Name { get; set; }
        public int MaintenanceIntervalMonths { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
    }
}