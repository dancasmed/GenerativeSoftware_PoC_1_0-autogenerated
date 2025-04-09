using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MovieDatabaseModule : IGeneratedModule
{
    public string Name { get; set; } = "Movie Database Manager";

    private string _dataFilePath;
    private List<Movie> _movies;

    public MovieDatabaseModule()
    {
        _movies = new List<Movie>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Movie Database Module is running...");
        _dataFilePath = Path.Combine(dataFolder, "movies.json");

        try
        {
            LoadMovies();
            bool exitRequested = false;

            while (!exitRequested)
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
                        SearchMovies();
                        break;
                    case "4":
                        exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            SaveMovies();
            Console.WriteLine("Movie database saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadMovies()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _movies = JsonSerializer.Deserialize<List<Movie>>(json);
        }
    }

    private void SaveMovies()
    {
        string json = JsonSerializer.Serialize(_movies);
        File.WriteAllText(_dataFilePath, json);
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nMovie Database Manager");
        Console.WriteLine("1. Add a new movie");
        Console.WriteLine("2. List all movies");
        Console.WriteLine("3. Search movies");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddMovie()
    {
        Console.Write("Enter movie title: ");
        string title = Console.ReadLine();

        Console.Write("Enter genre: ");
        string genre = Console.ReadLine();

        Console.Write("Enter release date (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime releaseDate))
        {
            Console.WriteLine("Invalid date format. Using current date.");
            releaseDate = DateTime.Now;
        }

        Console.Write("Enter rating (1-10): ");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 10)
        {
            Console.WriteLine("Invalid rating. Using default value 5.");
            rating = 5;
        }

        _movies.Add(new Movie
        {
            Title = title,
            Genre = genre,
            ReleaseDate = releaseDate,
            Rating = rating
        });

        Console.WriteLine("Movie added successfully.");
    }

    private void ListMovies()
    {
        if (_movies.Count == 0)
        {
            Console.WriteLine("No movies in the database.");
            return;
        }

        Console.WriteLine("\nList of Movies:");
        foreach (var movie in _movies)
        {
            Console.WriteLine($"{movie.Title} ({movie.ReleaseDate:yyyy}) - {movie.Genre} - Rating: {movie.Rating}/10");
        }
    }

    private void SearchMovies()
    {
        Console.Write("Enter search term (title or genre): ");
        string term = Console.ReadLine().ToLower();

        var results = _movies.FindAll(m => 
            m.Title.ToLower().Contains(term) || 
            m.Genre.ToLower().Contains(term));

        if (results.Count == 0)
        {
            Console.WriteLine("No matching movies found.");
            return;
        }

        Console.WriteLine("\nSearch Results:");
        foreach (var movie in results)
        {
            Console.WriteLine($"{movie.Title} ({movie.ReleaseDate:yyyy}) - {movie.Genre} - Rating: {movie.Rating}/10");
        }
    }

    private class Movie
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }
    }
}