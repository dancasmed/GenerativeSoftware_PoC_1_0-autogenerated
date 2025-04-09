using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DiceRollGame : IGeneratedModule
{
    public string Name { get; set; } = "Dice Roll Game";
    
    private List<Player> players = new List<Player>();
    private string dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Dice Roll Game...");
        
        dataFilePath = Path.Combine(dataFolder, "players.json");
        
        LoadPlayers();
        
        if (players.Count == 0)
        {
            Console.WriteLine("No players found. Adding default players.");
            AddDefaultPlayers();
        }
        
        PlayGame();
        
        SavePlayers();
        
        Console.WriteLine("Dice Roll Game completed.");
        return true;
    }
    
    private void LoadPlayers()
    {
        if (File.Exists(dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(dataFilePath);
                players = JsonSerializer.Deserialize<List<Player>>(json);
                Console.WriteLine("Players loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading players: " + ex.Message);
            }
        }
    }
    
    private void SavePlayers()
    {
        try
        {
            string json = JsonSerializer.Serialize(players);
            File.WriteAllText(dataFilePath, json);
            Console.WriteLine("Players saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving players: " + ex.Message);
        }
    }
    
    private void AddDefaultPlayers()
    {
        players.Add(new Player("Player 1"));
        players.Add(new Player("Player 2"));
        players.Add(new Player("Player 3"));
    }
    
    private void PlayGame()
    {
        Console.WriteLine("Starting the game with " + players.Count + " players.");
        
        Random random = new Random();
        
        foreach (var player in players)
        {
            int roll = random.Next(1, 7);
            player.Score += roll;
            Console.WriteLine(player.Name + " rolled a " + roll + ". Total score: " + player.Score);
        }
        
        players.Sort((p1, p2) => p2.Score.CompareTo(p1.Score));
        
        Console.WriteLine("Game results:");
        for (int i = 0; i < players.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + players[i].Name + " - " + players[i].Score + " points");
        }
    }
}

public class Player
{
    public string Name { get; set; }
    public int Score { get; set; }
    
    public Player(string name)
    {
        Name = name;
        Score = 0;
    }
}