using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomeSecurityCostCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Security Cost Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Home Security Cost Calculator Module is running...");

        try
        {
            string configPath = Path.Combine(dataFolder, "security_config.json");
            string resultPath = Path.Combine(dataFolder, "security_cost_result.json");

            if (!File.Exists(configPath))
            {
                var defaultConfig = new SecurityConfig
                {
                    BaseSystemCost = 500.00m,
                    InstallationCost = 200.00m,
                    MonthlyMonitoringCost = 30.00m,
                    ContractMonths = 24
                };

                string defaultConfigJson = JsonSerializer.Serialize(defaultConfig);
                File.WriteAllText(configPath, defaultConfigJson);
                Console.WriteLine("Default configuration file created.");
            }

            string configJson = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<SecurityConfig>(configJson);

            decimal totalCost = config.BaseSystemCost + config.InstallationCost + 
                              (config.MonthlyMonitoringCost * config.ContractMonths);

            var result = new SecurityCostResult
            {
                BaseCost = config.BaseSystemCost,
                InstallationCost = config.InstallationCost,
                TotalMonitoringCost = config.MonthlyMonitoringCost * config.ContractMonths,
                TotalCost = totalCost,
                CalculationDate = DateTime.Now
            };

            string resultJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(resultPath, resultJson);

            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Base System Cost: " + config.BaseSystemCost.ToString("C"));
            Console.WriteLine("Installation Cost: " + config.InstallationCost.ToString("C"));
            Console.WriteLine("Monitoring Cost (" + config.ContractMonths + " months): " + 
                            (config.MonthlyMonitoringCost * config.ContractMonths).ToString("C"));
            Console.WriteLine("Total Cost: " + totalCost.ToString("C"));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private class SecurityConfig
    {
        public decimal BaseSystemCost { get; set; }
        public decimal InstallationCost { get; set; }
        public decimal MonthlyMonitoringCost { get; set; }
        public int ContractMonths { get; set; }
    }

    private class SecurityCostResult
    {
        public decimal BaseCost { get; set; }
        public decimal InstallationCost { get; set; }
        public decimal TotalMonitoringCost { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CalculationDate { get; set; }
    }
}