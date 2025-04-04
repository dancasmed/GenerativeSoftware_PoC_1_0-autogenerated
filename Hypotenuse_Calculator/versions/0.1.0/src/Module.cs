using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HypotenuseCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Hypotenuse Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Hypotenuse Calculator module is running.");
        
        try
        {
            double sideA = 0;
            double sideB = 0;
            
            Console.WriteLine("Enter the length of side A:");
            string inputA = Console.ReadLine();
            
            Console.WriteLine("Enter the length of side B:");
            string inputB = Console.ReadLine();
            
            if (!double.TryParse(inputA, out sideA) || !double.TryParse(inputB, out sideB))
            {
                Console.WriteLine("Invalid input. Please enter valid numbers.");
                return false;
            }
            
            if (sideA <= 0 || sideB <= 0)
            {
                Console.WriteLine("Sides must be greater than zero.");
                return false;
            }
            
            double hypotenuse = Math.Sqrt(Math.Pow(sideA, 2) + Math.Pow(sideB, 2));
            Console.WriteLine("The hypotenuse is: " + hypotenuse);
            
            string resultPath = Path.Combine(dataFolder, "hypotenuse_result.json");
            var result = new 
            {
                SideA = sideA,
                SideB = sideB,
                Hypotenuse = hypotenuse,
                CalculationDate = DateTime.Now
            };
            
            string jsonResult = JsonSerializer.Serialize(result);
            File.WriteAllText(resultPath, jsonResult);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}