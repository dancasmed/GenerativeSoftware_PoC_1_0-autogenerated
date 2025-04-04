using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TextAdventureModule : IGeneratedModule
{
    public string Name { get; set; } = "Text Adventure Game";

    private class GameState
    {
        public string CurrentSceneId { get; set; }
        public Dictionary<string, int> Inventory { get; set; } = new Dictionary<string, int>();
    }

    private class GameScene
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public List<GameChoice> Choices { get; set; } = new List<GameChoice>();
    }

    private class GameChoice
    {
        public string Text { get; set; }
        public string NextSceneId { get; set; }
        public string ItemReward { get; set; }
        public string ItemRequired { get; set; }
    }

    private Dictionary<string, GameScene> scenes;
    private GameState currentState;
    private string dataFolder;

    public TextAdventureModule()
    {
        scenes = new Dictionary<string, GameScene>
        {
            {
                "start", new GameScene
                {
                    Id = "start",
                    Description = "You wake up in a dark forest. The path splits in two directions.",
                    Choices = new List<GameChoice>
                    {
                        new GameChoice { Text = "Take the left path", NextSceneId = "cave" },
                        new GameChoice { Text = "Take the right path", NextSceneId = "river" },
                        new GameChoice { Text = "Search your pockets", NextSceneId = "pockets" }
                    }
                }
            },
            {
                "cave", new GameScene
                {
                    Id = "cave",
                    Description = "You enter a damp cave. There's a shiny object in the corner.",
                    Choices = new List<GameChoice>
                    {
                        new GameChoice { Text = "Pick up the object", NextSceneId = "object_found", ItemReward = "key" },
                        new GameChoice { Text = "Leave the cave", NextSceneId = "start" }
                    }
                }
            },
            {
                "river", new GameScene
                {
                    Id = "river",
                    Description = "You come to a fast-flowing river. There's a rickety bridge.",
                    Choices = new List<GameChoice>
                    {
                        new GameChoice { Text = "Cross the bridge", NextSceneId = "bridge_crossed" },
                        new GameChoice { Text = "Turn back", NextSceneId = "start" }
                    }
                }
            },
            {
                "pockets", new GameScene
                {
                    Id = "pockets",
                    Description = "You find a small knife in your pocket.",
                    Choices = new List<GameChoice>
                    {
                        new GameChoice { Text = "Continue on your way", NextSceneId = "start", ItemReward = "knife" }
                    }
                }
            },
            {
                "object_found", new GameScene
                {
                    Id = "object_found",
                    Description = "You found a rusty key! Maybe it opens something.",
                    Choices = new List<GameChoice>
                    {
                        new GameChoice { Text = "Return to the forest", NextSceneId = "start" }
                    }
                }
            },
            {
                "bridge_crossed", new GameScene
                {
                    Id = "bridge_crossed",
                    Description = "You made it across safely! There's a locked chest here.",
                    Choices = new List<GameChoice>
                    {
                        new GameChoice { Text = "Try to open the chest", NextSceneId = "chest_opened", ItemRequired = "key" },
                        new GameChoice { Text = "Go back", NextSceneId = "river" }
                    }
                }
            },
            {
                "chest_opened", new GameScene
                {
                    Id = "chest_opened",
                    Description = "The chest contains a treasure map! You win!",
                    Choices = new List<GameChoice>()
                }
            }
        };
    }

    public bool Main(string dataFolder)
    {
        this.dataFolder = dataFolder;
        Console.WriteLine("Starting Text Adventure Game Module");
        Console.WriteLine("--------------------------------");

        LoadGameState();
        if (currentState == null)
        {
            currentState = new GameState { CurrentSceneId = "start" };
        }

        while (true)
        {
            var currentScene = scenes[currentState.CurrentSceneId];
            DisplayScene(currentScene);

            if (currentScene.Choices.Count == 0)
            {
                Console.WriteLine("\nGame Over!");
                SaveGameState();
                return true;
            }

            var choice = GetPlayerChoice(currentScene.Choices.Count);
            if (choice == -1)
            {
                SaveGameState();
                return false;
            }

            ProcessChoice(currentScene.Choices[choice]);
        }
    }

    private void DisplayScene(GameScene scene)
    {
        Console.WriteLine();
        Console.WriteLine(scene.Description);
        Console.WriteLine();

        if (currentState.Inventory.Count > 0)
        {
            Console.WriteLine("Inventory:");
            foreach (var item in currentState.Inventory)
            {
                Console.WriteLine(" - " + item.Key);
            }
            Console.WriteLine();
        }

        for (int i = 0; i < scene.Choices.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + scene.Choices[i].Text);
        }
        Console.WriteLine("0. Save and quit");
    }

    private int GetPlayerChoice(int maxChoices)
    {
        while (true)
        {
            Console.Write("\nYour choice: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int choice))
            {
                if (choice >= 0 && choice <= maxChoices)
                {
                    return choice - 1;
                }
            }

            Console.WriteLine("Invalid choice. Please try again.");
        }
    }

    private void ProcessChoice(GameChoice choice)
    {
        if (!string.IsNullOrEmpty(choice.ItemRequired))
        {
            if (!currentState.Inventory.ContainsKey(choice.ItemRequired))
            {
                Console.WriteLine("\nYou need a " + choice.ItemRequired + " to do that!");
                return;
            }
        }

        if (!string.IsNullOrEmpty(choice.ItemReward))
        {
            if (currentState.Inventory.ContainsKey(choice.ItemReward))
            {
                currentState.Inventory[choice.ItemReward]++;
            }
            else
            {
                currentState.Inventory[choice.ItemReward] = 1;
            }
            Console.WriteLine("\nYou obtained: " + choice.ItemReward);
        }

        currentState.CurrentSceneId = choice.NextSceneId;
    }

    private void LoadGameState()
    {
        try
        {
            string savePath = Path.Combine(dataFolder, "adventure_save.json");
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                currentState = JsonSerializer.Deserialize<GameState>(json);
                Console.WriteLine("Game loaded successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading game: " + ex.Message);
        }
    }

    private void SaveGameState()
    {
        try
        {
            string savePath = Path.Combine(dataFolder, "adventure_save.json");
            string json = JsonSerializer.Serialize(currentState);
            File.WriteAllText(savePath, json);
            Console.WriteLine("Game saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving game: " + ex.Message);
        }
    }
}