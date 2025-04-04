using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DiceRollerModule : IGeneratedModule
{
    public string Name { get; set; } = "Dice Roller Module";
    
    private Dictionary<string, int> diceTypes = new Dictionary<string, int>
    {
        { "D4", 4 },
        { "D6", 6 },
        { "D8", 8 },
        { "D10", 10 },
        { "D12", 12 },
        { "D20", 20 },
        { "D100", 100 }
    };
    
    private string statsFilePath;
    
    public bool Main(string dataFolder)
    {
        statsFilePath = Path.Combine(dataFolder, "dice_stats.json");
        
        Console.WriteLine("Dice Roller Module started");
        Console.WriteLine("Available dice types: D4, D6, D8, D10, D12, D20, D100");
        
        bool continueRolling = true;
        while (continueRolling)
        {
            Console.Write("Enter dice type (or 'quit' to exit): ");
            string input = Console.ReadLine()?.Trim().ToUpper();
            
            if (input == "QUIT")
            {
                continueRolling = false;
                continue;
            }
            
            if (!diceTypes.ContainsKey(input))
            {
                Console.WriteLine("Invalid dice type. Please try again.");
                continue;
            }
            
            int maxValue = diceTypes[input];
            int result = RollDice(maxValue);
            
            Console.WriteLine("Rolled " + input + ": " + result);
            SaveRollStatistics(input, result);
        }
        
        DisplayStatistics();
        Console.WriteLine("Dice Roller Module finished");
        return true;
    }
    
    private int RollDice(int sides)
    {
        Random random = new Random();
        return random.Next(1, sides + 1);
    }
    
    private void SaveRollStatistics(string diceType, int result)
    {
        Dictionary<string, List<int>> stats = new Dictionary<string, List<int>>();
        
        if (File.Exists(statsFilePath))
        {
            string json = File.ReadAllText(statsFilePath);
            stats = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(json);
        }
        
        if (!stats.ContainsKey(diceType))
        {
            stats[diceType] = new List<int>();
        }
        
        stats[diceType].Add(result);
        
        string updatedJson = JsonSerializer.Serialize(stats);
        File.WriteAllText(statsFilePath, updatedJson);
    }
    
    private void DisplayStatistics()
    {
        if (!File.Exists(statsFilePath))
        {
            Console.WriteLine("No roll statistics available.");
            return;
        }
        
        string json = File.ReadAllText(statsFilePath);
        var stats = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(json);
        
        Console.WriteLine("\nRoll Statistics:");
        foreach (var kvp in stats)
        {
            Console.WriteLine("Dice: " + kvp.Key);
            Console.WriteLine("  Rolls: " + kvp.Value.Count);
            Console.WriteLine("  Average: " + CalculateAverage(kvp.Value));
            Console.WriteLine("  Highest: " + FindMax(kvp.Value));
            Console.WriteLine("  Lowest: " + FindMin(kvp.Value));
        }
    }
    
    private double CalculateAverage(List<int> values)
    {
        if (values.Count == 0) return 0;
        return values.Average();
    }
    
    private int FindMax(List<int> values)
    {
        if (values.Count == 0) return 0;
        return values.Max();
    }
    
    private int FindMin(List<int> values)
    {
        if (values.Count == 0) return 0;
        return values.Min();
    }
}