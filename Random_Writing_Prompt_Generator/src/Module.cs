using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RandomWritingPromptGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Random Writing Prompt Generator";
    
    private List<string> _prompts;
    private Random _random;
    
    public RandomWritingPromptGenerator()
    {
        _random = new Random();
        _prompts = new List<string>
        {
            "Write a story about a character who discovers a hidden door in their home.",
            "Describe a world where colors have different meanings than they do in our world.",
            "Write a dialogue between two characters who are meeting for the first time in an unusual setting.",
            "Create a story that begins with the sentence: 'The last thing I expected to find in the attic was...'",
            "Imagine a society where dreams are traded as currency. What happens when someone runs out?",
            "Write a letter from the perspective of an inanimate object.",
            "Describe a day in the life of a character who can see five minutes into the future.",
            "Write a story that takes place entirely in one room, but feels as vast as an entire world."
        };
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Random Writing Prompt Generator is running...");
        
        string promptsFilePath = Path.Combine(dataFolder, "writing_prompts.json");
        
        try
        {
            if (File.Exists(promptsFilePath))
            {
                string json = File.ReadAllText(promptsFilePath);
                _prompts = JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                Directory.CreateDirectory(dataFolder);
                string json = JsonSerializer.Serialize(_prompts);
                File.WriteAllText(promptsFilePath, json);
            }
            
            string randomPrompt = GetRandomPrompt();
            Console.WriteLine("Your writing prompt: " + randomPrompt);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private string GetRandomPrompt()
    {
        int index = _random.Next(_prompts.Count);
        return _prompts[index];
    }
}