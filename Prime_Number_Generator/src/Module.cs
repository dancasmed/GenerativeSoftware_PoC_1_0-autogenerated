using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PrimeNumberGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Prime Number Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Prime Number Generator module is running.");
        Console.WriteLine("Generating prime numbers between 1 and 100...");

        List<int> primes = GeneratePrimes(1, 100);
        
        Console.WriteLine("Prime numbers generated successfully.");
        Console.WriteLine("Saving prime numbers to JSON file...");
        
        string filePath = Path.Combine(dataFolder, "primes.json");
        SavePrimesToJson(primes, filePath);
        
        Console.WriteLine("Prime numbers saved to " + filePath);
        
        return true;
    }

    private List<int> GeneratePrimes(int start, int end)
    {
        List<int> primes = new List<int>();
        
        for (int i = start; i <= end; i++)
        {
            if (IsPrime(i))
            {
                primes.Add(i);
            }
        }
        
        return primes;
    }

    private bool IsPrime(int number)
    {
        if (number <= 1)
        {
            return false;
        }
        
        if (number == 2)
        {
            return true;
        }
        
        if (number % 2 == 0)
        {
            return false;
        }
        
        for (int i = 3; i <= Math.Sqrt(number); i += 2)
        {
            if (number % i == 0)
            {
                return false;
            }
        }
        
        return true;
    }

    private void SavePrimesToJson(List<int> primes, string filePath)
    {
        string json = JsonSerializer.Serialize(primes);
        File.WriteAllText(filePath, json);
    }
}