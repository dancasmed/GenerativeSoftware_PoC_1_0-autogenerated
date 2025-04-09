using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class FactorialCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Factorial Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Factorial Calculator Module is running.");
        Console.WriteLine("Enter a number to calculate its factorial:");
        
        string input = Console.ReadLine();
        
        if (int.TryParse(input, out int number))
        {
            if (number < 0)
            {
                Console.WriteLine("Factorial is not defined for negative numbers.");
                return false;
            }
            
            long factorial = CalculateFactorial(number);
            Console.WriteLine("The factorial of " + number + " is " + factorial + ".");
            
            string resultPath = Path.Combine(dataFolder, "factorial_result.json");
            var result = new { Number = number, Factorial = factorial };
            string jsonResult = JsonSerializer.Serialize(result);
            File.WriteAllText(resultPath, jsonResult);
            
            return true;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid integer.");
            return false;
        }
    }
    
    private long CalculateFactorial(int n)
    {
        if (n == 0 || n == 1)
            return 1;
            
        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
}