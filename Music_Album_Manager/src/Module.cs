using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MusicAlbumManager : IGeneratedModule
{
    public string Name { get; set; } = "Music Album Manager";
    
    private List<Album> _albums;
    private string _dataFilePath;
    
    public MusicAlbumManager()
    {
        _albums = new List<Album>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Music Album Manager module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "albums.json");
        
        try
        {
            LoadAlbums();
            
            bool exitRequested = false;
            while (!exitRequested)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Add Album");
                Console.WriteLine("2. List All Albums");
                Console.WriteLine("3. Search Albums by Artist");
                Console.WriteLine("4. Search Albums by Genre");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddAlbum();
                        break;
                    case "2":
                        ListAlbums();
                        break;
                    case "3":
                        SearchByArtist();
                        break;
                    case "4":
                        SearchByGenre();
                        break;
                    case "5":
                        exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            SaveAlbums();
            Console.WriteLine("Music Album Manager module finished.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LoadAlbums()
    {
        if (File.Exists(_dataFilePath))
        {
            var json = File.ReadAllText(_dataFilePath);
            _albums = JsonSerializer.Deserialize<List<Album>>(json) ?? new List<Album>();
            Console.WriteLine("Loaded " + _albums.Count + " albums from storage.");
        }
        else
        {
            Console.WriteLine("No existing album data found. Starting with empty collection.");
        }
    }
    
    private void SaveAlbums()
    {
        var json = JsonSerializer.Serialize(_albums);
        File.WriteAllText(_dataFilePath, json);
        Console.WriteLine("Saved " + _albums.Count + " albums to storage.");
    }
    
    private void AddAlbum()
    {
        Console.Write("Enter artist name: ");
        var artist = Console.ReadLine();
        
        Console.Write("Enter album title: ");
        var title = Console.ReadLine();
        
        Console.Write("Enter genre: ");
        var genre = Console.ReadLine();
        
        Console.Write("Enter release year: ");
        if (int.TryParse(Console.ReadLine(), out int year))
        {
            _albums.Add(new Album { Artist = artist, Title = title, Genre = genre, Year = year });
            Console.WriteLine("Album added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid year format. Album not added.");
        }
    }
    
    private void ListAlbums()
    {
        if (_albums.Count == 0)
        {
            Console.WriteLine("No albums in collection.");
            return;
        }
        
        Console.WriteLine("\nAlbum Collection:");
        foreach (var album in _albums)
        {
            Console.WriteLine($"{album.Artist} - {album.Title} ({album.Genre}, {album.Year})");
        }
    }
    
    private void SearchByArtist()
    {
        Console.Write("Enter artist name to search: ");
        var artist = Console.ReadLine();
        
        var results = _albums.FindAll(a => a.Artist.Contains(artist, StringComparison.OrdinalIgnoreCase));
        
        if (results.Count == 0)
        {
            Console.WriteLine("No albums found for this artist.");
            return;
        }
        
        Console.WriteLine($"\nFound {results.Count} albums by {artist}:");
        foreach (var album in results)
        {
            Console.WriteLine($"{album.Title} ({album.Genre}, {album.Year})");
        }
    }
    
    private void SearchByGenre()
    {
        Console.Write("Enter genre to search: ");
        var genre = Console.ReadLine();
        
        var results = _albums.FindAll(a => a.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));
        
        if (results.Count == 0)
        {
            Console.WriteLine("No albums found in this genre.");
            return;
        }
        
        Console.WriteLine($"\nFound {results.Count} albums in {genre} genre:");
        foreach (var album in results)
        {
            Console.WriteLine($"{album.Artist} - {album.Title} ({album.Year})");
        }
    }
}

public class Album
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
}