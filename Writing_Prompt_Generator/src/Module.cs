using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WritingPromptGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Writing Prompt Generator";
    
    private readonly List<string> _subjects = new List<string>
    {
        "a mysterious stranger", "an abandoned house", "a lost letter", 
        "a hidden treasure", "a forgotten memory", "a secret society"
    };
    
    private readonly List<string> _actions = new List<string>
    {
        "discovers", "uncovers", "creates", "destroys", "hides", "searches for"
    };
    
    private readonly List<string> _settings = new List<string>
    {
        "in a small coastal town", "during a thunderstorm", "on a distant planet", 
        "in the year 2150", "in a dream world", "in a parallel universe"
    };
    
    private readonly Random _random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Writing Prompt Generator is running...");
        Console.WriteLine("Generating creative writing prompts:");
        
        try
        {
            string prompt = GeneratePrompt();
            Console.WriteLine(prompt);
            
            SavePromptToFile(dataFolder, prompt);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating writing prompt: " + ex.Message);
            return false;
        }
    }
    
    private string GeneratePrompt()
    {
        string subject = _subjects[_random.Next(_subjects.Count)];
        string action = _actions[_random.Next(_actions.Count)];
        string setting = _settings[_random.Next(_settings.Count)];
        
        return $"Write a story about {subject} who {action} something unexpected {setting}.";
    }
    
    private void SavePromptToFile(string dataFolder, string prompt)
    {
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        string filePath = Path.Combine(dataFolder, "writing_prompts.json");
        
        List<string> prompts = new List<string>();
        
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            prompts = JsonSerializer.Deserialize<List<string>>(json);
        }
        
        prompts.Add(prompt);
        
        string updatedJson = JsonSerializer.Serialize(prompts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, updatedJson);
    }
}