using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class EscapeRoomGame : IGeneratedModule
{
    public string Name { get; set; } = "Text-Based Escape Room Game";

    private string dataFolder;
    private GameState gameState;

    public bool Main(string dataFolder)
    {
        this.dataFolder = dataFolder;
        LoadGameState();

        Console.WriteLine("Welcome to the Text-Based Escape Room!");
        Console.WriteLine("You find yourself locked in a mysterious room. Find a way to escape!");
        Console.WriteLine("Type 'help' for available commands.\n");

        bool gameRunning = true;
        while (gameRunning)
        {
            Console.Write("> ");
            string input = Console.ReadLine().Trim().ToLower();

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
                case "exit":
                    gameRunning = false;
                    break;
                default:
                    if (input.StartsWith("take "))
                    {
                        TakeItem(input.Substring(5));
                    }
                    else if (input.StartsWith("use "))
                    {
                        UseItem(input.Substring(4));
                    }
                    else
                    {
                        Console.WriteLine("I don't understand that command. Type 'help' for available commands.");
                    }
                    break;
            }

            if (gameState.HasEscaped)
            {
                Console.WriteLine("\nCongratulations! You've successfully escaped the room!");
                gameRunning = false;
            }
        }

        SaveGameState();
        return true;
    }

    private void ShowHelp()
    {
        Console.WriteLine("\nAvailable commands:");
        Console.WriteLine("help - Show this help message");
        Console.WriteLine("look - Look around the room");
        Console.WriteLine("take [item] - Pick up an item");
        Console.WriteLine("use [item] - Use an item from your inventory");
        Console.WriteLine("inventory - Show items in your inventory");
        Console.WriteLine("exit - Quit the game\n");
    }

    private void LookAround()
    {
        Console.WriteLine("\nYou look around the dimly lit room:");
        Console.WriteLine("- There's a sturdy wooden door with a keypad.");
        Console.WriteLine("- A small desk with a drawer stands in the corner.");
        Console.WriteLine("- A bookshelf contains various old books.");
        Console.WriteLine("- A painting hangs slightly crooked on the wall.\n");

        if (!gameState.HasTakenNote && !gameState.HasCheckedDesk)
        {
            Console.WriteLine("You notice a piece of paper on the desk.\n");
        }
    }

    private void TakeItem(string item)
    {
        if (item == "paper" || item == "note")
        {
            if (!gameState.HasTakenNote)
            {
                gameState.HasTakenNote = true;
                gameState.Inventory.Add("note");
                Console.WriteLine("\nYou take the note. It appears to have numbers written on it: 7392\n");
            }
            else
            {
                Console.WriteLine("\nYou've already taken the note.\n");
            }
        }
        else
        {
            Console.WriteLine("\nYou can't take that.\n");
        }
    }

    private void UseItem(string item)
    {
        if (gameState.Inventory.Contains(item))
        {
            if (item == "note")
            {
                Console.WriteLine("\nYou examine the note again. The numbers 7392 might be important.\n");
            }
            else if (item == "key")
            {
                Console.WriteLine("\nYou don't have a key to use.\n");
            }
        }
        else if (item == "7392" || item == "keypad")
        {
            if (gameState.HasTakenNote)
            {
                Console.WriteLine("\nYou enter the code 7392 on the keypad. The door clicks open!\n");
                gameState.HasEscaped = true;
            }
            else
            {
                Console.WriteLine("\nYou don't know the code to the keypad. Maybe you should look around more.\n");
            }
        }
        else
        {
            Console.WriteLine("\nYou can't use that.\n");
        }
    }

    private void ShowInventory()
    {
        if (gameState.Inventory.Count == 0)
        {
            Console.WriteLine("\nYour inventory is empty.\n");
        }
        else
        {
            Console.WriteLine("\nInventory:");
            foreach (var item in gameState.Inventory)
            {
                Console.WriteLine("- " + item);
            }
            Console.WriteLine();
        }
    }

    private void LoadGameState()
    {
        string saveFile = Path.Combine(dataFolder, "escape_room_save.json");
        if (File.Exists(saveFile))
        {
            try
            {
                string json = File.ReadAllText(saveFile);
                gameState = JsonSerializer.Deserialize<GameState>(json);
                return;
            }
            catch { }
        }

        // Default game state
        gameState = new GameState
        {
            HasTakenNote = false,
            HasCheckedDesk = false,
            HasEscaped = false,
            Inventory = new System.Collections.Generic.List<string>()
        };
    }

    private void SaveGameState()
    {
        string saveFile = Path.Combine(dataFolder, "escape_room_save.json");
        try
        {
            string json = JsonSerializer.Serialize(gameState);
            File.WriteAllText(saveFile, json);
        }
        catch { }
    }

    private class GameState
    {
        public bool HasTakenNote { get; set; }
        public bool HasCheckedDesk { get; set; }
        public bool HasEscaped { get; set; }
        public System.Collections.Generic.List<string> Inventory { get; set; }
    }
}