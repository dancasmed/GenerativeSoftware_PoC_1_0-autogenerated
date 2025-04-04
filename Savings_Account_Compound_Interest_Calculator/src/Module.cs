using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class SavingsAccountCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Savings Account Compound Interest Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Savings Account Compound Interest Calculator...");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "savings_config.json");
            string resultPath = Path.Combine(dataFolder, "savings_result.json");

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration created. Please edit the file and run again.");
                return false;
            }

            var config = LoadConfig(configPath);
            var result = CalculateCompoundInterest(config);
            
            SaveResult(resultPath, result);
            
            Console.WriteLine("Calculation completed successfully!");
            Console.WriteLine("Principal: " + config.Principal.ToString("C"));
            Console.WriteLine("Interest Rate: " + config.AnnualInterestRate.ToString("P2"));
            Console.WriteLine("Years: " + config.Years);
            Console.WriteLine("Compounding Frequency: " + config.CompoundingFrequencyPerYear + " times per year");
            Console.WriteLine("Final Amount: " + result.FinalAmount.ToString("C"));
            Console.WriteLine("Total Interest Earned: " + result.TotalInterest.ToString("C"));
            
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
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    private void CreateDefaultConfig(string path)
    {
        var defaultConfig = new SavingsConfig
        {
            Principal = 1000.00m,
            AnnualInterestRate = 0.05m,
            Years = 10,
            CompoundingFrequencyPerYear = 12
        };

        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    private SavingsResult CalculateCompoundInterest(SavingsConfig config)
    {
        decimal ratePerPeriod = config.AnnualInterestRate / config.CompoundingFrequencyPerYear;
        int totalPeriods = config.Years * config.CompoundingFrequencyPerYear;
        
        decimal finalAmount = config.Principal * (decimal)Math.Pow(
            (double)(1 + ratePerPeriod), 
            totalPeriods);
            
        decimal totalInterest = finalAmount - config.Principal;
        
        return new SavingsResult
        {
            FinalAmount = decimal.Round(finalAmount, 2),
            TotalInterest = decimal.Round(totalInterest, 2)
        };
    }
}

public class SavingsConfig
{
    public decimal Principal { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int Years { get; set; }
    public int CompoundingFrequencyPerYear { get; set; }
}

public class SavingsResult
{
    public decimal FinalAmount { get; set; }
    public decimal TotalInterest { get; set; }
}