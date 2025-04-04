using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

public class PrimeNumberGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Prime Number Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Prime Number Generator module is running.");
        Console.WriteLine("Generating prime numbers up to 100...");
        
        int limit = 100;
        List<int> primes = GeneratePrimes(limit);
        
        string filePath = Path.Combine(dataFolder, "primes.json");
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(primes);
        File.WriteAllText(filePath, json);
        
        Console.WriteLine($"Generated {primes.Count} prime numbers and saved to {filePath}");
        return true;
    }
    
    private List<int> GeneratePrimes(int limit)
    {
        List<int> primes = new List<int>();
        
        for (int number = 2; number <= limit; number++)
        {
            bool isPrime = true;
            
            for (int divisor = 2; divisor <= Math.Sqrt(number); divisor++)
            {
                if (number % divisor == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            
            if (isPrime)
            {
                primes.Add(number);
            }
        }
        
        return primes;
    }
}