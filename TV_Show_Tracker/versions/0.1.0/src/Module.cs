using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TVShowTracker : IGeneratedModule
{
    public string Name { get; set; } = "TV Show Tracker";
    
    private string _dataFilePath;
    
    private List<TVShow> _shows;
    
    public TVShowTracker()
    {
        _shows = new List<TVShow>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("TV Show Tracker module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "tvshows.json");
        
        try
        {
            LoadData();
            
            bool running = true;
            while (running)
            {
                DisplayMenu();
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddShow();
                        break;
                    case "2":
                        AddEpisode();
                        break;
                    case "3":
                        ListShows();
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveData();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _shows = JsonSerializer.Deserialize<List<TVShow>>(json);
        }
    }
    
    private void SaveData()
    {
        string json = JsonSerializer.Serialize(_shows);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nTV Show Tracker Menu:");
        Console.WriteLine("1. Add a new TV show");
        Console.WriteLine("2. Add a watched episode");
        Console.WriteLine("3. List all shows and episodes");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddShow()
    {
        Console.Write("Enter TV show name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter total seasons: ");
        int totalSeasons = int.Parse(Console.ReadLine());
        
        _shows.Add(new TVShow
        {
            Name = name,
            TotalSeasons = totalSeasons,
            Episodes = new List<Episode>()
        });
        
        Console.WriteLine("TV show added successfully.");
    }
    
    private void AddEpisode()
    {
        if (_shows.Count == 0)
        {
            Console.WriteLine("No shows available. Please add a show first.");
            return;
        }
        
        ListShows();
        
        Console.Write("Select show index: ");
        int showIndex = int.Parse(Console.ReadLine()) - 1;
        
        if (showIndex < 0 || showIndex >= _shows.Count)
        {
            Console.WriteLine("Invalid show index.");
            return;
        }
        
        TVShow show = _shows[showIndex];
        
        Console.Write("Enter season number: ");
        int season = int.Parse(Console.ReadLine());
        
        Console.Write("Enter episode number: ");
        int number = int.Parse(Console.ReadLine());
        
        Console.Write("Enter episode title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter your rating (1-10): ");
        int rating = int.Parse(Console.ReadLine());
        
        show.Episodes.Add(new Episode
        {
            Season = season,
            Number = number,
            Title = title,
            Rating = rating,
            WatchedDate = DateTime.Now
        });
        
        Console.WriteLine("Episode added successfully.");
    }
    
    private void ListShows()
    {
        if (_shows.Count == 0)
        {
            Console.WriteLine("No shows available.");
            return;
        }
        
        for (int i = 0; i < _shows.Count; i++)
        {
            TVShow show = _shows[i];
            Console.WriteLine($"{i + 1}. {show.Name} (Seasons: {show.TotalSeasons})");
            
            if (show.Episodes.Count > 0)
            {
                Console.WriteLine("   Watched Episodes:");
                foreach (var episode in show.Episodes)
                {
                    Console.WriteLine($"   - S{episode.Season.ToString().PadLeft(2, '0')}E{episode.Number.ToString().PadLeft(2, '0')}: {episode.Title} (Rating: {episode.Rating}/10)");
                }
            }
            else
            {
                Console.WriteLine("   No episodes watched yet.");
            }
        }
    }
}

public class TVShow
{
    public string Name { get; set; }
    public int TotalSeasons { get; set; }
    public List<Episode> Episodes { get; set; }
}

public class Episode
{
    public int Season { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public int Rating { get; set; }
    public DateTime WatchedDate { get; set; }
}