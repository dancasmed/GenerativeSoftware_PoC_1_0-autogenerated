using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PodcastLibraryManager : IGeneratedModule
{
    public string Name { get; set; } = "Podcast Library Manager";

    private string _dataFilePath;
    private List<Podcast> _podcasts;

    public PodcastLibraryManager()
    {
        _podcasts = new List<Podcast>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Podcast Library Manager...");
        
        _dataFilePath = Path.Combine(dataFolder, "podcast_library.json");
        
        try
        {
            LoadLibrary();
            Console.WriteLine("Podcast library loaded successfully.");
            
            bool exitRequested = false;
            while (!exitRequested)
            {
                DisplayMenu();
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddPodcast();
                        break;
                    case "2":
                        AddEpisode();
                        break;
                    case "3":
                        UpdateListeningProgress();
                        break;
                    case "4":
                        DisplayLibrary();
                        break;
                    case "5":
                        exitRequested = true;
                        Console.WriteLine("Saving changes and exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveLibrary();
            Console.WriteLine("Changes saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void LoadLibrary()
    {
        if (File.Exists(_dataFilePath))
        {
            var json = File.ReadAllText(_dataFilePath);
            _podcasts = JsonSerializer.Deserialize<List<Podcast>>(json) ?? new List<Podcast>();
        }
    }

    private void SaveLibrary()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_podcasts, options);
        File.WriteAllText(_dataFilePath, json);
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPodcast Library Manager");
        Console.WriteLine("1. Add Podcast");
        Console.WriteLine("2. Add Episode to Podcast");
        Console.WriteLine("3. Update Listening Progress");
        Console.WriteLine("4. Display Library");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddPodcast()
    {
        Console.Write("Enter podcast title: ");
        var title = Console.ReadLine();
        
        Console.Write("Enter podcast author: ");
        var author = Console.ReadLine();
        
        _podcasts.Add(new Podcast
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            Episodes = new List<Episode>()
        });
        
        Console.WriteLine("Podcast added successfully.");
    }

    private void AddEpisode()
    {
        if (_podcasts.Count == 0)
        {
            Console.WriteLine("No podcasts available. Please add a podcast first.");
            return;
        }
        
        DisplayPodcasts();
        Console.Write("Select podcast number: ");
        if (int.TryParse(Console.ReadLine(), out int podcastIndex) && podcastIndex >= 1 && podcastIndex <= _podcasts.Count)
        {
            var podcast = _podcasts[podcastIndex - 1];
            
            Console.Write("Enter episode title: ");
            var title = Console.ReadLine();
            
            Console.Write("Enter episode duration (minutes): ");
            if (int.TryParse(Console.ReadLine(), out int duration))
            {
                podcast.Episodes.Add(new Episode
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    DurationMinutes = duration,
                    ProgressMinutes = 0,
                    IsCompleted = false
                });
                
                Console.WriteLine("Episode added successfully.");
            }
            else
            {
                Console.WriteLine("Invalid duration. Please enter a number.");
            }
        }
        else
        {
            Console.WriteLine("Invalid podcast selection.");
        }
    }

    private void UpdateListeningProgress()
    {
        if (_podcasts.Count == 0)
        {
            Console.WriteLine("No podcasts available.");
            return;
        }
        
        DisplayPodcasts();
        Console.Write("Select podcast number: ");
        if (int.TryParse(Console.ReadLine(), out int podcastIndex) && podcastIndex >= 1 && podcastIndex <= _podcasts.Count)
        {
            var podcast = _podcasts[podcastIndex - 1];
            
            if (podcast.Episodes.Count == 0)
            {
                Console.WriteLine("No episodes available for this podcast.");
                return;
            }
            
            DisplayEpisodes(podcast);
            Console.Write("Select episode number: ");
            if (int.TryParse(Console.ReadLine(), out int episodeIndex) && episodeIndex >= 1 && episodeIndex <= podcast.Episodes.Count)
            {
                var episode = podcast.Episodes[episodeIndex - 1];
                
                Console.Write("Enter current progress in minutes: ");
                if (int.TryParse(Console.ReadLine(), out int progress) && progress >= 0 && progress <= episode.DurationMinutes)
                {
                    episode.ProgressMinutes = progress;
                    episode.IsCompleted = (progress == episode.DurationMinutes);
                    Console.WriteLine("Progress updated successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid progress value. Must be between 0 and " + episode.DurationMinutes);
                }
            }
            else
            {
                Console.WriteLine("Invalid episode selection.");
            }
        }
        else
        {
            Console.WriteLine("Invalid podcast selection.");
        }
    }

    private void DisplayLibrary()
    {
        if (_podcasts.Count == 0)
        {
            Console.WriteLine("No podcasts in the library.");
            return;
        }
        
        foreach (var podcast in _podcasts)
        {
            Console.WriteLine("\nPodcast: " + podcast.Title + " by " + podcast.Author);
            if (podcast.Episodes.Count > 0)
            {
                Console.WriteLine("Episodes:");
                foreach (var episode in podcast.Episodes)
                {
                    var status = episode.IsCompleted ? "Completed" : 
                                episode.ProgressMinutes > 0 ? "In Progress (" + episode.ProgressMinutes + "m)" : "Not Started";
                    Console.WriteLine("- " + episode.Title + " (" + episode.DurationMinutes + "m): " + status);
                }
            }
            else
            {
                Console.WriteLine("No episodes available.");
            }
        }
    }

    private void DisplayPodcasts()
    {
        Console.WriteLine("\nAvailable Podcasts:");
        for (int i = 0; i < _podcasts.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + _podcasts[i].Title);
        }
    }

    private void DisplayEpisodes(Podcast podcast)
    {
        Console.WriteLine("\nEpisodes for " + podcast.Title + ":");
        for (int i = 0; i < podcast.Episodes.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + podcast.Episodes[i].Title);
        }
    }
}

public class Podcast
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public List<Episode> Episodes { get; set; }
}

public class Episode
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int DurationMinutes { get; set; }
    public int ProgressMinutes { get; set; }
    public bool IsCompleted { get; set; }
}