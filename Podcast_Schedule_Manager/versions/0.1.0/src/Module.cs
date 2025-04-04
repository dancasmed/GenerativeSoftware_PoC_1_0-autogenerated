using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PodcastScheduleManager : IGeneratedModule
{
    public string Name { get; set; } = "Podcast Schedule Manager";
    
    private string _scheduleFilePath;
    
    public PodcastScheduleManager()
    {
    }
    
    public bool Main(string dataFolder)
    {
        _scheduleFilePath = Path.Combine(dataFolder, "podcast_schedule.json");
        
        Console.WriteLine("Podcast Schedule Manager is running.");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<PodcastEpisode> episodes = LoadEpisodes();
        
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add new episode");
            Console.WriteLine("2. View all episodes");
            Console.WriteLine("3. Remove episode");
            Console.WriteLine("4. Save and exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddEpisode(episodes);
                    break;
                case "2":
                    ViewEpisodes(episodes);
                    break;
                case "3":
                    RemoveEpisode(episodes);
                    break;
                case "4":
                    SaveEpisodes(episodes);
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private List<PodcastEpisode> LoadEpisodes()
    {
        if (File.Exists(_scheduleFilePath))
        {
            string json = File.ReadAllText(_scheduleFilePath);
            return JsonSerializer.Deserialize<List<PodcastEpisode>>(json);
        }
        
        return new List<PodcastEpisode>();
    }
    
    private void SaveEpisodes(List<PodcastEpisode> episodes)
    {
        string json = JsonSerializer.Serialize(episodes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_scheduleFilePath, json);
        Console.WriteLine("Episodes saved successfully.");
    }
    
    private void AddEpisode(List<PodcastEpisode> episodes)
    {
        Console.Write("Enter episode title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter release date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime releaseDate))
        {
            Console.WriteLine("Invalid date format. Episode not added.");
            return;
        }
        
        episodes.Add(new PodcastEpisode { Title = title, ReleaseDate = releaseDate });
        Console.WriteLine("Episode added successfully.");
    }
    
    private void ViewEpisodes(List<PodcastEpisode> episodes)
    {
        if (episodes.Count == 0)
        {
            Console.WriteLine("No episodes scheduled.");
            return;
        }
        
        Console.WriteLine("\nScheduled Episodes:");
        foreach (var episode in episodes)
        {
            Console.WriteLine($"{episode.Title} - {episode.ReleaseDate:yyyy-MM-dd}");
        }
    }
    
    private void RemoveEpisode(List<PodcastEpisode> episodes)
    {
        ViewEpisodes(episodes);
        
        if (episodes.Count == 0)
        {
            return;
        }
        
        Console.Write("Enter the title of the episode to remove: ");
        string title = Console.ReadLine();
        
        int removed = episodes.RemoveAll(e => e.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        
        if (removed > 0)
        {
            Console.WriteLine("Episode removed successfully.");
        }
        else
        {
            Console.WriteLine("Episode not found.");
        }
    }
}

public class PodcastEpisode
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
}