using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class CompoundInterestCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Compound Interest Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Compound Interest Calculator...");

        try
        {
            string configPath = Path.Combine(dataFolder, "interest_config.json");
            string resultPath = Path.Combine(dataFolder, "interest_results.json");

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Created default configuration file. Please edit it and run again.");
                return false;
            }

            var config = LoadConfiguration(configPath);
            var result = CalculateCompoundInterest(config);

            SaveResults(resultPath, result);
            Console.WriteLine("Calculation completed successfully. Results saved.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            return false;
        }
    }

    private InterestConfig LoadConfiguration(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<InterestConfig>(json);
    }

    private InterestResult CalculateCompoundInterest(InterestConfig config)
    {
        double amount = config.Principal;
        int periods = config.Years * config.CompoundsPerYear;
        double rate = config.AnnualInterestRate / config.CompoundsPerYear;

        for (int i = 0; i < periods; i++)
        {
            amount *= (1 + rate);
        }

        return new InterestResult
        {
            Principal = config.Principal,
            FinalAmount = Math.Round(amount, 2),
            InterestEarned = Math.Round(amount - config.Principal, 2),
            CalculationDate = DateTime.Now
        };
    }

    private void SaveResults(string filePath, InterestResult result)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(result, options);
        File.WriteAllText(filePath, json);
    }

    private void CreateDefaultConfig(string filePath)
    {
        var defaultConfig = new InterestConfig
        {
            Principal = 1000,
            AnnualInterestRate = 0.05,
            Years = 5,
            CompoundsPerYear = 12
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(defaultConfig, options);
        File.WriteAllText(filePath, json);
    }
}

public class InterestConfig
{
    public double Principal { get; set; }
    public double AnnualInterestRate { get; set; }
    public int Years { get; set; }
    public int CompoundsPerYear { get; set; }
}

public class InterestResult
{
    public double Principal { get; set; }
    public double FinalAmount { get; set; }
    public double InterestEarned { get; set; }
    public DateTime CalculationDate { get; set; }
}