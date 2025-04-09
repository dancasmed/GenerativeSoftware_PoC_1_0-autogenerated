using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HistoricalTriviaGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Historical Trivia Generator";
    
    private readonly List<string> _historicalEvents = new List<string>
    {
        "The construction of the Great Pyramid of Giza was completed around 2560 BCE.",
        "The Roman Empire was founded in 27 BCE by Augustus.",
        "The Black Death killed approximately 75-200 million people in Europe during the 14th century.",
        "Christopher Columbus reached the Americas in 1492.",
        "The Industrial Revolution began in Britain in the late 18th century.",
        "World War I started in 1914 after the assassination of Archduke Franz Ferdinand.",
        "The first human to orbit Earth was Yuri Gagarin in 1961.",
        "The Berlin Wall fell in 1989, leading to the reunification of Germany."
    };
    
    private readonly Random _random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Historical Trivia Generator is running...");
        
        try
        {
            string trivia = GenerateRandomTrivia();
            Console.WriteLine("Random Historical Trivia: " + trivia);
            
            string filePath = Path.Combine(dataFolder, "historical_trivia.json");
            SaveTriviaToFile(trivia, filePath);
            
            Console.WriteLine("Trivia has been saved to " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private string GenerateRandomTrivia()
    {
        int index = _random.Next(_historicalEvents.Count);
        return _historicalEvents[index];
    }
    
    private void SaveTriviaToFile(string trivia, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(new { Trivia = trivia, GeneratedAt = DateTime.Now }, options);
        
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, json);
    }
}