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
        Console.WriteLine("Dice Roller Module is running.");
        
        statsFilePath = Path.Combine(dataFolder, "dice_roll_stats.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        bool continueRolling = true;
        while (continueRolling)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    RollDice();
                    break;
                case "2":
                    ViewStatistics();
                    break;
                case "3":
                    continueRolling = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nDice Roller Menu:");
        Console.WriteLine("1. Roll Dice");
        Console.WriteLine("2. View Statistics");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
    }
    
    private void RollDice()
    {
        Console.WriteLine("\nAvailable Dice Types:");
        foreach (var dice in diceTypes)
        {
            Console.WriteLine(dice.Key);
        }
        
        Console.Write("Enter dice type (e.g., D6): ");
        string diceType = Console.ReadLine().ToUpper();
        
        if (!diceTypes.ContainsKey(diceType))
        {
            Console.WriteLine("Invalid dice type. Please try again.");
            return;
        }
        
        Console.Write("Enter number of dice to roll: ");
        if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
        {
            Console.WriteLine("Invalid number of dice. Please enter a positive integer.");
            return;
        }
        
        Random random = new Random();
        int maxValue = diceTypes[diceType];
        int total = 0;
        List<int> rolls = new List<int>();
        
        for (int i = 0; i < count; i++)
        {
            int roll = random.Next(1, maxValue + 1);
            rolls.Add(roll);
            total += roll;
        }
        
        Console.WriteLine("\nRoll Results:");
        Console.WriteLine("Individual Rolls: " + string.Join(", ", rolls));
        Console.WriteLine("Total: " + total);
        
        SaveRollStatistics(diceType, count, total);
    }
    
    private void SaveRollStatistics(string diceType, int count, int total)
    {
        Dictionary<string, List<RollStatistic>> stats;
        
        if (File.Exists(statsFilePath))
        {
            string json = File.ReadAllText(statsFilePath);
            stats = JsonSerializer.Deserialize<Dictionary<string, List<RollStatistic>>>(json);
        }
        else
        {
            stats = new Dictionary<string, List<RollStatistic>>();
        }
        
        if (!stats.ContainsKey(diceType))
        {
            stats[diceType] = new List<RollStatistic>();
        }
        
        stats[diceType].Add(new RollStatistic
        {
            Timestamp = DateTime.Now,
            DiceCount = count,
            TotalRoll = total
        });
        
        string updatedJson = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(statsFilePath, updatedJson);
    }
    
    private void ViewStatistics()
    {
        if (!File.Exists(statsFilePath))
        {
            Console.WriteLine("No statistics available yet.");
            return;
        }
        
        string json = File.ReadAllText(statsFilePath);
        var stats = JsonSerializer.Deserialize<Dictionary<string, List<RollStatistic>>>(json);
        
        Console.WriteLine("\nRoll Statistics:");
        foreach (var entry in stats)
        {
            Console.WriteLine("\nDice Type: " + entry.Key);
            Console.WriteLine("Total Rolls: " + entry.Value.Count);
            
            int totalDice = 0;
            int totalSum = 0;
            
            foreach (var stat in entry.Value)
            {
                totalDice += stat.DiceCount;
                totalSum += stat.TotalRoll;
            }
            
            Console.WriteLine("Total Dice Rolled: " + totalDice);
            Console.WriteLine("Average Roll: " + (totalDice > 0 ? (double)totalSum / totalDice : 0).ToString("F2"));
        }
    }
}

public class RollStatistic
{
    public DateTime Timestamp { get; set; }
    public int DiceCount { get; set; }
    public int TotalRoll { get; set; }
}