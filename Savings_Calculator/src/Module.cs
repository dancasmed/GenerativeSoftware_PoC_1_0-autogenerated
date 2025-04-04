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
                var defaultConfig = new SavingsConfig
                {
                    Principal = 1000,
                    AnnualRate = 5.0,
                    Years = 10,
                    CompoundsPerYear = 12
                };
                
                File.WriteAllText(configPath, JsonSerializer.Serialize(defaultConfig));
                Console.WriteLine("Default configuration created. Please edit the file and run again.");
                return false;
            }
            
            var config = JsonSerializer.Deserialize<SavingsConfig>(File.ReadAllText(configPath));
            
            if (config == null)
            {
                Console.WriteLine("Invalid configuration file.");
                return false;
            }
            
            double amount = CalculateCompoundInterest(
                config.Principal, 
                config.AnnualRate, 
                config.Years, 
                config.CompoundsPerYear);
                
            var result = new SavingsResult
            {
                FinalAmount = amount,
                InterestEarned = amount - config.Principal,
                CalculationDate = DateTime.Now
            };
            
            File.WriteAllText(resultPath, JsonSerializer.Serialize(result));
            
            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Principal: " + config.Principal.ToString("C"));
            Console.WriteLine("Annual Rate: " + config.AnnualRate + "%");
            Console.WriteLine("Years: " + config.Years);
            Console.WriteLine("Final Amount: " + amount.ToString("C"));
            Console.WriteLine("Interest Earned: " + (amount - config.Principal).ToString("C"));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private double CalculateCompoundInterest(double principal, double annualRate, int years, int compoundsPerYear)
    {
        double rate = annualRate / 100;
        double amount = principal * Math.Pow(1 + (rate / compoundsPerYear), compoundsPerYear * years);
        return Math.Round(amount, 2);
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
    public double FinalAmount { get; set; }
    public double InterestEarned { get; set; }
    public DateTime CalculationDate { get; set; }
}