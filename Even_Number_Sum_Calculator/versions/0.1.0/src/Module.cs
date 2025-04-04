using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class EvenNumberSumCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Even Number Sum Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Even Number Sum Calculator is running...");
        Console.WriteLine("This module calculates the sum of even numbers up to a given number N.");

        Console.Write("Enter a positive integer N: ");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out int n) || n < 0)
        {
            Console.WriteLine("Invalid input. Please enter a positive integer.");
            return false;
        }

        int sum = 0;
        for (int i = 0; i <= n; i += 2)
        {
            sum += i;
        }

        Console.WriteLine("The sum of even numbers up to {0} is: {1}", n, sum);

        // Save the result to a JSON file in the data folder
        string resultFilePath = Path.Combine(dataFolder, "even_sum_result.json");
        string resultJson = $"{{\"N\": {n}, \"SumOfEvens\": {sum}}}";
        File.WriteAllText(resultFilePath, resultJson);

        Console.WriteLine("Result saved to: " + resultFilePath);
        return true;
    }
}