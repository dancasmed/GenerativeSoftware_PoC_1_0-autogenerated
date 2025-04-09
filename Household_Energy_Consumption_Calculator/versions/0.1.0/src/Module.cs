using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class EnergyConsumptionCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Household Energy Consumption Calculator";

    private const string ApplianceDataFile = "appliances.json";
    private const string ConsumptionDataFile = "consumption.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Household Energy Consumption Calculator...");

        try
        {
            string applianceFilePath = Path.Combine(dataFolder, ApplianceDataFile);
            string consumptionFilePath = Path.Combine(dataFolder, ConsumptionDataFile);

            List<Appliance> appliances = LoadAppliances(applianceFilePath);
            if (appliances == null || appliances.Count == 0)
            {
                Console.WriteLine("No appliances found. Please add appliances first.");
                return false;
            }

            List<ConsumptionRecord> consumptionRecords = LoadConsumptionRecords(consumptionFilePath);

            double totalConsumption = CalculateTotalConsumption(appliances, consumptionRecords);

            Console.WriteLine("Total energy consumption for all appliances: " + totalConsumption + " kWh");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private List<Appliance> LoadAppliances(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<Appliance>();
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Appliance>>(json);
    }

    private List<ConsumptionRecord> LoadConsumptionRecords(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<ConsumptionRecord>();
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<ConsumptionRecord>>(json);
    }

    private double CalculateTotalConsumption(List<Appliance> appliances, List<ConsumptionRecord> consumptionRecords)
    {
        double totalConsumption = 0;

        foreach (var appliance in appliances)
        {
            var record = consumptionRecords.Find(c => c.ApplianceId == appliance.Id);
            if (record != null)
            {
                totalConsumption += appliance.PowerConsumption * record.HoursUsed;
            }
        }

        return totalConsumption;
    }
}

public class Appliance
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double PowerConsumption { get; set; } // in watts
}

public class ConsumptionRecord
{
    public string ApplianceId { get; set; }
    public double HoursUsed { get; set; }
}