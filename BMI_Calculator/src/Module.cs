using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class BMICalculator : IGeneratedModule
{
    public string Name { get; set; } = "BMI Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("BMI Calculator Module is running.");
        Console.WriteLine("Please enter your weight in kilograms:");
        string weightInput = Console.ReadLine();

        Console.WriteLine("Please enter your height in meters:");
        string heightInput = Console.ReadLine();

        if (double.TryParse(weightInput, out double weight) && double.TryParse(heightInput, out double height))
        {
            if (weight <= 0 || height <= 0)
            {
                Console.WriteLine("Weight and height must be positive values.");
                return false;
            }

            double bmi = CalculateBMI(weight, height);
            string category = GetBMICategory(bmi);

            Console.WriteLine("Your BMI is: " + bmi.ToString("F2"));
            Console.WriteLine("Category: " + category);

            SaveResult(dataFolder, weight, height, bmi, category);
            return true;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter valid numbers.");
            return false;
        }
    }

    private double CalculateBMI(double weight, double height)
    {
        return weight / (height * height);
    }

    private string GetBMICategory(double bmi)
    {
        if (bmi < 18.5)
            return "Underweight";
        else if (bmi < 25)
            return "Normal weight";
        else if (bmi < 30)
            return "Overweight";
        else
            return "Obese";
    }

    private void SaveResult(string dataFolder, double weight, double height, double bmi, string category)
    {
        try
        {
            var result = new
            {
                Weight = weight,
                Height = height,
                BMI = bmi,
                Category = category,
                Date = DateTime.Now
            };

            string fileName = Path.Combine(dataFolder, "bmi_results.json");
            string jsonString = JsonSerializer.Serialize(result);

            File.AppendAllText(fileName, jsonString + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving results: " + ex.Message);
        }
    }
}