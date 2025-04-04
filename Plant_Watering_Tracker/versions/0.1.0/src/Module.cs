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
        Console.WriteLine("Plant Watering Tracker module is running.");
        _dataFilePath = Path.Combine(dataFolder, "plants.json");
        LoadPlants();

        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddPlant();
                    break;
                case "2":
                    ViewPlants();
                    break;
                case "3":
                    UpdatePlant();
                    break;
                case "4":
                    DeletePlant();
                    break;
                case "5":
                    CheckWateringSchedule();
                    break;
                case "6":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SavePlants();
        Console.WriteLine("Plant Watering Tracker module has finished.");
        return true;
    }

    private void LoadPlants()
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

    private void SavePlants()
    {
        string json = JsonSerializer.Serialize(_plants);
        File.WriteAllText(_dataFilePath, json);
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nPlant Watering Tracker");
        Console.WriteLine("1. Add a new plant");
        Console.WriteLine("2. View all plants");
        Console.WriteLine("3. Update a plant");
        Console.WriteLine("4. Delete a plant");
        Console.WriteLine("5. Check watering schedule");
        Console.WriteLine("6. Exit");
        Console.Write("Select an option: ");
    }

    private void AddPlant()
    {
        Console.Write("Enter plant name: ");
        string name = Console.ReadLine();

        Console.Write("Enter watering frequency (days): ");
        int wateringFrequency = int.Parse(Console.ReadLine());

        Console.Write("Enter sunlight needs (full, partial, shade): ");
        string sunlightNeeds = Console.ReadLine();

        Console.Write("Enter last watered date (yyyy-MM-dd): ");
        DateTime lastWatered = DateTime.Parse(Console.ReadLine());

        var plant = new Plant
        {
            Name = name,
            WateringFrequency = wateringFrequency,
            SunlightNeeds = sunlightNeeds,
            LastWatered = lastWatered
        };

        _plants.Add(plant);
        Console.WriteLine("Plant added successfully.");
    }

    private void ViewPlants()
    {
        if (_plants.Count == 0)
        {
            Console.WriteLine("No plants found.");
            return;
        }

        Console.WriteLine("\nList of Plants:");
        foreach (var plant in _plants)
        {
            Console.WriteLine($"Name: {plant.Name}, Watering Frequency: every {plant.WateringFrequency} days, Sunlight Needs: {plant.SunlightNeeds}, Last Watered: {plant.LastWatered.ToShortDateString()}");
        }
    }

    private void UpdatePlant()
    {
        ViewPlants();
        if (_plants.Count == 0) return;

        Console.Write("Enter the name of the plant to update: ");
        string name = Console.ReadLine();

        var plant = _plants.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (plant == null)
        {
            Console.WriteLine("Plant not found.");
            return;
        }


        Console.Write("Enter new watering frequency (days): ");
        plant.WateringFrequency = int.Parse(Console.ReadLine());

        Console.Write("Enter new sunlight needs (full, partial, shade): ");
        plant.SunlightNeeds = Console.ReadLine();

        Console.Write("Enter new last watered date (yyyy-MM-dd): ");
        plant.LastWatered = DateTime.Parse(Console.ReadLine());

        Console.WriteLine("Plant updated successfully.");
    }

    private void DeletePlant()
    {
        ViewPlants();
        if (_plants.Count == 0) return;

        Console.Write("Enter the name of the plant to delete: ");
        string name = Console.ReadLine();

        var plant = _plants.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (plant == null)
        {
            Console.WriteLine("Plant not found.");
            return;
        }

        _plants.Remove(plant);
        Console.WriteLine("Plant deleted successfully.");
    }

    private void CheckWateringSchedule()
    {
        if (_plants.Count == 0)
        {
            Console.WriteLine("No plants found.");
            return;
        }

        Console.WriteLine("\nWatering Schedule:");
        foreach (var plant in _plants)
        {
            DateTime nextWatering = plant.LastWatered.AddDays(plant.WateringFrequency);
            if (DateTime.Now >= nextWatering)
            {
                Console.WriteLine($"{plant.Name} needs to be watered today! (Last watered: {plant.LastWatered.ToShortDateString()})");
            }
            else
            {
                Console.WriteLine($"{plant.Name} should be watered on {nextWatering.ToShortDateString()} (Last watered: {plant.LastWatered.ToShortDateString()})");
            }
        }
    }
}

public class Plant
{
    public string Name { get; set; }
    public int WateringFrequency { get; set; }
    public string SunlightNeeds { get; set; }
    public DateTime LastWatered { get; set; }
}