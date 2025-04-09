using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomeSecurityCostCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Security Cost Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Security Cost Calculator...");

        try
        {
            string configPath = Path.Combine(dataFolder, "security_config.json");
            string resultPath = Path.Combine(dataFolder, "cost_result.json");

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please edit it and run again.");
                return false;
            }

            var config = LoadConfiguration(configPath);
            var totalCost = CalculateTotalCost(config);
            SaveResult(resultPath, totalCost);

            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Total cost: " + totalCost.ToString("C"));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private SecurityConfig LoadConfiguration(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<SecurityConfig>(json);
    }

    private decimal CalculateTotalCost(SecurityConfig config)
    {
        decimal cameraCost = config.CameraCount * config.CameraUnitPrice;
        decimal sensorCost = config.SensorCount * config.SensorUnitPrice;
        decimal installationCost = config.InstallationFee;
        
        return cameraCost + sensorCost + installationCost;
    }

    private void SaveResult(string filePath, decimal totalCost)
    {
        var result = new { TotalCost = totalCost, CalculationDate = DateTime.Now };
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private void CreateDefaultConfig(string filePath)
    {
        var defaultConfig = new SecurityConfig
        {
            CameraCount = 4,
            CameraUnitPrice = 199.99m,
            SensorCount = 10,
            SensorUnitPrice = 49.99m,
            InstallationFee = 299.99m
        };

        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private class SecurityConfig
    {
        public int CameraCount { get; set; }
        public decimal CameraUnitPrice { get; set; }
        public int SensorCount { get; set; }
        public decimal SensorUnitPrice { get; set; }
        public decimal InstallationFee { get; set; }
    }
}