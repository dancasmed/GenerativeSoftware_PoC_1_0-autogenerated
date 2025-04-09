using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MotivationalQuoteGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Motivational Quote Generator";
    private List<string> _quotes;
    private Random _random;

    public MotivationalQuoteGenerator()
    {
        _random = new Random();
        _quotes = new List<string>
        {
            "Believe you can and you're halfway there.",
            "The only way to do great work is to love what you do.",
            "Don't watch the clock; do what it does. Keep going.",
            "The future belongs to those who believe in the beauty of their dreams.",
            "Success is not final, failure is not fatal: It is the courage to continue that counts.",
            "You are never too old to set another goal or to dream a new dream.",
            "The harder you work for something, the greater you'll feel when you achieve it.",
            "It always seems impossible until it's done.",
            "Your limitation—it's only your imagination.",
            "Push yourself, because no one else is going to do it for you."
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Motivational Quote Generator...");
        
        string quotesFilePath = Path.Combine(dataFolder, "quotes.json");
        
        try
        {
            if (File.Exists(quotesFilePath))
            {
                string json = File.ReadAllText(quotesFilePath);
                _quotes = JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                string json = JsonSerializer.Serialize(_quotes);
                File.WriteAllText(quotesFilePath, json);
            }
            
            string randomQuote = GetRandomQuote();
            Console.WriteLine("Here's your motivational quote for today:");
            Console.WriteLine(randomQuote);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private string GetRandomQuote()
    {
        int index = _random.Next(_quotes.Count);
        return _quotes[index];
    }
}