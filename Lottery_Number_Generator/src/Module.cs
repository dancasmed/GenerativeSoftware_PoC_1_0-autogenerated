using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LotteryNumberGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Lottery Number Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Lottery Number Generator Module is running...");
        
        try
        {
            string settingsPath = Path.Combine(dataFolder, "lottery_settings.json");
            LotterySettings settings;
            
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                settings = JsonSerializer.Deserialize<LotterySettings>(json);
                Console.WriteLine("Loaded previous lottery settings.");
            }
            else
            {
                settings = new LotterySettings
                {
                    MinNumber = 1,
                    MaxNumber = 49,
                    NumberCount = 6
                };
                
                string json = JsonSerializer.Serialize(settings);
                File.WriteAllText(settingsPath, json);
                Console.WriteLine("Created new lottery settings file.");
            }
            
            Console.WriteLine("Generating lottery numbers...");
            List<int> numbers = GenerateNumbers(settings);
            
            Console.WriteLine("Your lottery numbers are:");
            foreach (int number in numbers)
            {
                Console.Write(number + " ");
            }
            Console.WriteLine();
            
            string resultsPath = Path.Combine(dataFolder, "lottery_results.json");
            LotteryResult result = new LotteryResult
            {
                Numbers = numbers,
                GeneratedDate = DateTime.Now
            };
            
            string resultJson = JsonSerializer.Serialize(result);
            File.WriteAllText(resultsPath, resultJson);
            Console.WriteLine("Results saved to file.");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private List<int> GenerateNumbers(LotterySettings settings)
    {
        Random random = new Random();
        HashSet<int> numbers = new HashSet<int>();
        
        while (numbers.Count < settings.NumberCount)
        {
            int num = random.Next(settings.MinNumber, settings.MaxNumber + 1);
            numbers.Add(num);
        }
        
        List<int> sortedNumbers = new List<int>(numbers);
        sortedNumbers.Sort();
        
        return sortedNumbers;
    }
}

public class LotterySettings
{
    public int MinNumber { get; set; }
    public int MaxNumber { get; set; }
    public int NumberCount { get; set; }
}

public class LotteryResult
{
    public List<int> Numbers { get; set; }
    public DateTime GeneratedDate { get; set; }
}