using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PlantWateringTracker : IGeneratedModule
{
    public string Name { get; set; } = "Plant Watering Tracker";

    private string _dataFilePath;
    private List<Plant> _plants;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Plant Watering Tracker...");
        _dataFilePath = Path.Combine(dataFolder, "plants.json");
        LoadPlants();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nPlant Watering Tracker Menu:");
            Console.WriteLine("1. Add a new plant");
            Console.WriteLine("2. View all plants");
            Console.WriteLine("3. Update plant status");
            Console.WriteLine("4. Save and exit");
            Console.Write("Select an option: ");

            if (int.TryParse(Console.ReadLine(), out int option))
            {
                switch (option)
                {
                    case 1:
                        AddPlant();
                        break;
                    case 2:
                        ViewPlants();
                        break;
                    case 3:
                        UpdatePlantStatus();
                        break;
                    case 4:
                        SavePlants();
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }

        Console.WriteLine("Plant Watering Tracker has finished.");
        return true;
    }

    private void LoadPlants()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _plants = JsonSerializer.Deserialize<List<Plant>>(json);
            }
            else
            {
                _plants = new List<Plant>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading plants: " + ex.Message);
            _plants = new List<Plant>();
        }
    }

    private void SavePlants()
    {
        try
        {
            string json = JsonSerializer.Serialize(_plants);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Plant data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving plants: " + ex.Message);
        }
    }

    private void AddPlant()
    {
        Console.Write("Enter plant name: ");
        string name = Console.ReadLine();

        Console.Write("Enter watering frequency (in days): ");
        if (!int.TryParse(Console.ReadLine(), out int wateringFrequency))
        {
            Console.WriteLine("Invalid input for watering frequency.");
            return;
        }

        Console.Write("Enter sunlight needs (e.g., Full Sun, Partial Shade): ");
        string sunlightNeeds = Console.ReadLine();

        var plant = new Plant
        {
            Name = name,
            WateringFrequency = wateringFrequency,
            SunlightNeeds = sunlightNeeds,
            LastWateredDate = DateTime.Now
        };

        _plants.Add(plant);
        Console.WriteLine("Plant added successfully.");
    }

    private void ViewPlants()
    {
        if (_plants.Count == 0)
        {
            Console.WriteLine("No plants registered yet.");
            return;
        }

        Console.WriteLine("\nList of Plants:");
        foreach (var plant in _plants)
        {
            Console.WriteLine($"Name: {plant.Name}");
            Console.WriteLine($"Watering Frequency: Every {plant.WateringFrequency} days");
            Console.WriteLine($"Sunlight Needs: {plant.SunlightNeeds}");
            Console.WriteLine($"Last Watered: {plant.LastWateredDate.ToShortDateString()}");
            Console.WriteLine($"Next Watering Due: {plant.LastWateredDate.AddDays(plant.WateringFrequency).ToShortDateString()}");
            Console.WriteLine();
        }
    }

    private void UpdatePlantStatus()
    {
        if (_plants.Count == 0)
        {
            Console.WriteLine("No plants registered yet.");
            return;
        }

        ViewPlants();
        Console.Write("Enter the name of the plant to update: ");
        string name = Console.ReadLine();

        var plant = _plants.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (plant == null)
        {
            Console.WriteLine("Plant not found.");
            return;
        }

        Console.WriteLine("1. Update watering date");
        Console.WriteLine("2. Update sunlight needs");
        Console.Write("Select an option: ");

        if (int.TryParse(Console.ReadLine(), out int option))
        {
            switch (option)
            {
                case 1:
                    plant.LastWateredDate = DateTime.Now;
                    Console.WriteLine("Watering date updated to today.");
                    break;
                case 2:
                    Console.Write("Enter new sunlight needs: ");
                    plant.SunlightNeeds = Console.ReadLine();
                    Console.WriteLine("Sunlight needs updated.");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
}

public class Plant
{
    public string Name { get; set; }
    public int WateringFrequency { get; set; }
    public string SunlightNeeds { get; set; }
    public DateTime LastWateredDate { get; set; }
}