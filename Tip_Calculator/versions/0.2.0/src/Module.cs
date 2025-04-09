using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TipCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Tip Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Tip Calculator Module is running...");
        Console.WriteLine("This tool helps you calculate the tip amount for a restaurant bill.");

        try
        {
            double billAmount = GetValidInput("Enter the total bill amount: ");
            double tipPercentage = GetValidInput("Enter the tip percentage you want to leave (e.g., 15 for 15%): ");

            double tipAmount = CalculateTip(billAmount, tipPercentage);
            double totalAmount = billAmount + tipAmount;

            Console.WriteLine("\n--- Calculation Results ---");
            Console.WriteLine("Bill Amount: {0:C}", billAmount);
            Console.WriteLine("Tip Percentage: {0}%", tipPercentage);
            Console.WriteLine("Tip Amount: {0:C}", tipAmount);
            Console.WriteLine("Total Amount: {0:C}", totalAmount);

            SaveCalculation(dataFolder, billAmount, tipPercentage, tipAmount, totalAmount);
            Console.WriteLine("Calculation saved successfully.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private double GetValidInput(string prompt)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (double.TryParse(input, out value) && value > 0)
            {
                return value;
            }
            Console.WriteLine("Invalid input. Please enter a positive number.");
        }
    }

    private double CalculateTip(double billAmount, double tipPercentage)
    {
        return billAmount * (tipPercentage / 100);
    }

    private void SaveCalculation(string dataFolder, double billAmount, double tipPercentage, double tipAmount, double totalAmount)
    {
        try
        {
            var calculation = new
            {
                BillAmount = billAmount,
                TipPercentage = tipPercentage,
                TipAmount = tipAmount,
                TotalAmount = totalAmount,
                Timestamp = DateTime.Now
            };

            string fileName = Path.Combine(dataFolder, "tip_calculations.json");
            string jsonString = JsonSerializer.Serialize(calculation);

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            File.AppendAllText(fileName, jsonString + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Warning: Could not save calculation. " + ex.Message);
        }
    }
}