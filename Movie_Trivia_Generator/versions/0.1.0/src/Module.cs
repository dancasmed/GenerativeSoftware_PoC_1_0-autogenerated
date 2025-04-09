using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MovieTriviaGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Movie Trivia Generator";
    private List<string> _movieTriviaList;
    private List<string> _tvShowTriviaList;
    private Random _random;

    public MovieTriviaGenerator()
    {
        _random = new Random();
        _movieTriviaList = new List<string>
        {
            "The famous shower scene in 'Psycho' (1960) used chocolate syrup as fake blood because it showed up better in black and white.",
            "The Wilhelm Scream, a famous sound effect, has been used in over 200 movies, including 'Star Wars' and 'Indiana Jones'.",
            "The iconic line 'Here's looking at you, kid' from 'Casablanca' (1942) was ad-libbed by Humphrey Bogart.",
            "The spinning top at the end of 'Inception' (2010) was not CGI; it was a practical effect.",
            "The original 'Jurassic Park' (1993) used a mix of animatronics and CGI to bring the dinosaurs to life."
        };

        _tvShowTriviaList = new List<string>
        {
            "The 'Friends' cast negotiated their salaries together, ensuring all six main actors were paid equally.",
            "The famous 'Red Wedding' episode in 'Game of Thrones' was so intense that some actors needed therapy after filming.",
            "The 'Breaking Bad' episode 'Ozymandias' holds a perfect 10/10 rating on IMDb, making it one of the highest-rated TV episodes ever.",
            "The 'Stranger Things' character Eleven was almost named 'Experiment' in early drafts of the script.",
            "The 'The Office' (US) cast improvised many of their lines, especially those delivered by Steve Carell as Michael Scott."
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Movie Trivia Generator...");
        
        try
        {
            string triviaFilePath = Path.Combine(dataFolder, "trivia_history.json");
            List<string> triviaHistory = LoadTriviaHistory(triviaFilePath);
            
            string randomTrivia = GetRandomTrivia();
            Console.WriteLine("Random Trivia: " + randomTrivia);
            
            triviaHistory.Add(randomTrivia);
            SaveTriviaHistory(triviaFilePath, triviaHistory);
            
            Console.WriteLine("Trivia saved to history.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private string GetRandomTrivia()
    {
        int choice = _random.Next(2);
        if (choice == 0)
        {
            int index = _random.Next(_movieTriviaList.Count);
            return "[MOVIE] " + _movieTriviaList[index];
        }
        else
        {
            int index = _random.Next(_tvShowTriviaList.Count);
            return "[TV SHOW] " + _tvShowTriviaList[index];
        }
    }

    private List<string> LoadTriviaHistory(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        return new List<string>();
    }

    private void SaveTriviaHistory(string filePath, List<string> triviaHistory)
    {
        string json = JsonSerializer.Serialize(triviaHistory);
        File.WriteAllText(filePath, json);
    }
}