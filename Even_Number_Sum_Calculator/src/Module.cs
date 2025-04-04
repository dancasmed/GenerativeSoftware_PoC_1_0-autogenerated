using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class EvenNumberSumCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Even Number Sum Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Even Number Sum Calculator module is running.");
        Console.WriteLine("This module calculates the sum of even numbers up to a given number N.");

        int n = GetUserInput();
        if (n <= 0)
        {
            Console.WriteLine("Invalid input. N must be a positive integer.");
            return false;
        }

        int sum = CalculateEvenSum(n);
        Console.WriteLine("The sum of even numbers up to " + n + " is " + sum + ".");

        SaveResult(dataFolder, n, sum);
        return true;
    }

    private int GetUserInput()
    {
        Console.Write("Enter a positive integer N: ");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int n) && n > 0)
        {
            return n;
        }
        return -1;
    }

    private int CalculateEvenSum(int n)
    {
        int sum = 0;
        for (int i = 2; i <= n; i += 2)
        {
            sum += i;
        }
        return sum;
    }

    private void SaveResult(string dataFolder, int n, int sum)
    {
        try
        {
            string filePath = Path.Combine(dataFolder, "even_sum_results.json");
            string result = "{\"N\": " + n + ", \"Sum\": " + sum + "}";
            File.WriteAllText(filePath, result);
            Console.WriteLine("Result saved to " + filePath + ".");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving result: " + ex.Message);
        }
    }
}