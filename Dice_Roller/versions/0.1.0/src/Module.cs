using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DiceRollerModule : IGeneratedModule
{
    public string Name { get; set; } = "Dice Roller";
    private Dictionary<string, int> _diceTypes = new Dictionary<string, int>
    {
        { "d4", 4 },
        { "d6", 6 },
        { "d8", 8 },
        { "d10", 10 },
        { "d12", 12 },
        { "d20", 20 },
        { "d100", 100 }
    };
    private Random _random = new Random();
    private string _statsFilePath;

    public DiceRollerModule()
    {
    }

    public bool Main(string dataFolder)
    {
        _statsFilePath = Path.Combine(dataFolder, "dice_roll_stats.json");
        
        Console.WriteLine("Dice Roller Module Started");
        Console.WriteLine("Available dice types: d4, d6, d8, d10, d12, d20, d100");
        
        while (true)
        {
            Console.Write("Enter dice type (or 'quit' to exit): ");
            string input = Console.ReadLine()?.Trim().ToLower();
            
            if (input == "quit")
            {
                Console.WriteLine("Exiting Dice Roller Module");
                return true;
            }
            
            if (!_diceTypes.ContainsKey(input))
            {
                Console.WriteLine("Invalid dice type. Please try again.");
                continue;
            }
            
            Console.Write("Enter number of dice to roll: ");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Invalid number. Please enter a positive integer.");
                continue;
            }
            
            RollDice(input, count);
        }
    }
    
    private void RollDice(string diceType, int count)
    {
        int maxValue = _diceTypes[diceType];
        List<int> results = new List<int>();
        int total = 0;
        
        for (int i = 0; i < count; i++)
        {
            int roll = _random.Next(1, maxValue + 1);
            results.Add(roll);
            total += roll;
        }
        
        Console.WriteLine("Rolling " + count + " " + diceType + ": " + string.Join(", ", results));
        Console.WriteLine("Total: " + total);
        
        SaveRollStats(diceType, count, total);
    }
    
    private void SaveRollStats(string diceType, int count, int total)
    {
        Dictionary<string, List<RollStat>> stats;
        
        if (File.Exists(_statsFilePath))
        {
            string json = File.ReadAllText(_statsFilePath);
            stats = JsonSerializer.Deserialize<Dictionary<string, List<RollStat>>>(json);
        }
        else
        {
            stats = new Dictionary<string, List<RollStat>>();
        }
        
        if (!stats.ContainsKey(diceType))
        {
            stats[diceType] = new List<RollStat>();
        }
        
        stats[diceType].Add(new RollStat
        {
            Timestamp = DateTime.Now,
            DiceCount = count,
            Total = total
        });
        
        string newJson = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_statsFilePath, newJson);
    }
    
    private class RollStat
    {
        public DateTime Timestamp { get; set; }
        public int DiceCount { get; set; }
        public int Total { get; set; }
    }
}