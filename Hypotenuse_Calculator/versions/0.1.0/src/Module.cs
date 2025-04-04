using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HypotenuseCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Hypotenuse Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Hypotenuse Calculator Module is running.");
        Console.WriteLine("This module calculates the hypotenuse of a right-angled triangle.");

        try
        {
            double sideA = GetValidDoubleInput("Enter the length of side A: ");
            double sideB = GetValidDoubleInput("Enter the length of side B: ");

            double hypotenuse = CalculateHypotenuse(sideA, sideB);
            Console.WriteLine("The hypotenuse is: " + hypotenuse.ToString("F2"));

            SaveResult(dataFolder, sideA, sideB, hypotenuse);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private double CalculateHypotenuse(double sideA, double sideB)
    {
        return Math.Sqrt(Math.Pow(sideA, 2) + Math.Pow(sideB, 2));
    }

    private double GetValidDoubleInput(string prompt)
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

    private void SaveResult(string dataFolder, double sideA, double sideB, double hypotenuse)
    {
        try
        {
            var result = new
            {
                SideA = sideA,
                SideB = sideB,
                Hypotenuse = hypotenuse,
                CalculationDate = DateTime.Now
            };

            string jsonResult = JsonSerializer.Serialize(result);
            string filePath = Path.Combine(dataFolder, "hypotenuse_results.json");
            File.AppendAllText(filePath, jsonResult + Environment.NewLine);
            Console.WriteLine("Result saved to " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to save result: " + ex.Message);
        }
    }
}