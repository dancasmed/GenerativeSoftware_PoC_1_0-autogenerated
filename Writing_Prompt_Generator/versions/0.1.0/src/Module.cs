using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WritingPromptGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Writing Prompt Generator";
    private List<string> _prompts;
    private Random _random;
    private string _promptsFilePath;

    public WritingPromptGenerator()
    {
        _random = new Random();
        _prompts = new List<string>()
        {
            "Write a story about a character who finds a mysterious key.",
            "Describe a world where time moves backwards.",
            "Create a dialogue between two people who are lost in a forest.",
            "Write a letter from the perspective of an inanimate object.",
            "Imagine a society where emotions are bought and sold. Describe a day in this world.",
            "Write about a character who wakes up with no memory, only to find a note in their pocket.",
            "Describe a city that exists on the back of a giant creature.",
            "Write a story that begins with the line: 'The last thing I expected to find in the attic was...'",
            "Create a character who can hear people's thoughts but only when they lie.",
            "Write about a journey to a place where the laws of physics don't apply."
        };
    }

    public bool Main(string dataFolder)
    {
        try
        {
            _promptsFilePath = Path.Combine(dataFolder, "writing_prompts.json");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            if (File.Exists(_promptsFilePath))
            {
                string json = File.ReadAllText(_promptsFilePath);
                _prompts = JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                string json = JsonSerializer.Serialize(_prompts);
                File.WriteAllText(_promptsFilePath, json);
            }

            Console.WriteLine("Generating a random writing prompt...");
            string prompt = GetRandomPrompt();
            Console.WriteLine("Your writing prompt: " + prompt);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the writing prompt: " + ex.Message);
            return false;
        }
    }

    private string GetRandomPrompt()
    {
        int index = _random.Next(_prompts.Count);
        return _prompts[index];
    }
}