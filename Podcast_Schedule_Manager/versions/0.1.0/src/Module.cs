using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PodcastScheduleManager : IGeneratedModule
{
    public string Name { get; set; } = "Podcast Schedule Manager";
    
    private string _scheduleFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Podcast Schedule Manager is running...");
        
        _scheduleFilePath = Path.Combine(dataFolder, "podcast_schedule.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        while (true)
        {
            Console.WriteLine("\nPodcast Schedule Manager");
            Console.WriteLine("1. Add new episode");
            Console.WriteLine("2. View schedule");
            Console.WriteLine("3. Remove episode");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            if (!int.TryParse(input, out var option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }
            
            switch (option)
            {
                case 1:
                    AddEpisode();
                    break;
                case 2:
                    ViewSchedule();
                    break;
                case 3:
                    RemoveEpisode();
                    break;
                case 4:
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private List<PodcastEpisode> LoadSchedule()
    {
        if (!File.Exists(_scheduleFilePath))
        {
            return new List<PodcastEpisode>();
        }
        
        var json = File.ReadAllText(_scheduleFilePath);
        return JsonSerializer.Deserialize<List<PodcastEpisode>>(json);
    }
    
    private void SaveSchedule(List<PodcastEpisode> schedule)
    {
        var json = JsonSerializer.Serialize(schedule, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_scheduleFilePath, json);
    }
    
    private void AddEpisode()
    {
        Console.Write("Enter episode title: ");
        var title = Console.ReadLine();
        
        Console.Write("Enter release date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out var releaseDate))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
            return;
        }
        
        Console.Write("Enter duration (minutes): ");
        if (!int.TryParse(Console.ReadLine(), out var duration))
        {
            Console.WriteLine("Invalid duration. Please enter a number.");
            return;
        }
        
        var schedule = LoadSchedule();
        schedule.Add(new PodcastEpisode
        {
            Title = title,
            ReleaseDate = releaseDate,
            DurationMinutes = duration
        });
        
        SaveSchedule(schedule);
        Console.WriteLine("Episode added successfully.");
    }
    
    private void ViewSchedule()
    {
        var schedule = LoadSchedule();
        
        if (schedule.Count == 0)
        {
            Console.WriteLine("No episodes scheduled.");
            return;
        }
        
        Console.WriteLine("\nScheduled Episodes:");
        foreach (var episode in schedule)
        {
            Console.WriteLine($"{episode.ReleaseDate:yyyy-MM-dd}: {episode.Title} ({episode.DurationMinutes} min)");
        }
    }
    
    private void RemoveEpisode()
    {
        var schedule = LoadSchedule();
        
        if (schedule.Count == 0)
        {
            Console.WriteLine("No episodes to remove.");
            return;
        }
        
        Console.WriteLine("\nSelect an episode to remove:");
        for (int i = 0; i < schedule.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {schedule[i].Title} ({schedule[i].ReleaseDate:yyyy-MM-dd})");
        }
        
        Console.Write("Enter episode number: ");
        if (!int.TryParse(Console.ReadLine(), out var episodeNumber) || episodeNumber < 1 || episodeNumber > schedule.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }
        
        var removedEpisode = schedule[episodeNumber - 1];
        schedule.RemoveAt(episodeNumber - 1);
        SaveSchedule(schedule);
        
        Console.WriteLine($"Removed episode: {removedEpisode.Title}");
    }
}

public class PodcastEpisode
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int DurationMinutes { get; set; }
}