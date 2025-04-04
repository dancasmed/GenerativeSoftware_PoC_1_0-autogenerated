using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MotivationalQuoteGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Daily Motivational Quote Generator";
    private List<string> _quotes;
    private string _quotesFilePath;

    public MotivationalQuoteGenerator()
    {
        _quotes = new List<string>
        {
            "Believe you can and you're halfway there. - Theodore Roosevelt",
            "The only way to do great work is to love what you do. - Steve Jobs",
            "Don't watch the clock; do what it does. Keep going. - Sam Levenson",
            "The future belongs to those who believe in the beauty of their dreams. - Eleanor Roosevelt",
            "Success is not final, failure is not fatal: It is the courage to continue that counts. - Winston Churchill"
        };
    }

    public bool Main(string dataFolder)
    {
        try
        {
            _quotesFilePath = Path.Combine(dataFolder, "quotes.json");
            
            if (File.Exists(_quotesFilePath))
            {
                string json = File.ReadAllText(_quotesFilePath);
                _quotes = JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                string json = JsonSerializer.Serialize(_quotes);
                File.WriteAllText(_quotesFilePath, json);
            }

            Random random = new Random();
            int index = random.Next(_quotes.Count);
            string todaysQuote = _quotes[index];
            
            Console.WriteLine("Today's Motivational Quote:");
            Console.WriteLine(todaysQuote);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the motivational quote: " + ex.Message);
            return false;
        }
    }
}