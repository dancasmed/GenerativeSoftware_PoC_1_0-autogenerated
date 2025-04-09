using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomeRenovationLoanCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Renovation Loan Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Renovation Loan Calculator...");

        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string filePath = Path.Combine(dataFolder, "loan_data.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("No existing loan data found. Creating new loan calculation.");
                CalculateAndSaveLoan(filePath);
            }
            else
            {
                Console.WriteLine("Existing loan data found. Would you like to load it? (Y/N)");
                var response = Console.ReadLine();
                if (response != null && response.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    LoadAndDisplayLoan(filePath);
                }
                else
                {
                    CalculateAndSaveLoan(filePath);
                }
            }

            Console.WriteLine("Loan calculation completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void CalculateAndSaveLoan(string filePath)
    {
        Console.WriteLine("Enter the principal amount of the loan:");
        double principal = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Enter the annual interest rate (as a percentage, e.g., 5 for 5%):");
        double annualRate = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Enter the loan term in years:");
        int termYears = Convert.ToInt32(Console.ReadLine());

        double monthlyRate = annualRate / 100 / 12;
        int numberOfPayments = termYears * 12;

        double monthlyPayment = principal * monthlyRate * Math.Pow(1 + monthlyRate, numberOfPayments) / (Math.Pow(1 + monthlyRate, numberOfPayments) - 1);
        double totalPayment = monthlyPayment * numberOfPayments;
        double totalInterest = totalPayment - principal;

        var loanData = new
        {
            Principal = principal,
            AnnualInterestRate = annualRate,
            TermYears = termYears,
            MonthlyPayment = monthlyPayment,
            TotalPayment = totalPayment,
            TotalInterest = totalInterest
        };

        string jsonData = JsonSerializer.Serialize(loanData);
        File.WriteAllText(filePath, jsonData);

        Console.WriteLine("Loan calculation saved successfully.");
        DisplayLoanDetails(loanData);
    }

    private void LoadAndDisplayLoan(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        var loanData = JsonSerializer.Deserialize<LoanData>(jsonData);

        if (loanData != null)
        {
            Console.WriteLine("Loan data loaded successfully.");
            DisplayLoanDetails(loanData);
        }
        else
        {
            Console.WriteLine("Failed to load loan data. Calculating new loan.");
            CalculateAndSaveLoan(filePath);
        }
    }

    private void DisplayLoanDetails(dynamic loanData)
    {
        Console.WriteLine("\nLoan Details:");
        Console.WriteLine("Principal: " + loanData.Principal.ToString("C"));
        Console.WriteLine("Annual Interest Rate: " + loanData.AnnualInterestRate + "%");
        Console.WriteLine("Term: " + loanData.TermYears + " years");
        Console.WriteLine("Monthly Payment: " + loanData.MonthlyPayment.ToString("C"));
        Console.WriteLine("Total Payment: " + loanData.TotalPayment.ToString("C"));
        Console.WriteLine("Total Interest: " + loanData.TotalInterest.ToString("C"));
    }

    private class LoanData
    {
        public double Principal { get; set; }
        public double AnnualInterestRate { get; set; }
        public int TermYears { get; set; }
        public double MonthlyPayment { get; set; }
        public double TotalPayment { get; set; }
        public double TotalInterest { get; set; }
    }
}