using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class VirtualDiceModule : IGeneratedModule
{
    public string Name { get; set; } = "Virtual Dice Module";
    private Random _random;
    private string _statsFilePath;

    public VirtualDiceModule()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        _statsFilePath = Path.Combine(dataFolder, "dice_stats.json");
        Console.WriteLine("Virtual Dice Module is running.");
        Console.WriteLine("Type 'roll XdY' to roll X dice with Y sides (e.g., 'roll 2d6').");
        Console.WriteLine("Type 'stats' to view roll statistics.");
        Console.WriteLine("Type 'exit' to quit.");

        string input;
        do
        {
            Console.Write("> ");
            input = Console.ReadLine()?.Trim().ToLower();

            if (input == null) continue;

            if (input.StartsWith("roll "))
            {
                HandleRollCommand(input);
            }
            else if (input == "stats")
            {
                ShowStatistics();
            }
            else if (input != "exit")
            {
                Console.WriteLine("Invalid command. Try again.");
            }
        } while (input != "exit");

        return true;
    }

    private void HandleRollCommand(string input)
    {
        try
        {
            var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                Console.WriteLine("Invalid format. Use 'roll XdY'.");
                return;
            }

            var diceParts = parts[1].Split('d');
            if (diceParts.Length != 2 || !int.TryParse(diceParts[0], out int count) || !int.TryParse(diceParts[1], out int sides))
            {
                Console.WriteLine("Invalid dice format. Use 'XdY' (e.g., '2d6').");
                return;
            }

            if (count < 1 || sides < 2)
            {
                Console.WriteLine("Count must be at least 1 and sides must be at least 2.");
                return;
            }

            var results = new int[count];
            for (int i = 0; i < count; i++)
            {
                results[i] = _random.Next(1, sides + 1);
            }

            Console.WriteLine("Roll results: " + string.Join(", ", results));
            SaveRollStatistic(count, sides, results);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private void SaveRollStatistic(int count, int sides, int[] results)
    {
        try
        {
            DiceStatistics stats;
            if (File.Exists(_statsFilePath))
            {
                var json = File.ReadAllText(_statsFilePath);
                stats = JsonSerializer.Deserialize<DiceStatistics>(json) ?? new DiceStatistics();
            }
            else
            {
                stats = new DiceStatistics();
            }

            stats.TotalRolls += count;
            foreach (var result in results)
            {
                if (!stats.RollCounts.ContainsKey(result))
                {
                    stats.RollCounts[result] = 0;
                }
                stats.RollCounts[result]++;
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var newJson = JsonSerializer.Serialize(stats, options);
            File.WriteAllText(_statsFilePath, newJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to save statistics: " + ex.Message);
        }
    }

    private void ShowStatistics()
    {
        try
        {
            if (!File.Exists(_statsFilePath))
            {
                Console.WriteLine("No statistics available yet.");
                return;
            }

            var json = File.ReadAllText(_statsFilePath);
            var stats = JsonSerializer.Deserialize<DiceStatistics>(json);

            if (stats == null)
            {
                Console.WriteLine("No statistics available yet.");
                return;
            }

            Console.WriteLine("=== Dice Roll Statistics ===");
            Console.WriteLine("Total rolls: " + stats.TotalRolls);
            Console.WriteLine("Roll counts:");

            foreach (var kvp in stats.RollCounts)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value} times");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load statistics: " + ex.Message);
        }
    }

    private class DiceStatistics
    {
        public int TotalRolls { get; set; } = 0;
        public Dictionary<int, int> RollCounts { get; set; } = new Dictionary<int, int>();
    }
}