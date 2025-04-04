using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MovieWatchlistManager : IGeneratedModule
{
    public string Name { get; set; } = "Movie Watchlist Manager";
    
    private string _dataFilePath;
    private List<Movie> _movies;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Movie Watchlist Manager...");
        
        _dataFilePath = Path.Combine(dataFolder, "movies.json");
        _movies = new List<Movie>();
        
        LoadMovies();
        
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nMovie Watchlist Manager");
            Console.WriteLine("1. Add Movie");
            Console.WriteLine("2. View Movies");
            Console.WriteLine("3. Delete Movie");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddMovie();
                    break;
                case "2":
                    ViewMovies();
                    break;
                case "3":
                    DeleteMovie();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveMovies();
        Console.WriteLine("Movie Watchlist Manager exiting...");
        return true;
    }
    
    private void LoadMovies()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _movies = JsonSerializer.Deserialize<List<Movie>>(json);
                Console.WriteLine("Movie data loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading movie data: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("No existing movie data found. Starting with empty watchlist.");
        }
    }
    
    private void SaveMovies()
    {
        try
        {
            string json = JsonSerializer.Serialize(_movies);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Movie data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving movie data: " + ex.Message);
        }
    }
    
    private void AddMovie()
    {
        Console.Write("Enter movie title: ");
        string title = Console.ReadLine();
        
        Console.Write("Enter movie genre: ");
        string genre = Console.ReadLine();
        
        Console.Write("Enter movie rating (1-10): ");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 10)
        {
            Console.WriteLine("Invalid rating. Please enter a number between 1 and 10.");
            return;
        }
        
        _movies.Add(new Movie { Title = title, Genre = genre, Rating = rating });
        Console.WriteLine("Movie added successfully.");
    }
    
    private void ViewMovies()
    {
        if (_movies.Count == 0)
        {
            Console.WriteLine("No movies in the watchlist.");
            return;
        }
        
        Console.WriteLine("\nCurrent Watchlist:");
        Console.WriteLine("-----------------");
        foreach (var movie in _movies)
        {
            Console.WriteLine("Title: " + movie.Title);
            Console.WriteLine("Genre: " + movie.Genre);
            Console.WriteLine("Rating: " + movie.Rating + "/10");
            Console.WriteLine("-----------------");
        }
    }
    
    private void DeleteMovie()
    {
        if (_movies.Count == 0)
        {
            Console.WriteLine("No movies to delete.");
            return;
        }
        
        ViewMovies();
        Console.Write("Enter the title of the movie to delete: ");
        string title = Console.ReadLine();
        
        int removed = _movies.RemoveAll(m => m.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        
        if (removed > 0)
        {
            Console.WriteLine("Movie(s) removed successfully.");
        }
        else
        {
            Console.WriteLine("No movie found with that title.");
        }
    }
}

public class Movie
{
    public string Title { get; set; }
    public string Genre { get; set; }
    public int Rating { get; set; }
}