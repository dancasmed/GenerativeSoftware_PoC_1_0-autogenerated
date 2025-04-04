using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class EscapeRoomPuzzle : IGeneratedModule
{
    public string Name { get; set; } = "Text-Based Escape Room Puzzle";
    
    private string _dataFolder;
    private PuzzleState _puzzleState;
    
    public EscapeRoomPuzzle()
    {
    }
    
    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        LoadPuzzleState();
        
        Console.WriteLine("Welcome to the Text-Based Escape Room Puzzle!");
        Console.WriteLine("You find yourself locked in a mysterious room.");
        Console.WriteLine("Type 'help' for available commands.");
        
        bool exitRequested = false;
        while (!exitRequested && !_puzzleState.HasEscaped)
        {
            Console.Write("> ");
            string input = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;
            
            switch (input)
            {
                case "help":
                    ShowHelp();
                    break;
                case "look":
                    LookAround();
                    break;
                case "inventory":
                    ShowInventory();
                    break;
                case "use key":
                    UseKey();
                    break;
                case "open door":
                    OpenDoor();
                    break;
                case "exit":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("I don't understand that command.");
                    break;
            }
            
            SavePuzzleState();
        }
        
        if (_puzzleState.HasEscaped)
        {
            Console.WriteLine("Congratulations! You've escaped the room!");
        }
        
        return _puzzleState.HasEscaped;
    }
    
    private void ShowHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  help - Show this help message");
        Console.WriteLine("  look - Look around the room");
        Console.WriteLine("  inventory - Show your inventory");
        Console.WriteLine("  use key - Use the key in your inventory");
        Console.WriteLine("  open door - Try to open the door");
        Console.WriteLine("  exit - Quit the game");
    }
    
    private void LookAround()
    {
        if (!_puzzleState.HasFoundKey)
        {
            Console.WriteLine("You see a dusty room with a locked door. There's a small table with a drawer.");
            Console.WriteLine("Would you like to search the drawer? (yes/no)");
            
            string answer = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;
            if (answer.Equals("yes"))
            {
                _puzzleState.HasFoundKey = true;
                _puzzleState.Inventory.Add("key");
                Console.WriteLine("You found a small key in the drawer!");
            }
        }
        else
        {
            Console.WriteLine("The room looks the same, but you've already found the key.");
        }
    }
    
    private void ShowInventory()
    {
        if (_puzzleState.Inventory.Count == 0)
        {
            Console.WriteLine("Your inventory is empty.");
        }
        else
        {
            Console.WriteLine("Inventory:");
            foreach (var item in _puzzleState.Inventory)
            {
                Console.WriteLine(" - " + item);
            }
        }
    }
    
    private void UseKey()
    {
        if (_puzzleState.Inventory.Contains("key"))
        {
            _puzzleState.HasUsedKey = true;
            Console.WriteLine("You've used the key to unlock the door. Now try 'open door'.");
        }
        else
        {
            Console.WriteLine("You don't have a key to use.");
        }
    }
    
    private void OpenDoor()
    {
        if (_puzzleState.HasUsedKey)
        {
            _puzzleState.HasEscaped = true;
            Console.WriteLine("The door opens! You step out into freedom.");
        }
        else
        {
            Console.WriteLine("The door is locked. You need to find and use a key first.");
        }
    }
    
    private void LoadPuzzleState()
    {
        string filePath = Path.Combine(_dataFolder, "escape_room_state.json");
        
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _puzzleState = JsonSerializer.Deserialize<PuzzleState>(json) ?? new PuzzleState();
            }
            else
            {
                _puzzleState = new PuzzleState();
            }
        }
        catch
        {
            _puzzleState = new PuzzleState();
        }
    }
    
    private void SavePuzzleState()
    {
        string filePath = Path.Combine(_dataFolder, "escape_room_state.json");
        
        try
        {
            string json = JsonSerializer.Serialize(_puzzleState);
            File.WriteAllText(filePath, json);
        }
        catch
        {
            // Silently fail if we can't save
        }
    }
}

public class PuzzleState
{
    public bool HasFoundKey { get; set; } = false;
    public bool HasUsedKey { get; set; } = false;
    public bool HasEscaped { get; set; } = false;
    public List<string> Inventory { get; set; } = new List<string>();
}