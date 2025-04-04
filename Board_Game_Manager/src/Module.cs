using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BoardGameManager : IGeneratedModule
{
    public string Name { get; set; } = "Board Game Manager";
    
    private List<BoardGame> _games = new List<BoardGame>();
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Board Game Manager module...");
        
        _dataFilePath = Path.Combine(dataFolder, "boardgames.json");
        
        try
        {
            LoadGames();
            DisplayMenu();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void LoadGames()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _games = JsonSerializer.Deserialize<List<BoardGame>>(json);
            Console.WriteLine("Loaded " + _games.Count + " games from storage.");
        }
        else
        {
            Console.WriteLine("No existing game data found. Starting with empty collection.");
        }
    }
    
    private void SaveGames()
    {
        string json = JsonSerializer.Serialize(_games);
        File.WriteAllText(_dataFilePath, json);
        Console.WriteLine("Game data saved successfully.");
    }
    
    private void DisplayMenu()
    {
        bool running = true;
        
        while (running)
        {
            Console.WriteLine("\nBoard Game Manager");
            Console.WriteLine("1. Add new game");
            Console.WriteLine("2. Record play session");
            Console.WriteLine("3. List all games");
            Console.WriteLine("4. Show statistics");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddNewGame();
                    break;
                case "2":
                    RecordPlaySession();
                    break;
                case "3":
                    ListAllGames();
                    break;
                case "4":
                    ShowStatistics();
                    break;
                case "5":
                    running = false;
                    SaveGames();
                    Console.WriteLine("Exiting Board Game Manager.");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private void AddNewGame()
    {
        Console.Write("Enter game name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter minimum players: ");
        int minPlayers = int.Parse(Console.ReadLine());
        
        Console.Write("Enter maximum players: ");
        int maxPlayers = int.Parse(Console.ReadLine());
        
        Console.Write("Enter average play time (minutes): ");
        int playTime = int.Parse(Console.ReadLine());
        
        _games.Add(new BoardGame
        {
            Name = name,
            MinPlayers = minPlayers,
            MaxPlayers = maxPlayers,
            AveragePlayTime = playTime,
            PlayCount = 0
        });
        
        Console.WriteLine("Game added successfully.");
    }
    
    private void RecordPlaySession()
    {
        if (_games.Count == 0)
        {
            Console.WriteLine("No games available. Please add games first.");
            return;
        }
        
        ListAllGames();
        Console.Write("Select game number: ");
        
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _games.Count)
        {
            _games[index - 1].PlayCount++;
            Console.WriteLine("Play session recorded for " + _games[index - 1].Name);
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }
    
    private void ListAllGames()
    {
        if (_games.Count == 0)
        {
            Console.WriteLine("No games in collection.");
            return;
        }
        
        Console.WriteLine("\nCurrent Game Collection:");
        for (int i = 0; i < _games.Count; i++)
        {
            var game = _games[i];
            Console.WriteLine(string.Format("{0}. {1} ({2}-{3} players, {4} min) - Played {5} times",
                i + 1, game.Name, game.MinPlayers, game.MaxPlayers, game.AveragePlayTime, game.PlayCount));
        }
    }
    
    private void ShowStatistics()
    {
        if (_games.Count == 0)
        {
            Console.WriteLine("No games in collection.");
            return;
        }
        
        int totalPlays = 0;
        int mostPlayedCount = 0;
        string mostPlayedGame = "";
        
        foreach (var game in _games)
        {
            totalPlays += game.PlayCount;
            if (game.PlayCount > mostPlayedCount)
            {
                mostPlayedCount = game.PlayCount;
                mostPlayedGame = game.Name;
            }
        }
        
        Console.WriteLine("\nGame Statistics:");
        Console.WriteLine(string.Format("Total games: {0}", _games.Count));
        Console.WriteLine(string.Format("Total play sessions: {0}", totalPlays));
        Console.WriteLine(string.Format("Most played game: {0} ({1} plays)", mostPlayedGame, mostPlayedCount));
    }
}

public class BoardGame
{
    public string Name { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int AveragePlayTime { get; set; }
    public int PlayCount { get; set; }
}