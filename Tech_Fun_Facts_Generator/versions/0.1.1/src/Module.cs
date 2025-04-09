using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TechFunFactsGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Tech Fun Facts Generator";
    private readonly List<string> _funFacts;
    private string _dataFilePath;

    public TechFunFactsGenerator()
    {
        _funFacts = new List<string>
        {
            "The first computer virus was created in 1983 and was called 'Elk Cloner'.",
            "The QWERTY keyboard layout was designed to slow typists down to prevent jamming mechanical typewriters.",
            "The first 1GB hard drive was released in 1980 and weighed over 500 pounds.",
            "The term 'bug' in computing was coined when a real moth caused a malfunction in the Harvard Mark II computer.",
            "The Apollo 11 guidance computer had less processing power than a modern calculator.",
            "The first webcam was used to monitor a coffee pot at Cambridge University.",
            "CAPTCHA stands for 'Completely Automated Public Turing test to tell Computers and Humans Apart'.",
            "The average person spends about 6 years and 8 months of their life on social media.",
            "The first electronic computer ENIAC weighed more than 27 tons and took up 1800 square feet.",
            "The '@' symbol in email addresses was chosen because it was rarely used in computing at the time."
        };
        _dataFilePath = string.Empty;
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "tech_fun_facts.json");
        
        Console.WriteLine("Tech Fun Facts Generator is running...");
        Console.WriteLine("Generating a random tech fun fact...");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var random = new Random();
            int index = random.Next(_funFacts.Count);
            string randomFact = _funFacts[index];
            
            Console.WriteLine("Random Tech Fun Fact: " + randomFact);
            
            var factsData = new
            {
                GeneratedOn = DateTime.Now,
                FunFact = randomFact
            };
            
            string jsonData = JsonSerializer.Serialize(factsData);
            File.WriteAllText(_dataFilePath, jsonData);
            
            Console.WriteLine("Fun fact has been saved to: " + _dataFilePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}