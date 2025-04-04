using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class PriceCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Price Calculator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Price Calculator Module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "config.json");
            string resultPath = Path.Combine(dataFolder, "result.json");
            
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Configuration file not found. Creating default configuration.");
                var defaultConfig = new PriceConfig { CostPrice = 100, MarkupPercentage = 20 };
                File.WriteAllText(configPath, JsonSerializer.Serialize(defaultConfig));
            }
            
            string json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<PriceConfig>(json);
            
            if (config == null)
            {
                Console.WriteLine("Invalid configuration file.");
                return false;
            }
            
            double sellingPrice = CalculateSellingPrice(config.CostPrice, config.MarkupPercentage);
            
            var result = new CalculationResult
            {
                CostPrice = config.CostPrice,
                MarkupPercentage = config.MarkupPercentage,
                SellingPrice = sellingPrice,
                CalculationDate = DateTime.Now
            };
            
            File.WriteAllText(resultPath, JsonSerializer.Serialize(result));
            Console.WriteLine("Calculation completed successfully. Results saved.");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private double CalculateSellingPrice(double costPrice, double markupPercentage)
    {
        return costPrice * (1 + markupPercentage / 100);
    }
}

public class PriceConfig
{
    public double CostPrice { get; set; }
    public double MarkupPercentage { get; set; }
}

public class CalculationResult
{
    public double CostPrice { get; set; }
    public double MarkupPercentage { get; set; }
    public double SellingPrice { get; set; }
    public DateTime CalculationDate { get; set; }
}