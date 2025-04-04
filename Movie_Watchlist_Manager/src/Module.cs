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
        Console.WriteLine("Movie Watchlist Manager is running...");
        Console.WriteLine("Loading watchlist data...");

        _dataFilePath = Path.Combine(dataFolder, "watchlist.json");
        _movies = LoadMovies();

        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddMovie();
                    break;
                case "2":
                    ListMovies();
                    break;
                case "3":
                    FilterByGenre();
                    break;
                case "4":
                    FilterByRating();
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveMovies();
        Console.WriteLine("Watchlist data saved. Exiting...");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nMovie Watchlist Manager");
        Console.WriteLine("1. Add a movie");
        Console.WriteLine("2. List all movies");
        Console.WriteLine("3. Filter by genre");
        Console.WriteLine("4. Filter by rating");
        Console.WriteLine("5. Exit");
        Console.Write("Choose an option: ");
    }

    private void AddMovie()
    {
        Console.Write("Enter movie title: ");
        string title = Console.ReadLine();

        Console.Write("Enter genre: ");
        string genre = Console.ReadLine();

        Console.Write("Enter rating (1-5): ");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
        {
            Console.WriteLine("Invalid rating. Must be between 1 and 5.");
            return;
        }

        _movies.Add(new Movie { Title = title, Genre = genre, Rating = rating });
        Console.WriteLine("Movie added successfully.");
    }

    private void ListMovies()
    {
        if (_movies.Count == 0)
        {
            Console.WriteLine("No movies in the watchlist.");
            return;
        }

        Console.WriteLine("\nCurrent Watchlist:");
        foreach (var movie in _movies)
        {
            Console.WriteLine($"{movie.Title} ({movie.Genre}) - Rating: {movie.Rating}/5");
        }
    }

    private void FilterByGenre()
    {
        Console.Write("Enter genre to filter: ");
        string genre = Console.ReadLine();

        var filtered = _movies.FindAll(m => m.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
        if (filtered.Count == 0)
        {
            Console.WriteLine("No movies found with that genre.");
            return;
        }

        Console.WriteLine($"\nMovies in genre '{genre}':");
        foreach (var movie in filtered)
        {
            Console.WriteLine($"{movie.Title} - Rating: {movie.Rating}/5");
        }
    }

    private void FilterByRating()
    {
        Console.Write("Enter minimum rating (1-5): ");
        if (!int.TryParse(Console.ReadLine(), out int minRating) || minRating < 1 || minRating > 5)
        {
            Console.WriteLine("Invalid rating. Must be between 1 and 5.");
            return;
        }

        var filtered = _movies.FindAll(m => m.Rating >= minRating);
        if (filtered.Count == 0)
        {
            Console.WriteLine("No movies found with that minimum rating.");
            return;
        }

        Console.WriteLine($"\nMovies with rating {minRating} or higher:");
        foreach (var movie in filtered)
        {
            Console.WriteLine($"{movie.Title} ({movie.Genre}) - Rating: {movie.Rating}/5");
        }
    }

    private List<Movie> LoadMovies()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<Movie>();
        }

        string json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
    }

    private void SaveMovies()
    {
        string json = JsonSerializer.Serialize(_movies);
        File.WriteAllText(_dataFilePath, json);
    }

    private class Movie
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public int Rating { get; set; }
    }
}