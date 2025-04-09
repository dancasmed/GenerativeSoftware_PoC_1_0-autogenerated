using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LandmarkTriviaGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Landmark Trivia Generator";
    
    private readonly List<Landmark> _landmarks;
    
    public LandmarkTriviaGenerator()
    {
        _landmarks = new List<Landmark>
        {
            new Landmark("Eiffel Tower", "Paris, France", "Was originally intended to be a temporary installation for the 1889 World's Fair."),
            new Landmark("Statue of Liberty", "New York, USA", "Was a gift from France to the United States in 1886."),
            new Landmark("Great Wall of China", "China", "Is over 13,000 miles long and was built over several dynasties."),
            new Landmark("Pyramids of Giza", "Egypt", "The Great Pyramid was the tallest man-made structure for over 3,800 years."),
            new Landmark("Sydney Opera House", "Sydney, Australia", "Was designed by Danish architect Jørn Utzon and opened in 1973.")
        };
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Landmark Trivia Generator is running...");
        
        try
        {
            string filePath = Path.Combine(dataFolder, "landmark_trivia.json");
            
            // Generate random trivia
            Random random = new Random();
            Landmark randomLandmark = _landmarks[random.Next(_landmarks.Count)];
            
            // Save to JSON file
            string json = JsonSerializer.Serialize(randomLandmark, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            
            Console.WriteLine("Random landmark trivia has been generated and saved to " + filePath);
            Console.WriteLine("Landmark: " + randomLandmark.Name);
            Console.WriteLine("Location: " + randomLandmark.Location);
            Console.WriteLine("Trivia: " + randomLandmark.Trivia);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private class Landmark
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Trivia { get; set; }
        
        public Landmark(string name, string location, string trivia)
        {
            Name = name;
            Location = location;
            Trivia = trivia;
        }
    }
}