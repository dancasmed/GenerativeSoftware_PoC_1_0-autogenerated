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
            string inputFilePath = Path.Combine(dataFolder, "input.json");
            string outputFilePath = Path.Combine(dataFolder, "output.json");

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found. Creating a default input file.");
                CreateDefaultInputFile(inputFilePath);
                Console.WriteLine("Please edit the input.json file with your values and run the module again.");
                return false;
            }

            var input = ReadInputFile(inputFilePath);
            if (input == null)
            {
                Console.WriteLine("Failed to read input file.");
                return false;
            }


            double result = CalculateCompoundInterest(input.Principal, input.AnnualRate, input.Years, input.CompoundsPerYear);

            var output = new
            {
                Input = input,
                FutureValue = result,
                InterestEarned = result - input.Principal,
                CalculationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            WriteOutputFile(outputFilePath, output);
            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Results saved to: " + outputFilePath);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void CreateDefaultInputFile(string filePath)
    {
        var defaultInput = new
        {
            Principal = 1000.0,
            AnnualRate = 5.0,
            Years = 10,
            CompoundsPerYear = 12
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(defaultInput, options);
        File.WriteAllText(filePath, json);
    }

    private dynamic ReadInputFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<dynamic>(json);
    }

    private void WriteOutputFile(string filePath, object data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }

    private double CalculateCompoundInterest(double principal, double annualRate, int years, int compoundsPerYear)
    {
        double rate = annualRate / 100;
        double amount = principal * Math.Pow(1 + (rate / compoundsPerYear), compoundsPerYear * years);
        return Math.Round(amount, 2);
    }
}