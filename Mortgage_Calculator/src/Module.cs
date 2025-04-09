using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class MortgageCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Mortgage Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Mortgage Calculator Module is running...");

        try
        {
            string inputPath = Path.Combine(dataFolder, "mortgage_input.json");
            string outputPath = Path.Combine(dataFolder, "mortgage_output.json");

            if (!File.Exists(inputPath))
            {
                CreateSampleInputFile(inputPath);
                Console.WriteLine("Sample input file created. Please fill in your mortgage details and run again.");
                return false;
            }

            MortgageInput input = LoadInput(inputPath);
            MortgageResult result = CalculateMortgage(input);
            SaveResult(outputPath, result);

            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Principal: " + input.Principal.ToString("C"));
            Console.WriteLine("Annual Interest Rate: " + input.AnnualInterestRate.ToString("P2"));
            Console.WriteLine("Term (years): " + input.TermInYears);
            Console.WriteLine("Monthly Payment: " + result.MonthlyPayment.ToString("C"));
            Console.WriteLine("Total Interest: " + result.TotalInterest.ToString("C"));
            Console.WriteLine("Total Payment: " + result.TotalPayment.ToString("C"));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void CreateSampleInputFile(string path)
    {
        var sampleInput = new MortgageInput
        {
            Principal = 200000,
            AnnualInterestRate = 0.035,
            TermInYears = 30
        };

        string json = JsonSerializer.Serialize(sampleInput, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    private MortgageInput LoadInput(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<MortgageInput>(json);
    }

    private MortgageResult CalculateMortgage(MortgageInput input)
    {
        double monthlyRate = input.AnnualInterestRate / 12;
        int numberOfPayments = input.TermInYears * 12;

        double monthlyPayment = input.Principal * 
            (monthlyRate * Math.Pow(1 + monthlyRate, numberOfPayments)) / 
            (Math.Pow(1 + monthlyRate, numberOfPayments) - 1);

        double totalPayment = monthlyPayment * numberOfPayments;
        double totalInterest = totalPayment - input.Principal;

        return new MortgageResult
        {
            MonthlyPayment = monthlyPayment,
            TotalInterest = totalInterest,
            TotalPayment = totalPayment
        };
    }

    private void SaveResult(string path, MortgageResult result)
    {
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}

public class MortgageInput
{
    public double Principal { get; set; }
    public double AnnualInterestRate { get; set; }
    public int TermInYears { get; set; }
}

public class MortgageResult
{
    public double MonthlyPayment { get; set; }
    public double TotalInterest { get; set; }
    public double TotalPayment { get; set; }
}