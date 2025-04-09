using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CulturalTriviaGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Cultural Trivia Generator";

    private List<CulturalTrivia> _triviaList;
    private Random _random;

    public CulturalTriviaGenerator()
    {
        _random = new Random();
        _triviaList = new List<CulturalTrivia>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Cultural Trivia Generator is running...");
        
        string triviaFilePath = Path.Combine(dataFolder, "cultural_trivia.json");
        
        try
        {
            if (File.Exists(triviaFilePath))
            {
                string jsonData = File.ReadAllText(triviaFilePath);
                _triviaList = JsonSerializer.Deserialize<List<CulturalTrivia>>(jsonData);
            }
            else
            {
                InitializeDefaultTrivia();
                SaveTriviaToFile(triviaFilePath);
            }
            
            if (_triviaList.Count == 0)
            {
                Console.WriteLine("No trivia questions available.");
                return false;
            }
            
            CulturalTrivia randomTrivia = GetRandomTrivia();
            DisplayTrivia(randomTrivia);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void InitializeDefaultTrivia()
    {
        _triviaList = new List<CulturalTrivia>
        {
            new CulturalTrivia { Question = "What is the traditional dance of Brazil?", Answer = "Samba" },
            new CulturalTrivia { Question = "Which country is known as the Land of the Rising Sun?", Answer = "Japan" },
            new CulturalTrivia { Question = "What is the official language of Egypt?", Answer = "Arabic" },
            new CulturalTrivia { Question = "Which festival is known for its colorful powders thrown in the air?", Answer = "Holi" },
            new CulturalTrivia { Question = "What is the traditional clothing of Scotland?", Answer = "Kilt" }
        };
    }

    private void SaveTriviaToFile(string filePath)
    {
        string jsonData = JsonSerializer.Serialize(_triviaList);
        File.WriteAllText(filePath, jsonData);
    }

    private CulturalTrivia GetRandomTrivia()
    {
        int index = _random.Next(_triviaList.Count);
        return _triviaList[index];
    }

    private void DisplayTrivia(CulturalTrivia trivia)
    {
        Console.WriteLine("Cultural Trivia Question:");
        Console.WriteLine(trivia.Question);
        Console.WriteLine("Press any key to reveal the answer...");
        Console.ReadKey();
        Console.WriteLine("Answer: " + trivia.Answer);
    }
}

public class CulturalTrivia
{
    public string Question { get; set; }
    public string Answer { get; set; }
}