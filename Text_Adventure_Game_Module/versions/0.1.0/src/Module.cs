using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TextAdventureModule : IGeneratedModule
{
    public string Name { get; set; } = "Text Adventure Game Module";
    
    private string _saveFilePath;
    private GameState _currentState;
    
    public bool Main(string dataFolder)
    {
        _saveFilePath = Path.Combine(dataFolder, "adventure_save.json");
        
        Console.WriteLine("Welcome to the Text Adventure Game!");
        Console.WriteLine("Your choices will shape your destiny...");
        
        if (File.Exists(_saveFilePath))
        {
            Console.WriteLine("Loading saved game...");
            try
            {
                string json = File.ReadAllText(_saveFilePath);
                _currentState = JsonSerializer.Deserialize<GameState>(json);
                Console.WriteLine("Game loaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load save file. Starting new game.");
                Console.WriteLine("Error: " + ex.Message);
                _currentState = new GameState();
            }
        }
        else
        {
            _currentState = new GameState();
            Console.WriteLine("Starting a new adventure!");
        }
        
        PlayGame();
        
        return true;
    }
    
    private void PlayGame()
    {
        bool gameRunning = true;
        
        while (gameRunning)
        {
            DisplayCurrentScene();
            
            Console.WriteLine("\nWhat will you do?");
            for (int i = 0; i < _currentState.CurrentScene.Choices.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + _currentState.CurrentScene.Choices[i].Description);
            }
            
            int choice = GetPlayerChoice(_currentState.CurrentScene.Choices.Length);
            
            if (choice == -1)
            {
                SaveGame();
                Console.WriteLine("Game saved. Goodbye!");
                gameRunning = false;
                continue;
            }
            
            var selectedChoice = _currentState.CurrentScene.Choices[choice - 1];
            _currentState.CurrentScene = selectedChoice.NextScene;
            _currentState.PlayerHealth += selectedChoice.HealthChange;
            _currentState.PlayerScore += selectedChoice.ScoreChange;
            
            if (_currentState.PlayerHealth <= 0)
            {
                Console.WriteLine("\nYou have perished in your adventure! Game over.");
                Console.WriteLine("Final Score: " + _currentState.PlayerScore);
                gameRunning = false;
            }
            else if (_currentState.CurrentScene.IsEndScene)
            {
                Console.WriteLine("\n" + _currentState.CurrentScene.Description);
                Console.WriteLine("You've reached the end of your adventure!");
                Console.WriteLine("Final Score: " + _currentState.PlayerScore);
                gameRunning = false;
            }
        }
    }
    
    private void DisplayCurrentScene()
    {
        Console.WriteLine("\n---");
        Console.WriteLine("Health: " + _currentState.PlayerHealth);
        Console.WriteLine("Score: " + _currentState.PlayerScore);
        Console.WriteLine("\n" + _currentState.CurrentScene.Description);
    }
    
    private int GetPlayerChoice(int maxChoices)
    {
        while (true)
        {
            Console.Write("Enter your choice (1-" + maxChoices + ") or 'S' to save and quit: ");
            string input = Console.ReadLine().Trim().ToUpper();
            
            if (input == "S")
            {
                return -1;
            }
            
            if (int.TryParse(input, out int result) && result >= 1 && result <= maxChoices)
            {
                return result;
            }
            
            Console.WriteLine("Invalid input. Please try again.");
        }
    }
    
    private void SaveGame()
    {
        try
        {
            string json = JsonSerializer.Serialize(_currentState);
            File.WriteAllText(_saveFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to save game: " + ex.Message);
        }
    }
}

public class GameState
{
    public int PlayerHealth { get; set; } = 100;
    public int PlayerScore { get; set; } = 0;
    public Scene CurrentScene { get; set; }
    
    public GameState()
    {
        InitializeScenes();
        CurrentScene = GetStartingScene();
    }
    
    private void InitializeScenes()
    {
        // Create all scenes
        var scene1 = new Scene
        {
            Description = "You stand at the entrance of a dark cave. The air is damp and cool. " +
                          "Torches flicker weakly on the walls, casting long shadows.",
            IsEndScene = false
        };
        
        var scene2 = new Scene
        {
            Description = "You enter the cave cautiously. The floor is uneven and slippery. " +
                          "You hear distant dripping water and what might be... scratching sounds?",
            IsEndScene = false
        };
        
        var scene3 = new Scene
        {
            Description = "You turn back from the cave entrance. As you walk away, you notice " +
                          "a glint of metal in the bushes nearby.",
            IsEndScene = false
        };
        
        var scene4 = new Scene
        {
            Description = "You investigate the scratching sounds deeper in the cave. Suddenly, " +
                          "a swarm of bats flies past you! You barely avoid injury.",
            IsEndScene = false
        };
        
        var scene5 = new Scene
        {
            Description = "You examine the glint in the bushes and find an ancient sword! " +
                          "This could be valuable or useful in future adventures.",
            IsEndScene = false
        };
        
        var scene6 = new Scene
        {
            Description = "You press on through the cave and find a hidden treasure chamber! " +
                          "Gold coins and jewels glitter in the dim light.",
            IsEndScene = true
        };
        
        var scene7 = new Scene
        {
            Description = "As you move deeper into the cave, the floor gives way! You fall into " +
                          "a pit and cannot climb out. Your adventure ends here.",
            IsEndScene = true
        };
        
        // Set up choices
        scene1.Choices = new[]
        {
            new Choice { Description = "Enter the cave", NextScene = scene2, HealthChange = 0, ScoreChange = 10 },
            new Choice { Description = "Turn back and explore the area", NextScene = scene3, HealthChange = 0, ScoreChange = 5 }
        };
        
        scene2.Choices = new[]
        {
            new Choice { Description = "Investigate the scratching sounds", NextScene = scene4, HealthChange = -10, ScoreChange = 15 },
            new Choice { Description = "Proceed carefully deeper into the cave", NextScene = scene6, HealthChange = 0, ScoreChange = 20 },
            new Choice { Description = "Turn back to the entrance", NextScene = scene1, HealthChange = 0, ScoreChange = 0 }
        };
        
        scene3.Choices = new[]
        {
            new Choice { Description = "Investigate the glint in the bushes", NextScene = scene5, HealthChange = 0, ScoreChange = 25 },
            new Choice { Description = "Return to the cave entrance", NextScene = scene1, HealthChange = 0, ScoreChange = 0 }
        };
        
        scene4.Choices = new[]
        {
            new Choice { Description = "Continue deeper into the cave", NextScene = scene6, HealthChange = 0, ScoreChange = 30 },
            new Choice { Description = "Retreat to the cave entrance", NextScene = scene1, HealthChange = 0, ScoreChange = -5 }
        };
        
        scene5.Choices = new[]
        {
            new Choice { Description = "Take the sword and enter the cave", NextScene = scene2, HealthChange = 0, ScoreChange = 10 },
            new Choice { Description = "Leave the sword and return home", NextScene = null, HealthChange = 0, ScoreChange = 15, IsEnding = true }
        };
    }
    
    private Scene GetStartingScene()
    {
        return new Scene
        {
            Description = "You stand at the entrance of a dark cave. The air is damp and cool. " +
                          "Torches flicker weakly on the walls, casting long shadows.",
            IsEndScene = false,
            Choices = new[]
            {
                new Choice { Description = "Enter the cave", HealthChange = 0, ScoreChange = 10 },
                new Choice { Description = "Turn back and explore the area", HealthChange = 0, ScoreChange = 5 }
            }
        };
    }
}

public class Scene
{
    public string Description { get; set; }
    public Choice[] Choices { get; set; }
    public bool IsEndScene { get; set; }
}

public class Choice
{
    public string Description { get; set; }
    public Scene NextScene { get; set; }
    public int HealthChange { get; set; }
    public int ScoreChange { get; set; }
    public bool IsEnding { get; set; } = false;
}