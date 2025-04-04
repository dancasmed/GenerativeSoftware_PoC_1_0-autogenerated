using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MusicPlaylistManager : IGeneratedModule
{
    public string Name { get; set; } = "Music Playlist Manager";
    
    private List<Song> _playlist;
    private string _dataFilePath;
    
    public MusicPlaylistManager()
    {
        _playlist = new List<Song>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Music Playlist Manager module is running.");
        
        _dataFilePath = Path.Combine(dataFolder, "playlist.json");
        
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _playlist = JsonSerializer.Deserialize<List<Song>>(json);
                Console.WriteLine("Playlist loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading playlist: " + ex.Message);
                return false;
            }
        }
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddSong();
                    break;
                case "2":
                    RemoveSong();
                    break;
                case "3":
                    DisplayPlaylist();
                    break;
                case "4":
                    CalculateTotalDuration();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        try
        {
            string json = JsonSerializer.Serialize(_playlist);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Playlist saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving playlist: " + ex.Message);
            return false;
        }
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nMusic Playlist Manager");
        Console.WriteLine("1. Add Song");
        Console.WriteLine("2. Remove Song");
        Console.WriteLine("3. Display Playlist");
        Console.WriteLine("4. Calculate Total Duration");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddSong()
    {
        Console.Write("Enter song title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter artist: ");
        string artist = Console.ReadLine();
        
        Console.Write("Enter duration in seconds: ");
        if (int.TryParse(Console.ReadLine(), out int duration))
        {
            _playlist.Add(new Song { Title = title, Artist = artist, Duration = duration });
            Console.WriteLine("Song added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid duration. Please enter a number.");
        }
    }
    
    private void RemoveSong()
    {
        if (_playlist.Count == 0)
        {
            Console.WriteLine("Playlist is empty.");
            return;
        }
        
        DisplayPlaylist();
        Console.Write("Enter the number of the song to remove: ");
        
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _playlist.Count)
        {
            _playlist.RemoveAt(index - 1);
            Console.WriteLine("Song removed successfully.");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }
    
    private void DisplayPlaylist()
    {
        if (_playlist.Count == 0)
        {
            Console.WriteLine("Playlist is empty.");
            return;
        }
        
        Console.WriteLine("\nCurrent Playlist:");
        for (int i = 0; i < _playlist.Count; i++)
        {
            var song = _playlist[i];
            Console.WriteLine(string.Format("{0}. {1} by {2} ({3} seconds)", 
                i + 1, song.Title, song.Artist, song.Duration));
        }
    }
    
    private void CalculateTotalDuration()
    {
        int totalSeconds = 0;
        foreach (var song in _playlist)
        {
            totalSeconds += song.Duration;
        }
        
        TimeSpan duration = TimeSpan.FromSeconds(totalSeconds);
        Console.WriteLine(string.Format("Total playlist duration: {0:D2}:{1:D2}:{2:D2}", 
            duration.Hours, duration.Minutes, duration.Seconds));
    }
}

public class Song
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public int Duration { get; set; }
}