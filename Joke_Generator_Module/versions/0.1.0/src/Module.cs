using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class JokeGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Joke Generator Module";
    private List<string> _jokes;
    private Random _random;

    public JokeGeneratorModule()
    {
        _random = new Random();
        _jokes = new List<string>
        {
            "Why don't scientists trust atoms? Because they make up everything!",
            "Why did the scarecrow win an award? Because he was outstanding in his field!",
            "Why don't skeletons fight each other? They don't have the guts!",
            "Why couldn't the bicycle stand up by itself? It was two tired!",
            "What do you call fake spaghetti? An impasta!",
            "How do you organize a space party? You planet!",
            "Why did the math book look sad? Because it had too many problems.",
            "What did one wall say to the other wall? I'll meet you at the corner!",
            "Why don't eggs tell jokes? They'd crack each other up!",
            "How do you make a tissue dance? Put a little boogie in it!"
        };
    }

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Joke Generator Module is running...");
            Console.WriteLine("Generating a random joke for you:");
            
            string joke = GetRandomJoke();
            Console.WriteLine(joke);
            
            string jokesFilePath = Path.Combine(dataFolder, "jokes_history.json");
            SaveJokeToFile(joke, jokesFilePath);
            
            Console.WriteLine("Joke has been saved to the history file.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the joke: " + ex.Message);
            return false;
        }
    }

    private string GetRandomJoke()
    {
        int index = _random.Next(_jokes.Count);
        return _jokes[index];
    }

    private void SaveJokeToFile(string joke, string filePath)
    {
        List<string> jokesHistory = new List<string>();
        
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            jokesHistory = JsonSerializer.Deserialize<List<string>>(existingJson);
        }
        
        jokesHistory.Add(joke);
        string updatedJson = JsonSerializer.Serialize(jokesHistory);
        File.WriteAllText(filePath, updatedJson);
    }
}