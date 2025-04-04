using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WritingPromptGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Writing Prompt Generator";
    
    private readonly List<string> _prompts = new List<string>
    {
        "Write a story about a character who discovers a hidden door in their home.",
        "Describe a world where time moves backward instead of forward.",
        "Write a dialogue between two strangers who meet on a train.",
        "Create a story where the protagonist has a unique superpower that seems useless at first.",
        "Imagine a society where emotions are bought and sold. How does it function?",
        "Write about a character who receives a mysterious letter with no return address.",
        "Describe a day in the life of a sentient robot living among humans.",
        "Write a story set in a library where the books come to life at night.",
        "Imagine a world where dreams are shared experiences. What happens when a nightmare spreads?",
        "Write about a person who finds an old photograph that changes their life."
    };
    
    private readonly Random _random = new Random();
    
    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Writing Prompt Generator is running...");
            
            string filePath = Path.Combine(dataFolder, "writing_prompts.json");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            string randomPrompt = _prompts[_random.Next(_prompts.Count)];
            
            var promptData = new
            {
                Prompt = randomPrompt,
                GeneratedAt = DateTime.Now
            };
            
            string json = JsonSerializer.Serialize(promptData);
            File.WriteAllText(filePath, json);
            
            Console.WriteLine("A new writing prompt has been generated and saved.");
            Console.WriteLine("Prompt: " + randomPrompt);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}