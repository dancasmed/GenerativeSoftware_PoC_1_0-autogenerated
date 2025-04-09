using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PhotographyPortfolioManager : IGeneratedModule
{
    public string Name { get; set; } = "Photography Portfolio Manager";

    private string _dataFolder;
    private string _portfolioFilePath;

    public PhotographyPortfolioManager()
    {
    }

    public bool Main(string dataFolder)
    {
        _dataFolder = dataFolder;
        _portfolioFilePath = Path.Combine(_dataFolder, "portfolio.json");

        Console.WriteLine("Photography Portfolio Manager is running.");
        Console.WriteLine("Data will be stored in: " + _dataFolder);

        InitializePortfolioFile();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddPhoto();
                    break;
                case "2":
                    ListPhotos();
                    break;
                case "3":
                    SearchPhotosByTag();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Exiting Photography Portfolio Manager.");
        return true;
    }

    private void InitializePortfolioFile()
    {
        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }

        if (!File.Exists(_portfolioFilePath))
        {
            File.WriteAllText(_portfolioFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPhotography Portfolio Manager");
        Console.WriteLine("1. Add a new photo");
        Console.WriteLine("2. List all photos");
        Console.WriteLine("3. Search photos by tag");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddPhoto()
    {
        Console.Write("Enter photo file name: ");
        string fileName = Console.ReadLine();

        Console.Write("Enter photo title: ");
        string title = Console.ReadLine();

        Console.Write("Enter photo description: ");
        string description = Console.ReadLine();

        Console.Write("Enter date taken (yyyy-MM-dd): ");
        string dateTaken = Console.ReadLine();

        Console.Write("Enter tags (comma separated): ");
        string tagsInput = Console.ReadLine();
        List<string> tags = new List<string>(tagsInput.Split(','));

        var photo = new Photo
        {
            FileName = fileName,
            Title = title,
            Description = description,
            DateTaken = dateTaken,
            Tags = tags
        };

        var portfolio = LoadPortfolio();
        portfolio.Add(photo);
        SavePortfolio(portfolio);

        Console.WriteLine("Photo added successfully.");
    }

    private void ListPhotos()
    {
        var portfolio = LoadPortfolio();

        if (portfolio.Count == 0)
        {
            Console.WriteLine("No photos in the portfolio.");
            return;
        }

        Console.WriteLine("\nPhoto Portfolio:");
        foreach (var photo in portfolio)
        {
            Console.WriteLine("File: " + photo.FileName);
            Console.WriteLine("Title: " + photo.Title);
            Console.WriteLine("Description: " + photo.Description);
            Console.WriteLine("Date Taken: " + photo.DateTaken);
            Console.WriteLine("Tags: " + string.Join(", ", photo.Tags));
            Console.WriteLine();
        }
    }

    private void SearchPhotosByTag()
    {
        Console.Write("Enter tag to search: ");
        string searchTag = Console.ReadLine().Trim();

        var portfolio = LoadPortfolio();
        var matchingPhotos = portfolio.FindAll(p => p.Tags.Contains(searchTag));

        if (matchingPhotos.Count == 0)
        {
            Console.WriteLine("No photos found with tag: " + searchTag);
            return;
        }

        Console.WriteLine("\nPhotos with tag '" + searchTag + "':");
        foreach (var photo in matchingPhotos)
        {
            Console.WriteLine("File: " + photo.FileName);
            Console.WriteLine("Title: " + photo.Title);
            Console.WriteLine();
        }
    }

    private List<Photo> LoadPortfolio()
    {
        string json = File.ReadAllText(_portfolioFilePath);
        return JsonSerializer.Deserialize<List<Photo>>(json);
    }

    private void SavePortfolio(List<Photo> portfolio)
    {
        string json = JsonSerializer.Serialize(portfolio, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_portfolioFilePath, json);
    }
}

public class Photo
{
    public string FileName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string DateTaken { get; set; }
    public List<string> Tags { get; set; }
}