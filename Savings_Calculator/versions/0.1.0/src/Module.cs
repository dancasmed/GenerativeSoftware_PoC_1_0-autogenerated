using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class SavingsCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Savings Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Savings Calculator module is running.");
        
        string configPath = Path.Combine(dataFolder, "savings_config.json");
        string resultPath = Path.Combine(dataFolder, "savings_result.json");
        
        try
        {
            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please edit it and run again.");
                return false;
            }
            
            var config = LoadConfig(configPath);
            var result = CalculateCompoundInterest(config);
            
            SaveResult(resultPath, result);
            
            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Results saved to: " + resultPath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private SavingsConfig LoadConfig(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SavingsConfig>(json);
    }
    
    private void SaveResult(string path, SavingsResult result)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(result, options);
        File.WriteAllText(path, json);
    }
    
    private void CreateDefaultConfig(string path)
    {
        var defaultConfig = new SavingsConfig
        {
            Principal = 1000,
            AnnualRate = 5.0,
            Years = 10,
            CompoundsPerYear = 12
        };
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(defaultConfig, options);
        File.WriteAllText(path, json);
    }
    
    private SavingsResult CalculateCompoundInterest(SavingsConfig config)
    {
        double rate = config.AnnualRate / 100;
        double amount = config.Principal * Math.Pow(1 + (rate / config.CompoundsPerYear), 
                      config.CompoundsPerYear * config.Years);
        double interest = amount - config.Principal;
        
        return new SavingsResult
        {
            InitialAmount = config.Principal,
            FinalAmount = amount,
            InterestEarned = interest,
            CalculationDate = DateTime.Now
        };
    }
}

public class SavingsConfig
{
    public double Principal { get; set; }
    public double AnnualRate { get; set; }
    public int Years { get; set; }
    public int CompoundsPerYear { get; set; }
}

public class SavingsResult
{
    public double InitialAmount { get; set; }
    public double FinalAmount { get; set; }
    public double InterestEarned { get; set; }
    public DateTime CalculationDate { get; set; }
}