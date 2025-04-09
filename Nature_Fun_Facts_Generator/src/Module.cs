using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class NatureFunFactsGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Nature Fun Facts Generator";
    private List<string> _funFacts;
    private Random _random;

    public NatureFunFactsGenerator()
    {
        _random = new Random();
        _funFacts = new List<string>
        {
            "A single oak tree can produce about 10 million acorns in its lifetime.",
            "The Amazon Rainforest produces 20% of the world's oxygen.",
            "Bananas are berries, but strawberries are not.",
            "Honey never spoils. Archaeologists have found pots of honey in ancient Egyptian tombs that are over 3,000 years old and still perfectly good to eat.",
            "Cows have best friends and can become stressed when they are separated.",
            "A group of flamingos is called a 'flamboyance'.",
            "The heart of a shrimp is located in its head.",
            "Octopuses have three hearts and blue blood.",
            "A single strand of spider silk is stronger than a steel wire of the same thickness.",
            "Polar bears have black skin under their white fur to better absorb the sun's warmth."
        };
    }

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Generating a random fun fact about nature...");
            
            string fact = GetRandomFunFact();
            Console.WriteLine(fact);
            
            string filePath = Path.Combine(dataFolder, "nature_fun_facts.json");
            SaveFunFactToFile(fact, filePath);
            
            Console.WriteLine("Fun fact saved to " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private string GetRandomFunFact()
    {
        int index = _random.Next(_funFacts.Count);
        return _funFacts[index];
    }

    private void SaveFunFactToFile(string fact, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(new { FunFact = fact, GeneratedAt = DateTime.Now }, options);
        File.WriteAllText(filePath, jsonString);
    }
}