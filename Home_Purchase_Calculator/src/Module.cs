using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomePurchaseCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Purchase Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Home Purchase Calculator Module is running.");
        Console.WriteLine("This module calculates the total cost of a home purchase with down payment and mortgage.");

        try
        {
            double homePrice = GetInputDouble("Enter the home price: ");
            double downPaymentPercentage = GetInputDouble("Enter the down payment percentage (e.g., 20 for 20%): ");
            double annualInterestRate = GetInputDouble("Enter the annual interest rate (e.g., 3.5 for 3.5%): ");
            int loanTermYears = GetInputInt("Enter the loan term in years: ");

            double downPayment = homePrice * (downPaymentPercentage / 100);
            double loanAmount = homePrice - downPayment;
            double monthlyInterestRate = annualInterestRate / 100 / 12;
            int numberOfPayments = loanTermYears * 12;

            double monthlyPayment = loanAmount * 
                (monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, numberOfPayments)) / 
                (Math.Pow(1 + monthlyInterestRate, numberOfPayments) - 1);

            double totalPayment = monthlyPayment * numberOfPayments;
            double totalCost = downPayment + totalPayment;

            var result = new
            {
                HomePrice = homePrice,
                DownPaymentPercentage = downPaymentPercentage,
                DownPaymentAmount = downPayment,
                LoanAmount = loanAmount,
                AnnualInterestRate = annualInterestRate,
                LoanTermYears = loanTermYears,
                MonthlyPayment = monthlyPayment,
                TotalPayment = totalPayment,
                TotalCost = totalCost
            };

            string jsonResult = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(dataFolder, "HomePurchaseResult.json");
            File.WriteAllText(filePath, jsonResult);

            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Results saved to: " + filePath);
            Console.WriteLine("Total cost of home purchase: " + totalCost.ToString("C"));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private double GetInputDouble(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        while (!double.TryParse(input, out double result) || result <= 0)
        {
            Console.Write("Invalid input. Please enter a positive number: ");
            input = Console.ReadLine();
        }
        return double.Parse(input);
    }

    private int GetInputInt(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        while (!int.TryParse(input, out int result) || result <= 0)
        {
            Console.Write("Invalid input. Please enter a positive integer: ");
            input = Console.ReadLine();
        }
        return int.Parse(input);
    }
}