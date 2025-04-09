using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ArtistTriviaGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Artist Trivia Generator";

    private List<Artist> _artists;
    private Random _random;

    public ArtistTriviaGenerator()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Artist Trivia Generator is running...");

        string dataFilePath = Path.Combine(dataFolder, "artists.json");
        
        if (!File.Exists(dataFilePath))
        {
            InitializeDefaultArtists(dataFilePath);
        }

        try
        {
            string jsonData = File.ReadAllText(dataFilePath);
            _artists = JsonSerializer.Deserialize<List<Artist>>(jsonData);

            if (_artists == null || _artists.Count == 0)
            {
                Console.WriteLine("No artist data available.");
                return false;
            }

            Artist randomArtist = _artists[_random.Next(_artists.Count)];
            string trivia = GenerateTrivia(randomArtist);
            
            Console.WriteLine("Here's your artist trivia:");
            Console.WriteLine(trivia);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating trivia: " + ex.Message);
            return false;
        }
    }

    private string GenerateTrivia(Artist artist)
    {
        string[] triviaTemplates = 
        {
            "{0} was born in {1} and is known for {2}.",
            "Did you know? {0} created famous works like {2}.",
            "The artist {0}, born in {1}, revolutionized art with techniques like {2}."
        };

        string template = triviaTemplates[_random.Next(triviaTemplates.Length)];
        return string.Format(template, artist.Name, artist.Birthplace, artist.KnownFor);
    }

    private void InitializeDefaultArtists(string filePath)
    {
        _artists = new List<Artist>
        {
            new Artist { Name = "Vincent van Gogh", Birthplace = "Netherlands", KnownFor = "The Starry Night" },
            new Artist { Name = "Pablo Picasso", Birthplace = "Spain", KnownFor = "cubism" },
            new Artist { Name = "Leonardo da Vinci", Birthplace = "Italy", KnownFor = "Mona Lisa" },
            new Artist { Name = "Frida Kahlo", Birthplace = "Mexico", KnownFor = "self-portraits" },
            new Artist { Name = "Michelangelo", Birthplace = "Italy", KnownFor = "the Sistine Chapel ceiling" }
        };

        string jsonData = JsonSerializer.Serialize(_artists);
        File.WriteAllText(filePath, jsonData);
    }
}

public class Artist
{
    public string Name { get; set; }
    public string Birthplace { get; set; }
    public string KnownFor { get; set; }
}