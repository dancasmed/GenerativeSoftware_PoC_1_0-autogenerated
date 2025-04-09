using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GardenPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Garden Planner Module";

    private string plantsFilePath;
    private List<Plant> plants;

    public GardenPlannerModule()
    {
        plants = new List<Plant>();
    }

    public bool Main(string dataFolder)
    {
        plantsFilePath = Path.Combine(dataFolder, "plants.json");
        LoadPlants();

        Console.WriteLine("Garden Planner Module is running.");
        Console.WriteLine("Enter a command (add, list, save, exit):");

        string command;
        do
        {
            command = Console.ReadLine()?.ToLower().Trim();

            switch (command)
            {
                case "add":
                    AddPlant();
                    break;
                case "list":
                    ListPlants();
                    break;
                case "save":
                    SavePlants();
                    break;
                case "exit":
                    Console.WriteLine("Exiting Garden Planner Module.");
                    break;
                default:
                    if (command != "exit")
                        Console.WriteLine("Invalid command. Try again.");
                    break;
            }

        } while (command != "exit");

        return true;
    }

    private void AddPlant()
    {
        Console.WriteLine("Enter plant name:");
        string name = Console.ReadLine();

        Console.WriteLine("Enter planting date (yyyy-MM-dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime plantingDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        Console.WriteLine("Enter care instructions:");
        string careInstructions = Console.ReadLine();

        plants.Add(new Plant
        {
            Name = name,
            PlantingDate = plantingDate,
            CareInstructions = careInstructions
        });

        Console.WriteLine("Plant added successfully.");
    }

    private void ListPlants()
    {
        if (plants.Count == 0)
        {
            Console.WriteLine("No plants in the garden planner.");
            return;
        }

        Console.WriteLine("Plants in the garden planner:");
        foreach (var plant in plants)
        {
            Console.WriteLine("Name: " + plant.Name);
            Console.WriteLine("Planting Date: " + plant.PlantingDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("Care Instructions: " + plant.CareInstructions);
            Console.WriteLine();
        }
    }

    private void LoadPlants()
    {
        if (File.Exists(plantsFilePath))
        {
            try
            {
                string json = File.ReadAllText(plantsFilePath);
                plants = JsonSerializer.Deserialize<List<Plant>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading plants: " + ex.Message);
            }
        }
    }

    private void SavePlants()
    {
        try
        {
            string json = JsonSerializer.Serialize(plants);
            File.WriteAllText(plantsFilePath, json);
            Console.WriteLine("Plants saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving plants: " + ex.Message);
        }
    }
}

public class Plant
{
    public string Name { get; set; }
    public DateTime PlantingDate { get; set; }
    public string CareInstructions { get; set; }
}