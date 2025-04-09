using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class RockPaperScissorsGame : IGeneratedModule
{
    public string Name { get; set; } = "Rock Paper Scissors Game";

    private Random random = new Random();
    private string[] choices = { "Rock", "Paper", "Scissors" };
    private string statsFilePath;
    private GameStats stats;

    public bool Main(string dataFolder)
    {
        statsFilePath = Path.Combine(dataFolder, "rps_stats.json");
        LoadStats();

        Console.WriteLine("Welcome to Rock-Paper-Scissors!");
        Console.WriteLine("Enter your choice (Rock, Paper, Scissors) or 'Q' to quit:");

        bool running = true;
        while (running)
        {
            string input = Console.ReadLine()?.Trim();
            if (string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase))
            {
                running = false;
                continue;
            }

            if (!ValidateInput(input))
            {
                Console.WriteLine("Invalid choice. Please enter Rock, Paper, or Scissors.");
                continue;
            }

            string playerChoice = input;
            string computerChoice = GetComputerChoice();

            Console.WriteLine("You chose: " + playerChoice);
            Console.WriteLine("Computer chose: " + computerChoice);

            string result = DetermineWinner(playerChoice, computerChoice);
            Console.WriteLine(result);

            UpdateStats(result);
            SaveStats();

            Console.WriteLine("\nPlay again? Enter your choice or 'Q' to quit:");
        }

        DisplayStats();
        Console.WriteLine("Thanks for playing!");
        return true;
    }

    private bool ValidateInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        foreach (string choice in choices)
        {
            if (string.Equals(input, choice, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private string GetComputerChoice()
    {
        return choices[random.Next(choices.Length)];
    }

    private string DetermineWinner(string player, string computer)
    {
        if (player.Equals(computer, StringComparison.OrdinalIgnoreCase))
            return "It's a tie!";

        if ((player.Equals("Rock", StringComparison.OrdinalIgnoreCase) && computer.Equals("Scissors", StringComparison.OrdinalIgnoreCase)) ||
            (player.Equals("Paper", StringComparison.OrdinalIgnoreCase) && computer.Equals("Rock", StringComparison.OrdinalIgnoreCase)) ||
            (player.Equals("Scissors", StringComparison.OrdinalIgnoreCase) && computer.Equals("Paper", StringComparison.OrdinalIgnoreCase)))
        {
            return "You win!";
        }

        return "Computer wins!";
    }

    private void LoadStats()
    {
        try
        {
            if (File.Exists(statsFilePath))
            {
                string json = File.ReadAllText(statsFilePath);
                stats = JsonSerializer.Deserialize<GameStats>(json) ?? new GameStats();
            }
            else
            {
                stats = new GameStats();
            }
        }
        catch
        {
            stats = new GameStats();
        }
    }

    private void SaveStats()
    {
        try
        {
            string json = JsonSerializer.Serialize(stats);
            File.WriteAllText(statsFilePath, json);
        }
        catch { }
    }

    private void UpdateStats(string result)
    {
        stats.TotalGames++;

        if (result.Contains("win"))
            stats.Wins++;
        else if (result.Contains("Computer"))
            stats.Losses++;
        else
            stats.Ties++;
    }

    private void DisplayStats()
    {
        Console.WriteLine("\nGame Statistics:");
        Console.WriteLine("Total games: " + stats.TotalGames);
        Console.WriteLine("Wins: " + stats.Wins);
        Console.WriteLine("Losses: " + stats.Losses);
        Console.WriteLine("Ties: " + stats.Ties);
    }

    private class GameStats
    {
        public int TotalGames { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Ties { get; set; } = 0;
    }
}