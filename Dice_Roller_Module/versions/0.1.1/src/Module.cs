using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class DiceRollerModule : IGeneratedModule
{
    public string Name { get; set; } = "Dice Roller Module";
    private Random _random;
    private string _configFilePath;
    private DiceRollerConfig _config;

    public DiceRollerModule()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        _configFilePath = Path.Combine(dataFolder, "dice_roller_config.json");
        LoadOrCreateConfig();

        Console.WriteLine("Dice Roller Module is running.");
        Console.WriteLine("Available dice types: " + string.Join(", ", _config.AvailableDice));

        while (true)
        {
            Console.WriteLine("\nEnter dice type (e.g., d6, d20) or 'exit' to quit:");
            string input = Console.ReadLine().Trim().ToLower();

            if (input == "exit")
            {
                break;
            }

            if (!_config.AvailableDice.Contains(input))
            {
                Console.WriteLine("Invalid dice type. Try again.");
                continue;
            }


            Console.WriteLine("Enter number of dice to roll:");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Invalid number. Try again.");
                continue;
            }

            RollDice(input, count);
        }

        SaveConfig();
        return true;
    }

    private void RollDice(string diceType, int count)
    {
        int sides = int.Parse(diceType.Substring(1));
        List<int> results = new List<int>();

        for (int i = 0; i < count; i++)
        {
            results.Add(_random.Next(1, sides + 1));
        }

        Console.WriteLine("Rolling " + count + " " + diceType + ": " + string.Join(", ", results));
        Console.WriteLine("Total: " + results.Sum());
    }

    private void LoadOrCreateConfig()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                string json = File.ReadAllText(_configFilePath);
                _config = JsonSerializer.Deserialize<DiceRollerConfig>(json);
            }
            else
            {
                _config = new DiceRollerConfig
                {
                    AvailableDice = new List<string> { "d4", "d6", "d8", "d10", "d12", "d20", "d100" }
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading config: " + ex.Message);
            _config = new DiceRollerConfig
            {
                AvailableDice = new List<string> { "d4", "d6", "d8", "d10", "d12", "d20", "d100" }
            };
        }
    }

    private void SaveConfig()
    {
        try
        {
            string json = JsonSerializer.Serialize(_config);
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving config: " + ex.Message);
        }
    }

    private class DiceRollerConfig
    {
        public List<string> AvailableDice { get; set; }
    }
}