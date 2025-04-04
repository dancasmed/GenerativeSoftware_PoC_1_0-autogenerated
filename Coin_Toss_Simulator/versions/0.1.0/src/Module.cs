using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class CoinTossModule : IGeneratedModule
{
    public string Name { get; set; } = "Coin Toss Simulator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Coin Toss Simulator...");
        
        try
        {
            string resultsFilePath = Path.Combine(dataFolder, "coin_toss_results.json");
            
            Console.WriteLine("How many times would you like to flip the coin?");
            string input = Console.ReadLine();
            
            if (!int.TryParse(input, out int numberOfFlips) || numberOfFlips <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive integer.");
                return false;
            }
            
            Random random = new Random();
            string[] results = new string[numberOfFlips];
            
            for (int i = 0; i < numberOfFlips; i++)
            {
                results[i] = random.Next(2) == 0 ? "Heads" : "Tails";
                Console.WriteLine("Flip " + (i + 1) + ": " + results[i]);
            }
            
            var resultData = new
            {
                Timestamp = DateTime.Now,
                TotalFlips = numberOfFlips,
                HeadsCount = results.Count(r => r == "Heads"),
                TailsCount = results.Count(r => r == "Tails"),
                Results = results
            };
            
            string jsonData = JsonSerializer.Serialize(resultData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(resultsFilePath, jsonData);
            
            Console.WriteLine("Results saved to " + resultsFilePath);
            Console.WriteLine("Coin toss simulation completed successfully.");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}