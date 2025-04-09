using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FantasyFootballDraftManager : IGeneratedModule
{
    public string Name { get; set; } = "Fantasy Football Draft Manager";
    
    private List<Player> _availablePlayers;
    private List<Player> _draftedPlayers;
    private string _dataFolder;
    
    public FantasyFootballDraftManager()
    {
        _availablePlayers = new List<Player>();
        _draftedPlayers = new List<Player>();
    }
    
    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        
        Console.WriteLine("Initializing Fantasy Football Draft Manager...");
        
        LoadInitialPlayers();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ViewAvailablePlayers();
                    break;
                case "2":
                    ViewDraftedPlayers();
                    break;
                case "3":
                    DraftPlayer();
                    break;
                case "4":
                    SaveDraftResults();
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Fantasy Football Draft Manager session ended.");
        return true;
    }
    
    private void LoadInitialPlayers()
    {
        // Sample players - in a real app, this would load from a file
        _availablePlayers = new List<Player>
        {
            new Player { Name = "Patrick Mahomes", Position = "QB", Team = "KC", Points = 400 },
            new Player { Name = "Christian McCaffrey", Position = "RB", Team = "SF", Points = 350 },
            new Player { Name = "Justin Jefferson", Position = "WR", Team = "MIN", Points = 300 },
            new Player { Name = "Travis Kelce", Position = "TE", Team = "KC", Points = 250 },
            new Player { Name = "Josh Allen", Position = "QB", Team = "BUF", Points = 380 },
            new Player { Name = "Cooper Kupp", Position = "WR", Team = "LAR", Points = 290 },
            new Player { Name = "Derrick Henry", Position = "RB", Team = "TEN", Points = 320 },
            new Player { Name = "Davante Adams", Position = "WR", Team = "LV", Points = 280 }
        };
        
        Console.WriteLine("Loaded " + _availablePlayers.Count + " players into the draft pool.");
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nFantasy Football Draft Manager");
        Console.WriteLine("1. View Available Players");
        Console.WriteLine("2. View Drafted Players");
        Console.WriteLine("3. Draft a Player");
        Console.WriteLine("4. Save and Exit");
        Console.Write("Select an option: ");
    }
    
    private void ViewAvailablePlayers()
    {
        Console.WriteLine("\nAvailable Players:");
        Console.WriteLine("-----------------");
        
        for (int i = 0; i < _availablePlayers.Count; i++)
        {
            Player player = _availablePlayers[i];
            Console.WriteLine(string.Format("{0}. {1} ({2}) - {3} - Projected: {4} pts", 
                i + 1, player.Name, player.Position, player.Team, player.Points));
        }
    }
    
    private void ViewDraftedPlayers()
    {
        if (_draftedPlayers.Count == 0)
        {
            Console.WriteLine("No players have been drafted yet.");
            return;
        }
        
        Console.WriteLine("\nDrafted Players:");
        Console.WriteLine("----------------");
        
        for (int i = 0; i < _draftedPlayers.Count; i++)
        {
            Player player = _draftedPlayers[i];
            Console.WriteLine(string.Format("{0}. {1} ({2}) - {3} - Projected: {4} pts", 
                i + 1, player.Name, player.Position, player.Team, player.Points));
        }
    }
    
    private void DraftPlayer()
    {
        if (_availablePlayers.Count == 0)
        {
            Console.WriteLine("No players available to draft.");
            return;
        }
        
        ViewAvailablePlayers();
        Console.Write("\nEnter the number of the player to draft: ");
        
        if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= _availablePlayers.Count)
        {
            Player draftedPlayer = _availablePlayers[selection - 1];
            _availablePlayers.RemoveAt(selection - 1);
            _draftedPlayers.Add(draftedPlayer);
            
            Console.WriteLine(string.Format("Drafted {0} ({1}) from {2}!", 
                draftedPlayer.Name, draftedPlayer.Position, draftedPlayer.Team));
        }
        else
        {
            Console.WriteLine("Invalid selection. Please try again.");
        }
    }
    
    private void SaveDraftResults()
    {
        try
        {
            string filePath = Path.Combine(_dataFolder, "draft_results.json");
            string json = JsonSerializer.Serialize(_draftedPlayers);
            File.WriteAllText(filePath, json);
            
            Console.WriteLine("Draft results saved successfully to " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving draft results: " + ex.Message);
        }
    }
}

public class Player
{
    public string Name { get; set; }
    public string Position { get; set; }
    public string Team { get; set; }
    public int Points { get; set; }
}