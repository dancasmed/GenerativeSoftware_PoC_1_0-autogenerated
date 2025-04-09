using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class FreelancerTaxCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Freelancer Tax Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Freelancer Tax Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "tax_config.json");
            string inputPath = Path.Combine(dataFolder, "income_data.json");
            string outputPath = Path.Combine(dataFolder, "tax_estimates.json");

            if (!File.Exists(configPath) || !File.Exists(inputPath))
            {
                Console.WriteLine("Required data files not found in the data folder.");
                return false;
            }

            TaxConfig config = JsonSerializer.Deserialize<TaxConfig>(File.ReadAllText(configPath));
            IncomeData incomeData = JsonSerializer.Deserialize<IncomeData>(File.ReadAllText(inputPath));

            if (config == null || incomeData == null)
            {
                Console.WriteLine("Failed to read configuration or income data.");
                return false;
            }

            TaxEstimate estimate = CalculateTax(incomeData, config);

            string jsonOutput = JsonSerializer.Serialize(estimate, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputPath, jsonOutput);

            Console.WriteLine("Tax estimation completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private TaxEstimate CalculateTax(IncomeData incomeData, TaxConfig config)
    {
        decimal taxableIncome = incomeData.GrossIncome - incomeData.Expenses;
        decimal incomeTax = taxableIncome * config.IncomeTaxRate;
        decimal selfEmploymentTax = taxableIncome * config.SelfEmploymentTaxRate;
        decimal totalTax = incomeTax + selfEmploymentTax;
        decimal quarterlyPayment = totalTax / 4;

        return new TaxEstimate
        {
            GrossIncome = incomeData.GrossIncome,
            Expenses = incomeData.Expenses,
            TaxableIncome = taxableIncome,
            IncomeTax = incomeTax,
            SelfEmploymentTax = selfEmploymentTax,
            TotalEstimatedTax = totalTax,
            QuarterlyEstimatedPayment = quarterlyPayment
        };
    }
}

public class TaxConfig
{
    public decimal IncomeTaxRate { get; set; }
    public decimal SelfEmploymentTaxRate { get; set; }
}

public class IncomeData
{
    public decimal GrossIncome { get; set; }
    public decimal Expenses { get; set; }
}

public class TaxEstimate
{
    public decimal GrossIncome { get; set; }
    public decimal Expenses { get; set; }
    public decimal TaxableIncome { get; set; }
    public decimal IncomeTax { get; set; }
    public decimal SelfEmploymentTax { get; set; }
    public decimal TotalEstimatedTax { get; set; }
    public decimal QuarterlyEstimatedPayment { get; set; }
}