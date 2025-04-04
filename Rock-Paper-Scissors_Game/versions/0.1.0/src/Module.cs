using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class RockPaperScissorsGame : IGeneratedModule
{
    public string Name { get; set; } = "Rock-Paper-Scissors Game";
    private Random _random;
    private string _statsFilePath;
    private GameStats _stats;

    public RockPaperScissorsGame()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        _statsFilePath = Path.Combine(dataFolder, "rps_stats.json");
        LoadStats();

        Console.WriteLine("Welcome to Rock-Paper-Scissors!");
        Console.WriteLine("Enter your choice (rock, paper, scissors) or 'quit' to exit:");

        bool running = true;
        while (running)
        {
            string input = Console.ReadLine()?.ToLower().Trim();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "quit")
            {
                running = false;
                continue;
            }

            if (!IsValidChoice(input))
            {
                Console.WriteLine("Invalid choice. Please enter rock, paper, or scissors.");
                continue;
            }

            string computerChoice = GetComputerChoice();
            string result = DetermineWinner(input, computerChoice);

            UpdateStats(result);
            SaveStats();

            Console.WriteLine("You chose: " + input);
            Console.WriteLine("Computer chose: " + computerChoice);
            Console.WriteLine("Result: " + result);
            Console.WriteLine("Current stats - Wins: " + _stats.Wins + ", Losses: " + _stats.Losses + ", Ties: " + _stats.Ties);
            Console.WriteLine("Enter your next choice or 'quit' to exit:");
        }

        Console.WriteLine("Thanks for playing!");
        return true;
    }

    private bool IsValidChoice(string choice)
    {
        return choice == "rock" || choice == "paper" || choice == "scissors";
    }

    private string GetComputerChoice()
    {
        int choice = _random.Next(0, 3);
        return choice switch
        {
            0 => "rock",
            1 => "paper",
            _ => "scissors"
        };
    }

    private string DetermineWinner(string playerChoice, string computerChoice)
    {
        if (playerChoice == computerChoice)
            return "tie";

        if ((playerChoice == "rock" && computerChoice == "scissors") ||
            (playerChoice == "paper" && computerChoice == "rock") ||
            (playerChoice == "scissors" && computerChoice == "paper"))
            return "win";

        return "lose";
    }

    private void UpdateStats(string result)
    {
        switch (result)
        {
            case "win":
                _stats.Wins++;
                break;
            case "lose":
                _stats.Losses++;
                break;
            case "tie":
                _stats.Ties++;
                break;
        }
    }

    private void LoadStats()
    {
        try
        {
            if (File.Exists(_statsFilePath))
            {
                string json = File.ReadAllText(_statsFilePath);
                _stats = JsonSerializer.Deserialize<GameStats>(json) ?? new GameStats();
            }
            else
            {
                _stats = new GameStats();
            }
        }
        catch
        {
            _stats = new GameStats();
        }
    }

    private void SaveStats()
    {
        try
        {
            string json = JsonSerializer.Serialize(_stats);
            File.WriteAllText(_statsFilePath, json);
        }
        catch
        {
            // Silently fail if we can't save stats
        }
    }

    private class GameStats
    {
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Ties { get; set; } = 0;
    }
}