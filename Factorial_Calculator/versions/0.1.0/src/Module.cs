using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class FactorialCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Factorial Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Factorial Calculator Module is running.");
        Console.WriteLine("Enter a number to calculate its factorial:");
        
        string input = Console.ReadLine();
        
        if (int.TryParse(input, out int number) && number >= 0)
        {
            long factorial = CalculateFactorial(number);
            Console.WriteLine("The factorial of " + number + " is " + factorial + ".");
            
            string resultFilePath = Path.Combine(dataFolder, "factorial_result.json");
            string jsonResult = $"{{\"number\": {number}, \"factorial\": {factorial}}}";
            File.WriteAllText(resultFilePath, jsonResult);
            
            return true;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a non-negative integer.");
            return false;
        }
    }
    
    private long CalculateFactorial(int n)
    {
        if (n == 0)
            return 1;
            
        long result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
}