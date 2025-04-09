using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TeamNameGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Team Name Generator";
    
    private List<string> adjectives = new List<string>
    {
        "Fierce", "Swift", "Mighty", "Brave", "Golden",
        "Shadow", "Thunder", "Iron", "Wild", "Noble"
    };
    
    private List<string> nouns = new List<string>
    {
        "Lions", "Eagles", "Titans", "Wolves", "Dragons",
        "Phoenix", "Sharks", "Hawks", "Bears", "Storm"
    };
    
    private Random random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Team Name Generator Module is running...");
        Console.WriteLine("Generating random team names:");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "team_names_config.json");
            
            if (File.Exists(configPath))
            {
                string jsonConfig = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<TeamNameConfig>(jsonConfig);
                
                if (config != null && config.Adjectives != null && config.Nouns != null)
                {
                    adjectives = config.Adjectives;
                    nouns = config.Nouns;
                }
            }
            
            for (int i = 0; i < 5; i++)
            {
                string teamName = GenerateTeamName();
                Console.WriteLine(teamName);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating team names: " + ex.Message);
            return false;
        }
    }
    
    private string GenerateTeamName()
    {
        string adjective = adjectives[random.Next(adjectives.Count)];
        string noun = nouns[random.Next(nouns.Count)];
        return adjective + " " + noun;
    }
}

public class TeamNameConfig
{
    public List<string> Adjectives { get; set; }
    public List<string> Nouns { get; set; }
}