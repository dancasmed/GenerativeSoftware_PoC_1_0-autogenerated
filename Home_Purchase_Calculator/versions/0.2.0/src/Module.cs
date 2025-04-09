using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomePurchaseCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Purchase Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Home Purchase Calculator is running...");

        try
        {
            string inputFilePath = Path.Combine(dataFolder, "home_purchase_input.json");
            string outputFilePath = Path.Combine(dataFolder, "home_purchase_output.json");

            if (!File.Exists(inputFilePath))
            {
                CreateSampleInputFile(inputFilePath);
                Console.WriteLine("Sample input file created. Please fill in the details and run again.");
                return false;
            }

            var input = ReadInputFile(inputFilePath);
            var result = CalculateTotalCost(input);
            SaveResult(outputFilePath, result);

            Console.WriteLine("Calculation completed successfully. Results saved to: " + outputFilePath);
            Console.WriteLine("Purchase Price: " + result.PurchasePrice.ToString("C"));
            Console.WriteLine("Closing Costs: " + result.ClosingCosts.ToString("C"));
            Console.WriteLine("Taxes: " + result.Taxes.ToString("C"));
            Console.WriteLine("Total Cost: " + result.TotalCost.ToString("C"));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private HomePurchaseInput ReadInputFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<HomePurchaseInput>(json);
    }

    private void CreateSampleInputFile(string filePath)
    {
        var sampleInput = new HomePurchaseInput
        {
            PurchasePrice = 300000,
            ClosingCostPercentage = 2.5,
            TaxRate = 1.2
        };

        string json = JsonSerializer.Serialize(sampleInput, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private HomePurchaseResult CalculateTotalCost(HomePurchaseInput input)
    {
        decimal closingCosts = input.PurchasePrice * (decimal)(input.ClosingCostPercentage / 100);
        decimal taxes = input.PurchasePrice * (decimal)(input.TaxRate / 100);
        decimal totalCost = input.PurchasePrice + closingCosts + taxes;

        return new HomePurchaseResult
        {
            PurchasePrice = input.PurchasePrice,
            ClosingCosts = closingCosts,
            Taxes = taxes,
            TotalCost = totalCost
        };
    }

    private void SaveResult(string filePath, HomePurchaseResult result)
    {
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

public class HomePurchaseInput
{
    public decimal PurchasePrice { get; set; }
    public double ClosingCostPercentage { get; set; }
    public double TaxRate { get; set; }
}

public class HomePurchaseResult
{
    public decimal PurchasePrice { get; set; }
    public decimal ClosingCosts { get; set; }
    public decimal Taxes { get; set; }
    public decimal TotalCost { get; set; }
}