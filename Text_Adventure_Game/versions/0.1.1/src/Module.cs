using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class TextAdventureGame : IGeneratedModule
{
    public string Name { get; set; } = "Text Adventure Game";
    
    private string _dataFolder;
    private GameState _gameState;
    
    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        LoadGameState();
        
        Console.WriteLine("Welcome to the Text Adventure Game!");
        Console.WriteLine("Navigate through the story by making choices.");
        
        while (_gameState.CurrentSceneId != "end")
        {
            DisplayScene(_gameState.CurrentSceneId);
            ProcessChoice(GetPlayerChoice());
        }
        
        Console.WriteLine("Game Over. Thanks for playing!");
        SaveGameState();
        return true;
    }
    
    private void LoadGameState()
    {
        string savePath = Path.Combine(_dataFolder, "savedata.json");
        
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            _gameState = JsonSerializer.Deserialize<GameState>(json);
        }
        else
        {
            _gameState = new GameState { CurrentSceneId = "start" };
        }
    }
    
    private void SaveGameState()
    {
        string savePath = Path.Combine(_dataFolder, "savedata.json");
        string json = JsonSerializer.Serialize(_gameState);
        File.WriteAllText(savePath, json);
    }
    
    private void DisplayScene(string sceneId)
    {
        var scene = GetScene(sceneId);
        Console.WriteLine(scene.Description);
        
        for (int i = 0; i < scene.Choices.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + scene.Choices[i].Description);
        }
    }
    
    private int GetPlayerChoice()
    {
        while (true)
        {
            Console.Write("Enter your choice: ");
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= GetScene(_gameState.CurrentSceneId).Choices.Count)
            {
                return choice - 1;
            }
            
            Console.WriteLine("Invalid choice. Please try again.");
        }
    }
    
    private void ProcessChoice(int choiceIndex)
    {
        var currentScene = GetScene(_gameState.CurrentSceneId);
        _gameState.CurrentSceneId = currentScene.Choices[choiceIndex].NextSceneId;
    }
    
    private Scene GetScene(string sceneId)
    {
        return sceneId switch
        {
            "start" => new Scene
            {
                Description = "You wake up in a dark forest. The path splits in two directions.",
                Choices = new List<Choice>
                {
                    new Choice { Description = "Go left towards the cave", NextSceneId = "cave" },
                    new Choice { Description = "Go right towards the river", NextSceneId = "river" }
                }
            },
            "cave" => new Scene
            {
                Description = "You enter a damp cave. There's a chest and a tunnel deeper in.",
                Choices = new List<Choice>
                {
                    new Choice { Description = "Open the chest", NextSceneId = "chest" },
                    new Choice { Description = "Go deeper into the tunnel", NextSceneId = "tunnel" }
                }
            },
            "river" => new Scene
            {
                Description = "You reach a fast-flowing river. There's a boat and a bridge.",
                Choices = new List<Choice>
                {
                    new Choice { Description = "Take the boat", NextSceneId = "boat" },
                    new Choice { Description = "Cross the bridge", NextSceneId = "bridge" }
                }
            },
            "chest" => new Scene
            {
                Description = "You find a treasure! The adventure ends here.",
                Choices = new List<Choice>(),
            },
            "tunnel" => new Scene
            {
                Description = "You get lost in the dark tunnel. Game over.",
                Choices = new List<Choice>(),
            },
            "boat" => new Scene
            {
                Description = "The boat capsizes! You drown. Game over.",
                Choices = new List<Choice>(),
            },
            "bridge" => new Scene
            {
                Description = "You safely cross the bridge and find civilization. You win!",
                Choices = new List<Choice>(),
            },
            _ => new Scene
            {
                Description = "The adventure has ended.",
                Choices = new List<Choice>(),
            }
        };
    }
}

public class GameState
{
    public string CurrentSceneId { get; set; }
}

public class Scene
{
    public string Description { get; set; }
    public List<Choice> Choices { get; set; } = new List<Choice>();
}

public class Choice
{
    public string Description { get; set; }
    public string NextSceneId { get; set; }
}