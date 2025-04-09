using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class RenovationCostCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Renovation Cost Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Renovation Cost Calculator...");

        try
        {
            string configPath = Path.Combine(dataFolder, "renovation_config.json");
            RenovationConfig config = LoadConfig(configPath);

            if (config == null)
            {
                config = GetUserInput();
                SaveConfig(configPath, config);
            }

            double totalCost = CalculateTotalCost(config);
            Console.WriteLine("Total renovation cost: " + totalCost.ToString("C2"));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private RenovationConfig LoadConfig(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<RenovationConfig>(json);
    }

    private void SaveConfig(string filePath, RenovationConfig config)
    {
        string json = JsonSerializer.Serialize(config);
        File.WriteAllText(filePath, json);
    }

    private RenovationConfig GetUserInput()
    {
        Console.WriteLine("Enter renovation details:");

        Console.Write("Material cost: ");
        double materialCost = double.Parse(Console.ReadLine());

        Console.Write("Labor cost: ");
        double laborCost = double.Parse(Console.ReadLine());

        Console.Write("Permit cost: ");
        double permitCost = double.Parse(Console.ReadLine());

        Console.Write("Number of inspections: ");
        int inspectionCount = int.Parse(Console.ReadLine());

        Console.Write("Cost per inspection: ");
        double inspectionCost = double.Parse(Console.ReadLine());

        return new RenovationConfig
        {
            MaterialCost = materialCost,
            LaborCost = laborCost,
            PermitCost = permitCost,
            InspectionCount = inspectionCount,
            InspectionCost = inspectionCost
        };
    }

    private double CalculateTotalCost(RenovationConfig config)
    {
        return config.MaterialCost 
            + config.LaborCost 
            + config.PermitCost 
            + (config.InspectionCount * config.InspectionCost);
    }

    private class RenovationConfig
    {
        public double MaterialCost { get; set; }
        public double LaborCost { get; set; }
        public double PermitCost { get; set; }
        public int InspectionCount { get; set; }
        public double InspectionCost { get; set; }
    }
}