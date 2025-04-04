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
        Console.WriteLine("This module generates a list of prime numbers within a given range.");

        try
        {
            Console.Write("Enter the start of the range: ");
            int start = int.Parse(Console.ReadLine());

            Console.Write("Enter the end of the range: ");
            int end = int.Parse(Console.ReadLine());

            if (start > end)
            {
                Console.WriteLine("Start of the range must be less than or equal to the end.");
                return false;
            }

            List<int> primes = GeneratePrimes(start, end);

            string filePath = Path.Combine(dataFolder, "primes.json");
            File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(primes));

            Console.WriteLine("Prime numbers have been generated and saved to " + filePath);
            Console.WriteLine("Total primes found: " + primes.Count);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
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
}