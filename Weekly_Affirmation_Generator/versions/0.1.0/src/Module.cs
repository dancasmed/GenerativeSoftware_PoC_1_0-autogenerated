using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WeeklyAffirmationGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Affirmation Generator";
    private List<string> _affirmations;
    private Random _random;

    public WeeklyAffirmationGenerator()
    {
        _random = new Random();
        _affirmations = new List<string>
        {
            "You are capable of achieving great things.",
            "Every day is a new opportunity to grow.",
            "You have the strength to overcome any challenge.",
            "Believe in yourself and your abilities.",
            "Your potential is limitless.",
            "You are worthy of success and happiness.",
            "Stay positive and good things will come.",
            "You are making progress every single day.",
            "Your hard work will pay off.",
            "You are surrounded by love and support."
        };
    }

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Weekly Affirmation Generator is running...");
            
            string affirmationsFile = Path.Combine(dataFolder, "affirmations.json");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            if (File.Exists(affirmationsFile))
            {
                string json = File.ReadAllText(affirmationsFile);
                _affirmations = JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                string json = JsonSerializer.Serialize(_affirmations);
                File.WriteAllText(affirmationsFile, json);
            }

            string weeklyAffirmation = GetRandomAffirmation();
            Console.WriteLine("Your weekly affirmation: " + weeklyAffirmation);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private string GetRandomAffirmation()
    {
        int index = _random.Next(_affirmations.Count);
        return _affirmations[index];
    }
}