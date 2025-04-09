using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ZooManager : IGeneratedModule
{
    public string Name { get; set; } = "Zoo Animal Inventory and Feeding Schedule Manager";

    private string _animalsFilePath;
    private string _feedingSchedulesFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Zoo Manager Module...");

        _animalsFilePath = Path.Combine(dataFolder, "animals.json");
        _feedingSchedulesFilePath = Path.Combine(dataFolder, "feedingSchedules.json");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        List<Animal> animals = LoadAnimals();
        List<FeedingSchedule> feedingSchedules = LoadFeedingSchedules();

        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nZoo Management System");
            Console.WriteLine("1. Add Animal");
            Console.WriteLine("2. View Animals");
            Console.WriteLine("3. Add Feeding Schedule");
            Console.WriteLine("4. View Feeding Schedules");
            Console.WriteLine("5. Exit Module");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddAnimal(animals);
                    break;
                case "2":
                    ViewAnimals(animals);
                    break;
                case "3":
                    AddFeedingSchedule(feedingSchedules, animals);
                    break;
                case "4":
                    ViewFeedingSchedules(feedingSchedules);
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveAnimals(animals);
        SaveFeedingSchedules(feedingSchedules);

        Console.WriteLine("Zoo Manager Module completed successfully.");
        return true;
    }

    private List<Animal> LoadAnimals()
    {
        if (File.Exists(_animalsFilePath))
        {
            string json = File.ReadAllText(_animalsFilePath);
            return JsonSerializer.Deserialize<List<Animal>>(json) ?? new List<Animal>();
        }
        return new List<Animal>();
    }

    private List<FeedingSchedule> LoadFeedingSchedules()
    {
        if (File.Exists(_feedingSchedulesFilePath))
        {
            string json = File.ReadAllText(_feedingSchedulesFilePath);
            return JsonSerializer.Deserialize<List<FeedingSchedule>>(json) ?? new List<FeedingSchedule>();
        }
        return new List<FeedingSchedule>();
    }

    private void SaveAnimals(List<Animal> animals)
    {
        string json = JsonSerializer.Serialize(animals);
        File.WriteAllText(_animalsFilePath, json);
    }

    private void SaveFeedingSchedules(List<FeedingSchedule> schedules)
    {
        string json = JsonSerializer.Serialize(schedules);
        File.WriteAllText(_feedingSchedulesFilePath, json);
    }

    private void AddAnimal(List<Animal> animals)
    {
        Console.Write("Enter animal name: ");
        string name = Console.ReadLine();

        Console.Write("Enter species: ");
        string species = Console.ReadLine();

        Console.Write("Enter enclosure number: ");
        string enclosure = Console.ReadLine();

        animals.Add(new Animal
        {
            Id = animals.Count + 1,
            Name = name,
            Species = species,
            Enclosure = enclosure,
            LastFed = DateTime.MinValue
        });

        Console.WriteLine("Animal added successfully.");
    }

    private void ViewAnimals(List<Animal> animals)
    {
        if (animals.Count == 0)
        {
            Console.WriteLine("No animals in inventory.");
            return;
        }

        Console.WriteLine("\nAnimal Inventory:");
        foreach (var animal in animals)
        {
            Console.WriteLine($"ID: {animal.Id}, Name: {animal.Name}, Species: {animal.Species}, Enclosure: {animal.Enclosure}");
        }
    }

    private void AddFeedingSchedule(List<FeedingSchedule> schedules, List<Animal> animals)
    {
        ViewAnimals(animals);
        if (animals.Count == 0) return;

        Console.Write("Enter animal ID: ");
        if (!int.TryParse(Console.ReadLine(), out int animalId))
        {
            Console.WriteLine("Invalid animal ID.");
            return;
        }

        var animal = animals.Find(a => a.Id == animalId);
        if (animal == null)
        {
            Console.WriteLine("Animal not found.");
            return;
        }

        Console.Write("Enter feeding time (HH:mm): ");
        if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan feedingTime))
        {
            Console.WriteLine("Invalid time format.");
            return;
        }

        Console.Write("Enter food type: ");
        string foodType = Console.ReadLine();

        Console.Write("Enter quantity: ");
        if (!double.TryParse(Console.ReadLine(), out double quantity))
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        schedules.Add(new FeedingSchedule
        {
            Id = schedules.Count + 1,
            AnimalId = animalId,
            AnimalName = animal.Name,
            Time = feedingTime,
            FoodType = foodType,
            Quantity = quantity,
            LastFed = DateTime.MinValue
        });

        Console.WriteLine("Feeding schedule added successfully.");
    }

    private void ViewFeedingSchedules(List<FeedingSchedule> schedules)
    {
        if (schedules.Count == 0)
        {
            Console.WriteLine("No feeding schedules available.");
            return;
        }

        Console.WriteLine("\nFeeding Schedules:");
        foreach (var schedule in schedules)
        {
            Console.WriteLine($"ID: {schedule.Id}, Animal: {schedule.AnimalName}, Time: {schedule.Time}, Food: {schedule.FoodType}, Quantity: {schedule.Quantity}");
        }
    }
}

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Enclosure { get; set; }
    public DateTime LastFed { get; set; }
}

public class FeedingSchedule
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string AnimalName { get; set; }
    public TimeSpan Time { get; set; }
    public string FoodType { get; set; }
    public double Quantity { get; set; }
    public DateTime LastFed { get; set; }
}