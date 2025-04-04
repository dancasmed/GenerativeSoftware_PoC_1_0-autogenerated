using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class DiceRollerModule : IGeneratedModule
{
    public string Name { get; set; } = "Dice Roller Module";
    private Random random;
    private string statsFilePath;

    public DiceRollerModule()
    {
        random = new Random();
    }

    public bool Main(string dataFolder)
    {
        statsFilePath = Path.Combine(dataFolder, "dice_stats.json");
        Console.WriteLine("Dice Roller Module is running.");
        Console.WriteLine("Enter dice notation (e.g., 2d6) or 'stats' to view statistics, 'exit' to quit:");

        while (true)
        {
            string input = Console.ReadLine().Trim().ToLower();

            if (input == "exit")
            {
                break;
            }
            else if (input == "stats")
            {
                DisplayStatistics();
                continue;
            }

            try
            {
                var result = ParseAndRollDice(input);
                Console.WriteLine("Result: " + result);
                SaveRollToStats(input, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        return true;
    }

    private int ParseAndRollDice(string notation)
    {
        string[] parts = notation.Split('d');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid dice notation. Use format like '2d6'.");

        if (!int.TryParse(parts[0], out int count) || !int.TryParse(parts[1], out int sides))
            throw new ArgumentException("Invalid dice notation. Numbers must be integers.");

        if (count < 1 || sides < 2)
            throw new ArgumentException("Dice count must be at least 1 and sides at least 2.");

        int total = 0;
        for (int i = 0; i < count; i++)
        {
            total += random.Next(1, sides + 1);
        }

        return total;
    }

    private void SaveRollToStats(string notation, int result)
    {
        DiceStats stats = LoadStats();
        stats.TotalRolls++;
        stats.TotalResult += result;

        if (!stats.RollCounts.ContainsKey(notation))
        {
            stats.RollCounts[notation] = 0;
        }
        stats.RollCounts[notation]++;

        SaveStats(stats);
    }

    private DiceStats LoadStats()
    {
        try
        {
            if (File.Exists(statsFilePath))
            {
                string json = File.ReadAllText(statsFilePath);
                return JsonSerializer.Deserialize<DiceStats>(json);
            }
        }
        catch { }

        return new DiceStats();
    }

    private void SaveStats(DiceStats stats)
    {
        try
        {
            string json = JsonSerializer.Serialize(stats);
            File.WriteAllText(statsFilePath, json);
        }
        catch { }
    }

    private void DisplayStatistics()
    {
        DiceStats stats = LoadStats();
        Console.WriteLine("\n--- Dice Rolling Statistics ---");
        Console.WriteLine("Total rolls: " + stats.TotalRolls);
        Console.WriteLine("Average result: " + (stats.TotalRolls > 0 ? (stats.TotalResult / (double)stats.TotalRolls).ToString("F2") : "0"));
        Console.WriteLine("Roll counts by type:");

        foreach (var entry in stats.RollCounts)
        {
            Console.WriteLine(entry.Key + ": " + entry.Value);
        }

        Console.WriteLine();
    }
}

public class DiceStats
{
    public int TotalRolls { get; set; } = 0;
    public int TotalResult { get; set; } = 0;
    public Dictionary<string, int> RollCounts { get; set; } = new Dictionary<string, int>();
}