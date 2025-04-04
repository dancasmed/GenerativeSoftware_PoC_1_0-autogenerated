using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FictionalCharacterNameGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Fictional Character Name Generator";
    
    private List<string> firstNames;
    private List<string> lastNames;
    private List<string> titles;
    
    public FictionalCharacterNameGenerator()
    {
        firstNames = new List<string> { "Aelar", "Brin", "Cael", "Dain", "Eira", "Fael", "Gwen", "Haldir", "Ilyana", "Jorin" };
        lastNames = new List<string> { "Stormrider", "Blackthorn", "Moonwhisper", "Fireforge", "Shadowbane", "Brightblade", "Duskwalker", "Goldleaf", "Silvershield", "Windrider" };
        titles = new List<string> { "the Brave", "the Wise", "the Cunning", "the Mysterious", "the Relentless", "the Enigmatic", "the Fearless", "the Shadow", "the Lightbringer", "the Wanderer" };
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Fictional Character Name Generator is running...");
        
        string configPath = Path.Combine(dataFolder, "name_generator_config.json");
        
        try
        {
            if (File.Exists(configPath))
            {
                string jsonString = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<NameGeneratorConfig>(jsonString);
                
                if (config != null)
                {
                    if (config.FirstNames != null) firstNames = config.FirstNames;
                    if (config.LastNames != null) lastNames = config.LastNames;
                    if (config.Titles != null) titles = config.Titles;
                }
            }
            
            Random random = new Random();
            
            for (int i = 0; i < 5; i++)
            {
                string firstName = firstNames[random.Next(firstNames.Count)];
                string lastName = lastNames[random.Next(lastNames.Count)];
                string title = titles[random.Next(titles.Count)];
                
                Console.WriteLine("Generated character: " + firstName + " " + lastName + " " + title);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating names: " + ex.Message);
            return false;
        }
    }
    
    private class NameGeneratorConfig
    {
        public List<string> FirstNames { get; set; }
        public List<string> LastNames { get; set; }
        public List<string> Titles { get; set; }
    }
}