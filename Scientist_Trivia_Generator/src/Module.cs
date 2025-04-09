using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ScientistTriviaGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Scientist Trivia Generator";
    
    private List<Scientist> _scientists;
    private Random _random;
    
    public ScientistTriviaGenerator()
    {
        _random = new Random();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Scientist Trivia Generator...");
        
        string dataFilePath = Path.Combine(dataFolder, "scientists.json");
        
        try
        {
            if (!File.Exists(dataFilePath))
            {
                InitializeDefaultScientists(dataFilePath);
                Console.WriteLine("Default scientist data created.");
            }
            
            string jsonData = File.ReadAllText(dataFilePath);
            _scientists = JsonSerializer.Deserialize<List<Scientist>>(jsonData);
            
            if (_scientists == null || _scientists.Count == 0)
            {
                Console.WriteLine("No scientist data available.");
                return false;
            }
            
            GenerateRandomTrivia();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void InitializeDefaultScientists(string filePath)
    {
        var defaultScientists = new List<Scientist>
        {
            new Scientist(
                "Albert Einstein", 
                "Theoretical Physicist", 
                "Developed the theory of relativity, one of the two pillars of modern physics.",
                new DateTime(1879, 3, 14),
                new DateTime(1955, 4, 18)
            ),
            new Scientist(
                "Marie Curie", 
                "Physicist and Chemist", 
                "Conducted pioneering research on radioactivity and discovered polonium and radium.",
                new DateTime(1867, 11, 7),
                new DateTime(1934, 7, 4)
            ),
            new Scientist(
                "Isaac Newton", 
                "Physicist and Mathematician", 
                "Formulated the laws of motion and universal gravitation.",
                new DateTime(1643, 1, 4),
                new DateTime(1727, 3, 31)
            ),
            new Scientist(
                "Alan Turing", 
                "Computer Scientist", 
                "Pioneered theoretical computer science and formalized the concept of algorithms.",
                new DateTime(1912, 6, 23),
                new DateTime(1954, 6, 7)
            )
        };
        
        string jsonData = JsonSerializer.Serialize(defaultScientists);
        File.WriteAllText(filePath, jsonData);
    }
    
    private void GenerateRandomTrivia()
    {
        int index = _random.Next(_scientists.Count);
        var scientist = _scientists[index];
        
        Console.WriteLine("\n=== Random Scientist Trivia ===");
        Console.WriteLine("Name: " + scientist.Name);
        Console.WriteLine("Field: " + scientist.Field);
        Console.WriteLine("Contribution: " + scientist.Contribution);
        Console.WriteLine("Lifespan: " + scientist.BirthDate.ToString("yyyy-MM-dd") + " to " + 
                          scientist.DeathDate.ToString("yyyy-MM-dd"));
        
        // Generate random fact
        string[] possibleFacts = 
        {
            "Did you know " + scientist.Name + " published their first paper at age " + 
            _random.Next(18, 35) + "?",
            "Interesting fact: " + scientist.Name + " won " + _random.Next(1, 10) + " 
            major awards during their career.",
            "Trivia: " + scientist.Name + "'s work influenced at least " + 
            _random.Next(5, 50) + " subsequent scientific discoveries."
        };
        
        Console.WriteLine("\nRandom Fact: " + possibleFacts[_random.Next(possibleFacts.Length)]);
        Console.WriteLine("\nEnjoy your science trivia!");
    }
}

public class Scientist
{
    public string Name { get; set; }
    public string Field { get; set; }
    public string Contribution { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime DeathDate { get; set; }
    
    public Scientist(string name, string field, string contribution, DateTime birthDate, DateTime deathDate)
    {
        Name = name;
        Field = field;
        Contribution = contribution;
        BirthDate = birthDate;
        DeathDate = deathDate;
    }
}