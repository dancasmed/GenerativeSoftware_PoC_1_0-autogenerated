using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class CoinTossDiceRollSimulator : IGeneratedModule
{
    public string Name { get; set; } = "Coin Toss & Dice Roll Simulator";
    private Random _random;
    private const string ResultsFileName = "simulation_results.json";

    public CoinTossDiceRollSimulator()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Coin Toss and Dice Roll Simulator...");

        try
        {
            var resultsPath = Path.Combine(dataFolder, ResultsFileName);
            var simulationData = new SimulationData();

            Console.WriteLine("Performing 10 coin tosses...");
            for (int i = 0; i < 10; i++)
            {
                var result = TossCoin();
                simulationData.CoinTossResults.Add(result);
                Console.WriteLine("Coin toss " + (i + 1) + ": " + result);
            }

            Console.WriteLine("\nPerforming 10 dice rolls...");
            for (int i = 0; i < 10; i++)
            {
                var result = RollDice();
                simulationData.DiceRollResults.Add(result);
                Console.WriteLine("Dice roll " + (i + 1) + ": " + result);
            }

            SaveResults(resultsPath, simulationData);
            Console.WriteLine("\nResults saved to: " + resultsPath);
            Console.WriteLine("Simulation completed successfully.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during simulation: " + ex.Message);
            return false;
        }
    }

    private string TossCoin()
    {
        return _random.Next(2) == 0 ? "Heads" : "Tails";
    }

    private int RollDice()
    {
        return _random.Next(1, 7);
    }

    private void SaveResults(string filePath, SimulationData data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }
}

public class SimulationData
{
    public List<string> CoinTossResults { get; set; } = new List<string>();
    public List<int> DiceRollResults { get; set; } = new List<int>();
}