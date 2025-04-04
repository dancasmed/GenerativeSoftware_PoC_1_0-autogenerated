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
        Console.WriteLine("Please enter your height in meters (e.g., 1.75):");
        string heightInput = Console.ReadLine();

        Console.WriteLine("Please enter your weight in kilograms (e.g., 70):");
        string weightInput = Console.ReadLine();

        if (double.TryParse(heightInput, out double height) && double.TryParse(weightInput, out double weight))
        {
            double bmi = CalculateBMI(height, weight);
            string category = GetBMICategory(bmi);

            Console.WriteLine("Your BMI is: " + bmi.ToString("F2"));
            Console.WriteLine("Category: " + category);

            SaveResults(dataFolder, height, weight, bmi, category);
            return true;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter numeric values for height and weight.");
            return false;
        }
    }

    private double CalculateBMI(double height, double weight)
    {
        return weight / (height * height);
    }

    private string GetBMICategory(double bmi)
    {
        if (bmi < 18.5)
            return "Underweight";
        else if (bmi >= 18.5 && bmi < 25)
            return "Normal weight";
        else if (bmi >= 25 && bmi < 30)
            return "Overweight";
        else
            return "Obese";
    }

    private void SaveResults(string dataFolder, double height, double weight, double bmi, string category)
    {
        try
        {
            var result = new
            {
                Height = height,
                Weight = weight,
                BMI = bmi,
                Category = category,
                Date = DateTime.Now
            };

            string json = JsonSerializer.Serialize(result);
            string filePath = Path.Combine(dataFolder, "bmi_results.json");
            File.AppendAllText(filePath, json + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving results: " + ex.Message);
        }
    }
}