using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class StoryGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Story Generator Module";

    private readonly List<string> genres = new List<string>
    {
        "Fantasy",
        "Sci-Fi",
        "Mystery",
        "Horror",
        "Romance",
        "Adventure"
    };

    private readonly Dictionary<string, List<string>> storyStarters = new Dictionary<string, List<string>>
    {
        {"Fantasy", new List<string>
            {
                "In a land where magic flowed like rivers, a young apprentice discovered...",
                "The ancient prophecy spoke of a chosen one who would...",
                "Beneath the glowing twin moons, the hidden city of..."
            }
        },
        {"Sci-Fi", new List<string>
            {
                "As the starship entered the uncharted sector, sensors detected...",
                "The AI had been dormant for centuries until today, when...",
                "Quantum entanglement communication brought a message from the future saying..."
            }
        },
        {"Mystery", new List<string>
            {
                "The letter arrived with no return address, containing only...",
                "When the clock struck midnight, the lights in the old mansion...",
                "Detective Morgan had seen many cases, but nothing like the..."
            }
        },
        {"Horror", new List<string>
            {
                "The scratching sound in the walls started exactly at...",
                "No one in town would speak about what happened to the...",
                "The journal entries stopped abruptly after describing the..."
            }
        },
        {"Romance", new List<string>
            {
                "Their eyes met across the crowded ballroom, though they were...",
                "The love letter had been lost for fifty years until...",
                "She swore she'd never fall for a detective, but when..."
            }
        },
        {"Adventure", new List<string>
            {
                "The map was torn in three pieces, each hidden in...",
                "When the volcano erupted, it revealed the entrance to...",
                "The old sailor's tale about the island that moves turned out to be..."
            }
        }
    };

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Story Generator Module is running");
            Console.WriteLine("Available genres:");
            
            for (int i = 0; i < genres.Count; i++)
            {
                Console.WriteLine(genres[i]);
            }
            
            Console.WriteLine("Generating a random story starter...");
            
            Random random = new Random();
            string randomGenre = genres[random.Next(genres.Count)];
            List<string> starters = storyStarters[randomGenre];
            string randomStarter = starters[random.Next(starters.Count)];
            
            Console.WriteLine("Genre: " + randomGenre);
            Console.WriteLine("Story Starter: " + randomStarter);
            
            // Save the generated story to a JSON file
            var storyData = new
            {
                Genre = randomGenre,
                Starter = randomStarter,
                GeneratedAt = DateTime.Now
            };
            
            string jsonData = JsonSerializer.Serialize(storyData);
            string filePath = Path.Combine(dataFolder, "story_data.json");
            File.WriteAllText(filePath, jsonData);
            
            Console.WriteLine("Story data saved to: " + filePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
}