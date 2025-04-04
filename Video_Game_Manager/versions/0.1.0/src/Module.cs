using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VideoGameManager : IGeneratedModule
{
    public string Name { get; set; } = "Video Game Manager";

    private string _dataFilePath;
    private List<VideoGame> _videoGames;

    public VideoGameManager()
    {
        _videoGames = new List<VideoGame>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Video Game Manager module is running.");
        _dataFilePath = Path.Combine(dataFolder, "videogames.json");

        try
        {
            LoadVideoGames();
            ShowMenu();
            SaveVideoGames();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadVideoGames()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            _videoGames = JsonSerializer.Deserialize<List<VideoGame>>(json);
        }
    }

    private void SaveVideoGames()
    {
        string json = JsonSerializer.Serialize(_videoGames);
        File.WriteAllText(_dataFilePath, json);
    }

    private void ShowMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nVideo Game Manager");
            Console.WriteLine("1. Add Video Game");
            Console.WriteLine("2. List Video Games");
            Console.WriteLine("3. Update Completion Status");
            Console.WriteLine("4. Update Rating");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddVideoGame();
                    break;
                case "2":
                    ListVideoGames();
                    break;
                case "3":
                    UpdateCompletionStatus();
                    break;
                case "4":
                    UpdateRating();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void AddVideoGame()
    {
        Console.Write("Enter game title: ");
        string title = Console.ReadLine();

        Console.Write("Enter genre: ");
        string genre = Console.ReadLine();

        Console.Write("Enter rating (1-10): ");
        int rating = int.Parse(Console.ReadLine());

        Console.Write("Is the game completed? (true/false): ");
        bool isCompleted = bool.Parse(Console.ReadLine());

        _videoGames.Add(new VideoGame
        {
            Title = title,
            Genre = genre,
            Rating = rating,
            IsCompleted = isCompleted
        });

        Console.WriteLine("Game added successfully.");
    }

    private void ListVideoGames()
    {
        if (_videoGames.Count == 0)
        {
            Console.WriteLine("No games in the collection.");
            return;
        }

        Console.WriteLine("\nVideo Game Collection:");
        foreach (var game in _videoGames)
        {
            Console.WriteLine("Title: " + game.Title);
            Console.WriteLine("Genre: " + game.Genre);
            Console.WriteLine("Rating: " + game.Rating);
            Console.WriteLine("Completed: " + game.IsCompleted);
            Console.WriteLine();
        }
    }

    private void UpdateCompletionStatus()
    {
        if (_videoGames.Count == 0)
        {
            Console.WriteLine("No games in the collection.");
            return;
        }

        ListVideoGames();
        Console.Write("Enter the title of the game to update: ");
        string title = Console.ReadLine();

        var game = _videoGames.Find(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (game == null)
        {
            Console.WriteLine("Game not found.");
            return;
        }

        Console.Write("Enter new completion status (true/false): ");
        game.IsCompleted = bool.Parse(Console.ReadLine());
        Console.WriteLine("Completion status updated successfully.");
    }

    private void UpdateRating()
    {
        if (_videoGames.Count == 0)
        {
            Console.WriteLine("No games in the collection.");
            return;
        }

        ListVideoGames();
        Console.Write("Enter the title of the game to update: ");
        string title = Console.ReadLine();

        var game = _videoGames.Find(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (game == null)
        {
            Console.WriteLine("Game not found.");
            return;
        }

        Console.Write("Enter new rating (1-10): ");
        game.Rating = int.Parse(Console.ReadLine());
        Console.WriteLine("Rating updated successfully.");
    }
}

public class VideoGame
{
    public string Title { get; set; }
    public string Genre { get; set; }
    public int Rating { get; set; }
    public bool IsCompleted { get; set; }
}