using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SocialMediaPostGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Social Media Post Generator";
    
    private readonly List<string> _topics = new List<string>
    {
        "Travel", "Food", "Technology", "Fitness", "Books",
        "Movies", "Music", "Art", "Fashion", "Photography"
    };
    
    private readonly List<string> _verbs = new List<string>
    {
        "Discover", "Explore", "Create", "Share", "Learn",
        "Enjoy", "Master", "Try", "Experience", "Celebrate"
    };
    
    private readonly List<string> _styles = new List<string>
    {
        "Tips and Tricks", "Personal Story", "How-To Guide",
        "Top 10 List", "Behind the Scenes", "Q&A Session"
    };
    
    private readonly Random _random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Generating social media post ideas...");
        
        try
        {
            var ideas = GeneratePostIdeas(5);
            SaveIdeasToFile(ideas, Path.Combine(dataFolder, "post_ideas.json"));
            
            Console.WriteLine("Successfully generated post ideas!");
            Console.WriteLine("Saved to: " + Path.Combine(dataFolder, "post_ideas.json"));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating post ideas: " + ex.Message);
            return false;
        }
    }
    
    private List<string> GeneratePostIdeas(int count)
    {
        var ideas = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            string topic = _topics[_random.Next(_topics.Count)];
            string verb = _verbs[_random.Next(_verbs.Count)];
            string style = _styles[_random.Next(_styles.Count)];
            
            ideas.Add($"{verb} {topic}: {style}");
        }
        
        return ideas;
    }
    
    private void SaveIdeasToFile(List<string> ideas, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(ideas, options);
        File.WriteAllText(filePath, json);
    }
}