using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class RockPaperScissorsGame : IGeneratedModule
{
    public string Name { get; set; } = "Rock-Paper-Scissors Game";
    
    private Random _random;
    
    public RockPaperScissorsGame()
    {
        _random = new Random();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Welcome to Rock-Paper-Scissors!");
        Console.WriteLine("Enter your choice (rock, paper, or scissors):");
        
        string userChoice = Console.ReadLine()?.ToLower().Trim();
        
        if (userChoice != "rock" && userChoice != "paper" && userChoice != "scissors")
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return false;
        }
        
        string[] choices = { "rock", "paper", "scissors" };
        string computerChoice = choices[_random.Next(choices.Length)];
        
        Console.WriteLine("Computer chose: " + computerChoice);
        
        string result = DetermineWinner(userChoice, computerChoice);
        
        Console.WriteLine(result);
        
        SaveGameResult(dataFolder, userChoice, computerChoice, result);
        
        return true;
    }
    
    private string DetermineWinner(string userChoice, string computerChoice)
    {
        if (userChoice == computerChoice)
        {
            return "It's a tie!";
        }
        
        if ((userChoice == "rock" && computerChoice == "scissors") ||
            (userChoice == "paper" && computerChoice == "rock") ||
            (userChoice == "scissors" && computerChoice == "paper"))
        {
            return "You win!";
        }
        
        return "Computer wins!";
    }
    
    private void SaveGameResult(string dataFolder, string userChoice, string computerChoice, string result)
    {
        try
        {
            string filePath = Path.Combine(dataFolder, "rps_results.json");
            
            var gameResult = new
            {
                UserChoice = userChoice,
                ComputerChoice = computerChoice,
                Result = result,
                Timestamp = DateTime.Now
            };
            
            string jsonString = JsonSerializer.Serialize(gameResult);
            
            File.AppendAllText(filePath, jsonString + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving game result: " + ex.Message);
        }
    }
}