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
        "The only way to do great work is to love what you do. - Steve Jobs",
        "Life is what happens when you're busy making other plans. - John Lennon",
        "In the end, it's not the years in your life that count. It's the life in your years. - Abraham Lincoln",
        "The future belongs to those who believe in the beauty of their dreams. - Eleanor Roosevelt",
        "Strive not to be a success, but rather to be of value. - Albert Einstein"
    };
    
    private string _quotesFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Quote of the Day Module is running...");
        
        _quotesFilePath = Path.Combine(dataFolder, "quotes.json");
        
        try
        {
            // Load or initialize quotes
            if (File.Exists(_quotesFilePath))
            {
                string json = File.ReadAllText(_quotesFilePath);
                _quotes = JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                SaveQuotes();
            }
            
            // Get random quote
            Random random = new Random();
            int index = random.Next(_quotes.Count);
            string todaysQuote = _quotes[index];
            
            Console.WriteLine("Today's Quote: " + todaysQuote);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void SaveQuotes()
    {
        string json = JsonSerializer.Serialize(_quotes);
        File.WriteAllText(_quotesFilePath, json);
    }
}