using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class QuoteOfTheDayModule : IGeneratedModule
{
    public string Name { get; set; } = "Quote of the Day";
    
    private List<string> _quotes = new List<string>
    {
        "The only limit to our realization of tomorrow is our doubts of today. - Franklin D. Roosevelt",
        "Life is what happens when you're busy making other plans. - John Lennon",
        "The way to get started is to quit talking and begin doing. - Walt Disney",
        "In the end, it's not the years in your life that count. It's the life in your years. - Abraham Lincoln",
        "You only live once, but if you do it right, once is enough. - Mae West"
    };
    
    private string _quotesFilePath;
    
    public bool Main(string dataFolder)
    {
        _quotesFilePath = Path.Combine(dataFolder, "quotes.json");
        
        Console.WriteLine("Quote of the Day Module is running...");
        
        try
        {
            LoadQuotes();
            
            Random random = new Random();
            int index = random.Next(_quotes.Count);
            string quoteOfTheDay = _quotes[index];
            
            Console.WriteLine("Today's Quote:");
            Console.WriteLine(quoteOfTheDay);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LoadQuotes()
    {
        if (File.Exists(_quotesFilePath))
        {
            string json = File.ReadAllText(_quotesFilePath);
            _quotes = JsonSerializer.Deserialize<List<string>>(json);
        }
        else
        {
            SaveQuotes();
        }
    }
    
    private void SaveQuotes()
    {
        string json = JsonSerializer.Serialize(_quotes);
        File.WriteAllText(_quotesFilePath, json);
    }
}