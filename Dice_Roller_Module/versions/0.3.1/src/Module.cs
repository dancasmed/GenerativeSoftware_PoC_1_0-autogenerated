using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class DiceRollerModule : IGeneratedModule
{
    public string Name { get; set; } = "Dice Roller Module";
    private Random _random;

    public DiceRollerModule()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Dice Roller Module is running.");
        Console.WriteLine("Available commands: roll [sides], history, exit");

        string historyFilePath = Path.Combine(dataFolder, "dice_roll_history.json");
        List<DiceRoll> history = LoadHistory(historyFilePath);

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine().Trim().ToLower();

            if (input == "exit")
            {
                SaveHistory(historyFilePath, history);
                return true;
            }
            else if (input == "history")
            {
                DisplayHistory(history);
            }
            else if (input.StartsWith("roll "))
            {
                string[] parts = input.Split(' ');
                if (parts.Length == 2 && int.TryParse(parts[1], out int sides) && sides > 0)
                {
                    int result = RollDice(sides);
                    Console.WriteLine("Rolled: " + result);
                    history.Add(new DiceRoll(sides, result, DateTime.Now));
                }
                else
                {
                    Console.WriteLine("Invalid command. Usage: roll [number of sides]");
                }
            }
            else
            {
                Console.WriteLine("Unknown command. Available commands: roll [sides], history, exit");
            }
        }
    }

    private int RollDice(int sides)
    {
        return _random.Next(1, sides + 1);
    }

    private List<DiceRoll> LoadHistory(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<DiceRoll>>(json) ?? new List<DiceRoll>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading history: " + ex.Message);
        }
        return new List<DiceRoll>();
    }

    private void SaveHistory(string filePath, List<DiceRoll> history)
    {
        try
        {
            string json = JsonSerializer.Serialize(history);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving history: " + ex.Message);
        }
    }

    private void DisplayHistory(List<DiceRoll> history)
    {
        if (history.Count == 0)
        {
            Console.WriteLine("No rolls in history.");
            return;
        }

        Console.WriteLine("Roll History:");
        foreach (var roll in history)
        {
            Console.WriteLine($"{roll.Timestamp}: {roll.Result} (d{roll.Sides})");
        }
    }
}

public class DiceRoll
{
    public int Sides { get; set; }
    public int Result { get; set; }
    public DateTime Timestamp { get; set; }

    public DiceRoll(int sides, int result, DateTime timestamp)
    {
        Sides = sides;
        Result = result;
        Timestamp = timestamp;
    }
}