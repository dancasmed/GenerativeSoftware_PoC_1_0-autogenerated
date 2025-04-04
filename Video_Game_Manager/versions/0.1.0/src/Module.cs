using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VideoGameManager : IGeneratedModule
{
    public string Name { get; set; } = "Video Game Manager";

    private List<VideoGame> _videoGames;
    private string _dataFilePath;

    public VideoGameManager()
    {
        _videoGames = new List<VideoGame>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Video Game Manager module is running.");
        _dataFilePath = Path.Combine(dataFolder, "videogames.json");

        LoadVideoGames();

        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
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
                    UpdateVideoGame();
                    break;
                case "4":
                    DeleteVideoGame();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveVideoGames();
        Console.WriteLine("Video Game Manager module finished.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nVideo Game Manager");
        Console.WriteLine("1. Add a video game");
        Console.WriteLine("2. List all video games");
        Console.WriteLine("3. Update a video game");
        Console.WriteLine("4. Delete a video game");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddVideoGame()
    {
        Console.Write("Enter game title: ");
        string title = Console.ReadLine();

        Console.Write("Enter game rating (1-5): ");
        if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
        {
            Console.WriteLine("Invalid rating. Must be between 1 and 5.");
            return;
        }

        Console.Write("Is the game completed? (Y/N): ");
        bool isCompleted = Console.ReadLine().ToUpper() == "Y";

        _videoGames.Add(new VideoGame
        {
            Id = Guid.NewGuid(),
            Title = title,
            Rating = rating,
            IsCompleted = isCompleted
        });

        Console.WriteLine("Game added successfully.");
    }

    private void ListVideoGames()
    {
        if (_videoGames.Count == 0)
        {
            Console.WriteLine("No video games found.");
            return;
        }

        Console.WriteLine("\nVideo Games List:");
        foreach (var game in _videoGames)
        {
            Console.WriteLine("ID: " + game.Id);
            Console.WriteLine("Title: " + game.Title);
            Console.WriteLine("Rating: " + game.Rating);
            Console.WriteLine("Completed: " + (game.IsCompleted ? "Yes" : "No"));
            Console.WriteLine();
        }
    }

    private void UpdateVideoGame()
    {
        Console.Write("Enter game ID to update: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        var game = _videoGames.Find(g => g.Id == id);
        if (game == null)
        {
            Console.WriteLine("Game not found.");
            return;
        }

        Console.Write("Enter new title (leave blank to keep current): ");
        string newTitle = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            game.Title = newTitle;
        }

        Console.Write("Enter new rating (1-5, leave blank to keep current): ");
        string ratingInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(ratingInput))
        {
            if (int.TryParse(ratingInput, out int newRating) && newRating >= 1 && newRating <= 5)
            {
                game.Rating = newRating;
            }
            else
            {
                Console.WriteLine("Invalid rating. Must be between 1 and 5.");
            }
        }

        Console.Write("Update completion status? (Y/N, leave blank to keep current): ");
        string completionInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(completionInput))
        {
            game.IsCompleted = completionInput.ToUpper() == "Y";
        }

        Console.WriteLine("Game updated successfully.");
    }

    private void DeleteVideoGame()
    {
        Console.Write("Enter game ID to delete: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        var game = _videoGames.Find(g => g.Id == id);
        if (game == null)
        {
            Console.WriteLine("Game not found.");
            return;
        }

        _videoGames.Remove(game);
        Console.WriteLine("Game deleted successfully.");
    }

    private void LoadVideoGames()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _videoGames = JsonSerializer.Deserialize<List<VideoGame>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading video games: " + ex.Message);
            }
        }
    }

    private void SaveVideoGames()
    {
        try
        {
            string json = JsonSerializer.Serialize(_videoGames);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving video games: " + ex.Message);
        }
    }
}

public class VideoGame
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Rating { get; set; }
    public bool IsCompleted { get; set; }
}