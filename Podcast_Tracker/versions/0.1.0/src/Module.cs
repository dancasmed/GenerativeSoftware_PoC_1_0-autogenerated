using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PodcastTracker : IGeneratedModule
{
    public string Name { get; set; } = "Podcast Tracker";
    
    private string _historyFilePath;
    
    public PodcastTracker()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Podcast Tracker module is running...");
        
        _historyFilePath = Path.Combine(dataFolder, "podcast_history.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        var history = LoadHistory();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nPodcast Tracker Menu:");
            Console.WriteLine("1. Add podcast episode");
            Console.WriteLine("2. View listening history");
            Console.WriteLine("3. Exit module");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddPodcastEpisode(history);
                    break;
                case "2":
                    DisplayHistory(history);
                    break;
                case "3":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveHistory(history);
        Console.WriteLine("Podcast Tracker module finished.");
        return true;
    }
    
    private List<PodcastEpisode> LoadHistory()
    {
        if (!File.Exists(_historyFilePath))
        {
            return new List<PodcastEpisode>();
        }
        
        try
        {
            var json = File.ReadAllText(_historyFilePath);
            return JsonSerializer.Deserialize<List<PodcastEpisode>>(json) ?? new List<PodcastEpisode>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading history: " + ex.Message);
            return new List<PodcastEpisode>();
        }
    }
    
    private void SaveHistory(List<PodcastEpisode> history)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(history, options);
            File.WriteAllText(_historyFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving history: " + ex.Message);
        }
    }
    
    private void AddPodcastEpisode(List<PodcastEpisode> history)
    {
        Console.Write("Enter podcast name: ");
        var podcastName = Console.ReadLine();
        
        Console.Write("Enter episode title: ");
        var episodeTitle = Console.ReadLine();
        
        Console.Write("Enter date listened (yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();
        
        if (!DateTime.TryParse(dateInput, out var dateListened))
        {
            dateListened = DateTime.Now;
        }
        
        Console.Write("Enter rating (1-5): ");
        var ratingInput = Console.ReadLine();
        
        if (!int.TryParse(ratingInput, out var rating) || rating < 1 || rating > 5)
        {
            rating = 3;
        }
        
        var episode = new PodcastEpisode
        {
            PodcastName = podcastName,
            EpisodeTitle = episodeTitle,
            DateListened = dateListened,
            Rating = rating
        };
        
        history.Add(episode);
        Console.WriteLine("Episode added successfully!");
    }
    
    private void DisplayHistory(List<PodcastEpisode> history)
    {
        if (history.Count == 0)
        {
            Console.WriteLine("No podcast episodes in history yet.");
            return;
        }
        
        Console.WriteLine("\nPodcast Listening History:");
        foreach (var episode in history)
        {
            Console.WriteLine($"{episode.DateListened:yyyy-MM-dd} - {episode.PodcastName}: {episode.EpisodeTitle} (Rating: {episode.Rating}/5)");
        }
    }
}

public class PodcastEpisode
{
    public string PodcastName { get; set; }
    public string EpisodeTitle { get; set; }
    public DateTime DateListened { get; set; }
    public int Rating { get; set; }
}