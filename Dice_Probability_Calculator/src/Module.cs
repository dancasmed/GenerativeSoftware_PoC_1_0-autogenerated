using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

public class DiceProbabilityCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Dice Probability Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Dice Probability Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "dice_config.json");
            DiceConfig config;
            
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<DiceConfig>(json);
            }
            else
            {
                config = new DiceConfig { DiceCount = 2, DiceSides = 6, TargetSum = 7 };
                string defaultJson = JsonSerializer.Serialize(config);
                File.WriteAllText(configPath, defaultJson);
            }
            
            double probability = CalculateProbability(config.DiceCount, config.DiceSides, config.TargetSum);
            
            Console.WriteLine("Probability of rolling a sum of " + config.TargetSum + " with " + 
                             config.DiceCount + " dice (" + config.DiceSides + " sides each): " + 
                             probability.ToString("P2"));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private double CalculateProbability(int diceCount, int diceSides, int targetSum)
    {
        if (diceCount < 1 || diceSides < 1 || targetSum < diceCount || targetSum > diceCount * diceSides)
            return 0.0;
            
        int totalOutcomes = (int)Math.Pow(diceSides, diceCount);
        int favorableOutcomes = CountFavorableOutcomes(diceCount, diceSides, targetSum);
        
        return (double)favorableOutcomes / totalOutcomes;
    }
    
    private int CountFavorableOutcomes(int diceCount, int diceSides, int targetSum)
    {
        if (diceCount == 1)
        {
            return (targetSum >= 1 && targetSum <= diceSides) ? 1 : 0;
        }
        
        int count = 0;
        for (int i = 1; i <= diceSides; i++)
        {
            count += CountFavorableOutcomes(diceCount - 1, diceSides, targetSum - i);
        }
        
        return count;
    }
}

public class DiceConfig
{
    public int DiceCount { get; set; }
    public int DiceSides { get; set; }
    public int TargetSum { get; set; }
}