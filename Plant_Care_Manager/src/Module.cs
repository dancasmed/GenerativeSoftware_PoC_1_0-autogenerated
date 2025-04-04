using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PlantCareModule : IGeneratedModule
{
    public string Name { get; set; } = "Plant Care Manager";

    private string _plantsFilePath;
    private List<Plant> _plants;

    public PlantCareModule()
    {
        _plants = new List<Plant>();
    }

    public bool Main(string dataFolder)
    {
        _plantsFilePath = Path.Combine(dataFolder, "plants.json");
        LoadPlants();

        Console.WriteLine("Plant Care Manager is running.");
        Console.WriteLine("Checking plant care schedules...");

        CheckCareSchedules();

        return true;
    }

    private void LoadPlants()
    {
        if (File.Exists(_plantsFilePath))
        {
            try
            {
                string json = File.ReadAllText(_plantsFilePath);
                _plants = JsonSerializer.Deserialize<List<Plant>>(json);
                Console.WriteLine("Loaded existing plant data.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading plant data: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("No existing plant data found. Starting with an empty list.");
        }
    }

    private void SavePlants()
    {
        try
        {
            string json = JsonSerializer.Serialize(_plants);
            File.WriteAllText(_plantsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving plant data: " + ex.Message);
        }
    }

    private void CheckCareSchedules()
    {
        DateTime today = DateTime.Today;
        bool needsCare = false;

        foreach (var plant in _plants)
        {
            if (plant.NextWateringDate <= today)
            {
                Console.WriteLine("REMINDER: Water " + plant.Name + " today!");
                plant.LastWateredDate = today;
                plant.NextWateringDate = today.AddDays(plant.WateringFrequencyDays);
                needsCare = true;
            }

            if (plant.NextFertilizingDate <= today)
            {
                Console.WriteLine("REMINDER: Fertilize " + plant.Name + " today!");
                plant.LastFertilizedDate = today;
                plant.NextFertilizingDate = today.AddDays(plant.FertilizingFrequencyDays);
                needsCare = true;
            }
        }

        if (needsCare)
        {
            SavePlants();
        }
        else
        {
            Console.WriteLine("No plants need care today.");
        }
    }

    private class Plant
    {
        public string Name { get; set; }
        public int WateringFrequencyDays { get; set; }
        public int FertilizingFrequencyDays { get; set; }
        public DateTime LastWateredDate { get; set; }
        public DateTime NextWateringDate { get; set; }
        public DateTime LastFertilizedDate { get; set; }
        public DateTime NextFertilizingDate { get; set; }
    }
}