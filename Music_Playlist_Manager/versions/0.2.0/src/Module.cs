using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class MusicPlaylistManager : IGeneratedModule
{
    public string Name { get; set; } = "Music Playlist Manager";
    private List<string> _playlist;
    private string _playlistFilePath;

    public MusicPlaylistManager()
    {
        _playlist = new List<string>();
    }

    public bool Main(string dataFolder)
    {
        _playlistFilePath = Path.Combine(dataFolder, "playlist.json");
        Console.WriteLine("Music Playlist Manager is running.");
        LoadPlaylist();

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add song");
            Console.WriteLine("2. Remove song");
            Console.WriteLine("3. Shuffle playlist");
            Console.WriteLine("4. View playlist");
            Console.WriteLine("5. Exit");

            Console.Write("Enter your choice: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddSong();
                    break;
                case 2:
                    RemoveSong();
                    break;
                case 3:
                    ShufflePlaylist();
                    break;
                case 4:
                    ViewPlaylist();
                    break;
                case 5:
                    SavePlaylist();
                    Console.WriteLine("Exiting Music Playlist Manager.");
                    return true;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                    break;
            }
        }
    }

    private void LoadPlaylist()
    {
        if (File.Exists(_playlistFilePath))
        {
            try
            {
                string json = File.ReadAllText(_playlistFilePath);
                _playlist = JsonSerializer.Deserialize<List<string>>(json);
                Console.WriteLine("Playlist loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading playlist: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("No existing playlist found. A new playlist will be created.");
        }
    }

    private void SavePlaylist()
    {
        try
        {
            string json = JsonSerializer.Serialize(_playlist);
            File.WriteAllText(_playlistFilePath, json);
            Console.WriteLine("Playlist saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving playlist: " + ex.Message);
        }
    }

    private void AddSong()
    {
        Console.Write("Enter the name of the song to add: ");
        string song = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(song))
        {
            Console.WriteLine("Song name cannot be empty.");
            return;
        }

        _playlist.Add(song);
        Console.WriteLine("Song added successfully.");
    }

    private void RemoveSong()
    {
        if (_playlist.Count == 0)
        {
            Console.WriteLine("The playlist is empty.");
            return;
        }

        Console.WriteLine("Current playlist:");
        for (int i = 0; i < _playlist.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + _playlist[i]);
        }

        Console.Write("Enter the number of the song to remove: ");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out int songNumber) || songNumber < 1 || songNumber > _playlist.Count)
        {
            Console.WriteLine("Invalid input. Please enter a valid song number.");
            return;
        }

        string removedSong = _playlist[songNumber - 1];
        _playlist.RemoveAt(songNumber - 1);
        Console.WriteLine("Song '" + removedSong + "' removed successfully.");
    }

    private void ShufflePlaylist()
    {
        if (_playlist.Count == 0)
        {
            Console.WriteLine("The playlist is empty.");
            return;
        }

        Random rng = new Random();
        _playlist = _playlist.OrderBy(x => rng.Next()).ToList();
        Console.WriteLine("Playlist shuffled successfully.");
    }

    private void ViewPlaylist()
    {
        if (_playlist.Count == 0)
        {
            Console.WriteLine("The playlist is empty.");
            return;
        }

        Console.WriteLine("Current playlist:");
        for (int i = 0; i < _playlist.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + _playlist[i]);
        }
    }
}