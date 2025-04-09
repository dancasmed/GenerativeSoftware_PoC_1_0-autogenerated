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
            string configPath = Path.Combine(dataFolder, "compound_interest_config.json");
            string resultPath = Path.Combine(dataFolder, "compound_interest_results.json");

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please edit it and run again.");
                return false;
            }

            var config = LoadConfig(configPath);
            if (config == null)
            {
                Console.WriteLine("Error loading configuration file.");
                return false;
            }

            var results = CalculateCompoundInterest(config);
            SaveResults(results, resultPath);

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

    private CompoundInterestConfig LoadConfig(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<CompoundInterestConfig>(json);
        }
        catch
        {
            return null;
        }
    }

    private void CreateDefaultConfig(string filePath)
    {
        var defaultConfig = new CompoundInterestConfig
        {
            Principal = 1000,
            AnnualRate = 5.0,
            Years = 10,
            CompoundsPerYear = 12
        };

        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private CompoundInterestResult[] CalculateCompoundInterest(CompoundInterestConfig config)
    {
        var results = new CompoundInterestResult[config.Years];
        double ratePerPeriod = config.AnnualRate / 100 / config.CompoundsPerYear;
        int totalPeriods = config.Years * config.CompoundsPerYear;
        double currentAmount = config.Principal;

        for (int year = 1; year <= config.Years; year++)
        {
            for (int period = 1; period <= config.CompoundsPerYear; period++)
            {
                currentAmount *= (1 + ratePerPeriod);
            }

            results[year - 1] = new CompoundInterestResult
            {
                Year = year,
                Amount = Math.Round(currentAmount, 2)
            };
        }

        return results;
    }

    private void SaveResults(CompoundInterestResult[] results, string filePath)
    {
        string json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

public class CompoundInterestConfig
{
    public double Principal { get; set; }
    public double AnnualRate { get; set; }
    public int Years { get; set; }
    public int CompoundsPerYear { get; set; }
}

public class CompoundInterestResult
{
    public int Year { get; set; }
    public double Amount { get; set; }
}