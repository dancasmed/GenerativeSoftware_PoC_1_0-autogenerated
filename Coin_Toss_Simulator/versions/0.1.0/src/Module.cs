using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class CoinTossModule : IGeneratedModule
{
    public string Name { get; set; } = "Coin Toss Simulator";
    
    private Random _random;
    
    public CoinTossModule()
    {
        _random = new Random();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Coin Toss Simulator is running...");
        Console.WriteLine("This module simulates flipping a virtual coin multiple times.");
        
        string resultsFilePath = Path.Combine(dataFolder, "coin_toss_results.json");
        
        Console.Write("Enter the number of coin flips: ");
        string input = Console.ReadLine();
        
        if (!int.TryParse(input, out int numberOfFlips) || numberOfFlips <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a positive integer.");
            return false;
        }
        
        var results = new CoinTossResults
        {
            TotalFlips = numberOfFlips,
            HeadsCount = 0,
            TailsCount = 0,
            Flips = new string[numberOfFlips]
        };
        
        for (int i = 0; i < numberOfFlips; i++)
        {
            bool isHeads = _random.Next(2) == 0;
            
            if (isHeads)
            {
                results.HeadsCount++;
                results.Flips[i] = "Heads";
                Console.WriteLine("Flip " + (i + 1) + ": Heads");
            }
            else
            {
                results.TailsCount++;
                results.Flips[i] = "Tails";
                Console.WriteLine("Flip " + (i + 1) + ": Tails");
            }
        }
        
        Console.WriteLine("\nSummary:");
        Console.WriteLine("Total flips: " + results.TotalFlips);
        Console.WriteLine("Heads: " + results.HeadsCount);
        Console.WriteLine("Tails: " + results.TailsCount);
        Console.WriteLine("Heads percentage: " + (results.HeadsCount * 100.0 / results.TotalFlips).ToString("F2") + "%");
        Console.WriteLine("Tails percentage: " + (results.TailsCount * 100.0 / results.TotalFlips).ToString("F2") + "%");
        
        try
        {
            string json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(resultsFilePath, json);
            Console.WriteLine("\nResults saved to: " + resultsFilePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving results: " + ex.Message);
            return false;
        }
    }
    
    private class CoinTossResults
    {
        public int TotalFlips { get; set; }
        public int HeadsCount { get; set; }
        public int TailsCount { get; set; }
        public string[] Flips { get; set; }
    }
}