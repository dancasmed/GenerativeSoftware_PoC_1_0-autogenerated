using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RiddleGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Daily Riddle Generator";
    
    private List<Riddle> _riddles;
    private string _riddlesFilePath;
    
    public RiddleGeneratorModule()
    {
        _riddles = new List<Riddle>
        {
            new Riddle("What has keys but can't open locks?", "A piano"),
            new Riddle("What has to be broken before you can use it?", "An egg"),
            new Riddle("I'm tall when I'm young, and I'm short when I'm old. What am I?", "A candle"),
            new Riddle("What month of the year has 28 days?", "All of them"),
            new Riddle("What is full of holes but still holds water?", "A sponge")
        };
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Daily Riddle Generator Module is running...");
        
        _riddlesFilePath = Path.Combine(dataFolder, "riddles.json");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            if (File.Exists(_riddlesFilePath))
            {
                string json = File.ReadAllText(_riddlesFilePath);
                _riddles = JsonSerializer.Deserialize<List<Riddle>>(json);
            }
            else
            {
                SaveRiddles();
            }
            
            DisplayRandomRiddle();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void DisplayRandomRiddle()
    {
        Random random = new Random();
        int index = random.Next(_riddles.Count);
        Riddle riddle = _riddles[index];
        
        Console.WriteLine("Here's your daily riddle:");
        Console.WriteLine(riddle.Question);
        Console.WriteLine("Think about it... (Press any key to see the answer)");
        Console.ReadKey();
        Console.WriteLine("The answer is: " + riddle.Answer);
    }
    
    private void SaveRiddles()
    {
        string json = JsonSerializer.Serialize(_riddles);
        File.WriteAllText(_riddlesFilePath, json);
    }
}

public class Riddle
{
    public string Question { get; set; }
    public string Answer { get; set; }
    
    public Riddle(string question, string answer)
    {
        Question = question;
        Answer = answer;
    }
}