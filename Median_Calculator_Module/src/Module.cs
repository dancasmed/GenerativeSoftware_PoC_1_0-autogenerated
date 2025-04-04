using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MedianCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Median Calculator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Median Calculator Module is running...");
        Console.WriteLine("Reading numbers from data folder...");

        string inputFilePath = Path.Combine(dataFolder, "numbers.json");
        string outputFilePath = Path.Combine(dataFolder, "median_result.json");

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Error: Input file 'numbers.json' not found in the data folder.");
            return false;
        }

        try
        {
            string jsonContent = File.ReadAllText(inputFilePath);
            List<double> numbers = System.Text.Json.JsonSerializer.Deserialize<List<double>>(jsonContent);

            if (numbers == null || numbers.Count == 0)
            {
                Console.WriteLine("Error: No numbers found in the input file.");
                return false;
            }

            double median = CalculateMedian(numbers);
            Console.WriteLine("Median calculated successfully.");

            var result = new { Median = median };
            string resultJson = System.Text.Json.JsonSerializer.Serialize(result);
            File.WriteAllText(outputFilePath, resultJson);

            Console.WriteLine("Result saved to 'median_result.json' in the data folder.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private double CalculateMedian(List<double> numbers)
    {
        numbers.Sort();
        int count = numbers.Count;
        int midIndex = count / 2;

        if (count % 2 == 0)
        {
            return (numbers[midIndex - 1] + numbers[midIndex]) / 2.0;
        }
        else
        {
            return numbers[midIndex];
        }
    }
}