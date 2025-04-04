using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class BMICalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "BMI Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("BMI Calculator Module is running.");
        
        try
        {
            double weight = 0;
            double height = 0;
            
            Console.WriteLine("Enter your weight in kilograms:");
            string weightInput = Console.ReadLine();
            
            Console.WriteLine("Enter your height in meters:");
            string heightInput = Console.ReadLine();
            
            if (!double.TryParse(weightInput, out weight) || !double.TryParse(heightInput, out height))
            {
                Console.WriteLine("Invalid input. Please enter numeric values.");
                return false;
            }
            
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
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
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
                Timestamp = DateTime.Now,
                Weight = weight,
                Height = height,
                BMI = bmi,
                Category = category
            };
            
            string fileName = Path.Combine(dataFolder, "bmi_results.json");
            string jsonString = JsonSerializer.Serialize(result);
            
            File.AppendAllText(fileName, jsonString + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to save results: " + ex.Message);
        }
    }
}