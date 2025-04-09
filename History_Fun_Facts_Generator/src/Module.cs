using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HistoryFunFactsGenerator : IGeneratedModule
{
    public string Name { get; set; } = "History Fun Facts Generator";
    private readonly List<string> _funFacts;
    private readonly Random _random;

    public HistoryFunFactsGenerator()
    {
        _random = new Random();
        _funFacts = new List<string>
        {
            "The Great Pyramid of Giza was the tallest man-made structure for over 3,800 years.",
            "Cleopatra lived closer in time to the Moon landing than to the construction of the Great Pyramid.",
            "The shortest war in history was between Britain and Zanzibar in 1896. Zanzibar surrendered after 38 minutes.",
            "The ancient Romans used urine to whiten their teeth.",
            "The Titanic's distress signals were ignored by a nearby ship because the radio operator was off duty.",
            "Napoleon was once attacked by a horde of bunnies.",
            "The first recorded Olympic Games took place in 776 BCE.",
            "Vikings used the bones of slain animals to skate on ice.",
            "The Library of Alexandria was one of the largest and most significant libraries of the ancient world.",
            "The Hundred Years' War actually lasted 116 years (1337-1453)."
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("History Fun Facts Generator is running...");
        
        try
        {
            string filePath = Path.Combine(dataFolder, "fun_facts_history.json");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            string randomFact = GetRandomFunFact();
            Console.WriteLine("Random History Fun Fact: " + randomFact);
            
            var factsData = new
            {
                GeneratedOn = DateTime.Now,
                FunFact = randomFact
            };
            
            string jsonData = JsonSerializer.Serialize(factsData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
            
            Console.WriteLine("Fun fact saved to: " + filePath);
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
}