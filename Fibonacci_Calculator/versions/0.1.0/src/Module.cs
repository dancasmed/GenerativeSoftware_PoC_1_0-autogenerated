using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class FibonacciCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Fibonacci Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Fibonacci Calculator Module is running.");
        Console.WriteLine("This module calculates the nth Fibonacci number.");
        
        Console.Write("Enter the position (n) in the Fibonacci sequence: ");
        string input = Console.ReadLine();
        
        if (!int.TryParse(input, out int n) || n < 0)
        {
            Console.WriteLine("Invalid input. Please enter a non-negative integer.");
            return false;
        }
        
        long result = CalculateFibonacci(n);
        Console.WriteLine("The {0}th Fibonacci number is: {1}", n, result);
        
        string logFilePath = Path.Combine(dataFolder, "fibonacci_results.json");
        LogResult(logFilePath, n, result);
        
        return true;
    }
    
    private long CalculateFibonacci(int n)
    {
        if (n == 0) return 0;
        if (n == 1) return 1;
        
        long a = 0;
        long b = 1;
        long result = 0;
        
        for (int i = 2; i <= n; i++)
        {
            result = a + b;
            a = b;
            b = result;
        }
        
        return result;
    }
    
    private void LogResult(string filePath, int position, long result)
    {
        try
        {
            string logEntry = $"{{\"timestamp\": \"{DateTime.Now:o}\", \"position\": {position}, \"result\": {result}}}";
            
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, $"[\n{logEntry}\n]");
            }
            else
            {
                string existingContent = File.ReadAllText(filePath).TrimEnd(']');
                File.WriteAllText(filePath, $"{existingContent},\n{logEntry}\n]");
            }
            
            Console.WriteLine("Result logged to: " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error logging result: " + ex.Message);
        }
    }
}