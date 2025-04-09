using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SpaceExplorationFactsGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Space Exploration Facts Generator";

    private List<string> _facts;
    private Random _random;

    public SpaceExplorationFactsGenerator()
    {
        _random = new Random();
        _facts = new List<string>
        {
            "The first human-made object to reach space was the German V-2 rocket in 1944.",
            "Yuri Gagarin became the first human to orbit Earth on April 12, 1961.",
            "The Hubble Space Telescope was launched in 1990 and has provided some of the most detailed images of distant galaxies.",
            "Voyager 1, launched in 1977, is the farthest human-made object from Earth.",
            "The International Space Station (ISS) orbits Earth at an altitude of approximately 408 kilometers.",
            "Mars has the tallest volcano in the solar system, Olympus Mons, which is about 22 kilometers high.",
            "The Moon is moving away from Earth at a rate of about 3.8 centimeters per year.",
            "A day on Venus is longer than its year; it takes 243 Earth days to rotate once but only 225 Earth days to orbit the Sun.",
            "Saturn's rings are made up of billions of ice particles, some as small as grains of sand and others as large as mountains.",
            "The Sun accounts for about 99.86% of the mass in the solar system."
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Space Exploration Facts Generator is running...");
        
        string factsFilePath = Path.Combine(dataFolder, "space_facts.json");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            if (!File.Exists(factsFilePath))
            {
                SaveFactsToFile(factsFilePath);
                Console.WriteLine("Generated new space facts file.");
            }
            else
            {
                LoadFactsFromFile(factsFilePath);
                Console.WriteLine("Loaded existing space facts file.");
            }

            string randomFact = GetRandomFact();
            Console.WriteLine("Random Space Fact: " + randomFact);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private string GetRandomFact()
    {
        int index = _random.Next(_facts.Count);
        return _facts[index];
    }

    private void SaveFactsToFile(string filePath)
    {
        string json = JsonSerializer.Serialize(_facts);
        File.WriteAllText(filePath, json);
    }

    private void LoadFactsFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        _facts = JsonSerializer.Deserialize<List<string>>(json);
    }
}