using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class CompoundInterestCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Compound Interest Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Compound Interest Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "savings_config.json");
            string resultPath = Path.Combine(dataFolder, "savings_result.json");
            
            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please edit it and run the module again.");
                return false;
            }
            
            SavingsConfig config = LoadConfig(configPath);
            if (config == null)
            {
                Console.WriteLine("Failed to load configuration.");
                return false;
            }
            
            double result = CalculateCompoundInterest(
                config.Principal, 
                config.AnnualRate, 
                config.Years, 
                config.CompoundsPerYear);
                
            SavingsResult savingsResult = new()
            {
                Principal = config.Principal,
                FinalAmount = result,
                InterestEarned = result - config.Principal,
                CalculationDate = DateTime.Now
            };
            
            SaveResult(resultPath, savingsResult);
            
            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Principal: " + config.Principal.ToString("C"));
            Console.WriteLine("Final Amount: " + result.ToString("C"));
            Console.WriteLine("Interest Earned: " + (result - config.Principal).ToString("C"));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void CreateDefaultConfig(string path)
    {
        SavingsConfig defaultConfig = new()
        {
            Principal = 1000,
            AnnualRate = 0.05,
            Years = 10,
            CompoundsPerYear = 12
        };
        
        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
    
    private SavingsConfig LoadConfig(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SavingsConfig>(json);
    }
    
    private void SaveResult(string path, SavingsResult result)
    {
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
    
    private double CalculateCompoundInterest(double principal, double annualRate, int years, int compoundsPerYear)
    {
        return principal * Math.Pow(1 + (annualRate / compoundsPerYear), compoundsPerYear * years);
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
    public double Principal { get; set; }
    public double FinalAmount { get; set; }
    public double InterestEarned { get; set; }
    public DateTime CalculationDate { get; set; }
}