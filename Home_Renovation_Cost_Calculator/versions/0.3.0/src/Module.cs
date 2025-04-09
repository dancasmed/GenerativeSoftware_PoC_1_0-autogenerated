using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomeRenovationCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Renovation Cost Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Renovation Cost Calculator...");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string configPath = Path.Combine(dataFolder, "renovation_config.json");
            RenovationConfig config;

            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<RenovationConfig>(json);
                Console.WriteLine("Loaded existing configuration.");
            }
            else
            {
                config = new RenovationConfig();
                Console.WriteLine("Created new configuration.");
            }

            Console.WriteLine("Please enter the following details for your renovation project:");
            
            Console.Write("Labor cost: ");
            config.LaborCost = decimal.Parse(Console.ReadLine());
            
            Console.Write("Material cost: ");
            config.MaterialCost = decimal.Parse(Console.ReadLine());
            
            Console.Write("Permit cost: ");
            config.PermitCost = decimal.Parse(Console.ReadLine());
            
            Console.Write("Inspection cost: ");
            config.InspectionCost = decimal.Parse(Console.ReadLine());
            
            decimal totalCost = config.LaborCost + config.MaterialCost + config.PermitCost + config.InspectionCost;
            
            Console.WriteLine("Calculating total renovation cost...");
            Console.WriteLine("Total cost: " + totalCost.ToString("C"));
            
            string jsonOutput = JsonSerializer.Serialize(config);
            File.WriteAllText(configPath, jsonOutput);
            
            Console.WriteLine("Configuration saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}

public class RenovationConfig
{
    public decimal LaborCost { get; set; }
    public decimal MaterialCost { get; set; }
    public decimal PermitCost { get; set; }
    public decimal InspectionCost { get; set; }
}