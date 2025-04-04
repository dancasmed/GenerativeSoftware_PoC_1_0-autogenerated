using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MovieWatchlistModule : IGeneratedModule
{
    public string Name { get; set; } = "Movie Watchlist Manager";

    private string _dataFilePath;
    private List<Movie> _movies;

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "watchlist.json");
        _movies = LoadMovies();

        Console.WriteLine("Movie Watchlist Manager is running.");
        Console.WriteLine("Type 'help' for available commands.");

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine().Trim().ToLower();

            if (input == "exit")
            {
                SaveMovies();
                return true;
            }

            ProcessCommand(input);
        }
    }

    private void ProcessCommand(string command)
    {
        string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        switch (parts[0])
        {
            case "help":
                ShowHelp();
                break;
            case "add":
                if (parts.Length > 1)
                    AddMovie(string.Join(' ', parts[1..]));
                else
                    Console.WriteLine("Please specify a movie title.");
                break;
            case "list":
                ListMovies();
                break;
            case "watch":
                if (parts.Length > 1 && int.TryParse(parts[1], out int watchId))
                    MarkAsWatched(watchId);
                else
                    Console.WriteLine("Please specify a valid movie ID.");
                break;
            case "rate":
                if (parts.Length > 2 && int.TryParse(parts[1], out int rateId) && int.TryParse(parts[2], out int rating))
                    RateMovie(rateId, rating);
                else
                    Console.WriteLine("Please specify valid movie ID and rating (1-5).");
                break;
            case "delete":
                if (parts.Length > 1 && int.TryParse(parts[1], out int deleteId))
                    DeleteMovie(deleteId);
                else
                    Console.WriteLine("Please specify a valid movie ID.");
                break;
            default:
                Console.WriteLine("Unknown command. Type 'help' for available commands.");
                break;
        }
    }

    private void ShowHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  add <title> - Add a new movie to watchlist");
        Console.WriteLine("  list - Show all movies in watchlist");
        Console.WriteLine("  watch <id> - Mark movie as watched");
        Console.WriteLine("  rate <id> <rating> - Rate movie (1-5)");
        Console.WriteLine("  delete <id> - Remove movie from watchlist");
        Console.WriteLine("  exit - Save and exit");
    }

    private void AddMovie(string title)
    {
        _movies.Add(new Movie
        {
            Id = _movies.Count > 0 ? _movies.Max(m => m.Id) + 1 : 1,
            Title = title,
            Watched = false,
            Rating = 0
        });
        Console.WriteLine("Movie added successfully.");
    }

    private void ListMovies()
    {
        if (_movies.Count == 0)
        {
            Console.WriteLine("Your watchlist is empty.");
            return;
        }

        foreach (var movie in _movies)
        {
            string watchedStatus = movie.Watched ? "[Watched]" : "[Not Watched]";
            string rating = movie.Rating > 0 ? $"Rating: {movie.Rating}/5" : "Not rated";
            Console.WriteLine($"{movie.Id}. {movie.Title} {watchedStatus} {rating}");
        }
    }

    private void MarkAsWatched(int id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        if (movie != null)
        {
            movie.Watched = true;
            Console.WriteLine($"Marked '{movie.Title}' as watched.");
        }
        else
        {
            Console.WriteLine("Movie not found.");
        }
    }

    private void RateMovie(int id, int rating)
    {
        if (rating < 1 || rating > 5)
        {
            Console.WriteLine("Rating must be between 1 and 5.");
            return;
        }

        var movie = _movies.FirstOrDefault(m => m.Id == id);
        if (movie != null)
        {
            movie.Rating = rating;
            Console.WriteLine($"Rated '{movie.Title}' with {rating} stars.");
        }
        else
        {
            Console.WriteLine("Movie not found.");
        }
    }

    private void DeleteMovie(int id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        if (movie != null)
        {
            _movies.Remove(movie);
            Console.WriteLine($"Removed '{movie.Title}' from watchlist.");
        }
        else
        {
            Console.WriteLine("Movie not found.");
        }
    }

    private List<Movie> LoadMovies()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
            }
            catch
            {
                return new List<Movie>();
            }
        }
        return new List<Movie>();
    }

    private void SaveMovies()
    {
        try
        {
            string json = JsonSerializer.Serialize(_movies);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving watchlist: " + ex.Message);
        }
    }
}

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool Watched { get; set; }
    public int Rating { get; set; }
}