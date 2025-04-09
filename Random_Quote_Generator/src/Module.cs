using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RandomQuoteGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Random Quote Generator";
    
    private List<string> _quotes = new List<string>
    {
        "The only way to do great work is to love what you do. - Steve Jobs",
        "Life is what happens when you're busy making other plans. - John Lennon",
        "In the middle of difficulty lies opportunity. - Albert Einstein",
        "The future belongs to those who believe in the beauty of their dreams. - Eleanor Roosevelt",
        "Success is not final, failure is not fatal: It is the courage to continue that counts. - Winston Churchill"
    };
    
    private string _quotesFilePath;
    
    public bool Main(string dataFolder)
    {
        _quotesFilePath = Path.Combine(dataFolder, "quotes.json");
        
        Console.WriteLine("Initializing Random Quote Generator...");
        
        try
        {
            LoadQuotes();
            
            var random = new Random();
            int index = random.Next(_quotes.Count);
            string randomQuote = _quotes[index];
            
            Console.WriteLine("Here's your random quote:");
            Console.WriteLine(randomQuote);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the quote: " + ex.Message);
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