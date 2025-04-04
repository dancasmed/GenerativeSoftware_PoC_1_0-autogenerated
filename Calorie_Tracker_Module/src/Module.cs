using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CalorieTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Calorie Tracker Module";

    private const string FoodEntriesFileName = "food_entries.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Calorie Tracker Module is running...");
        
        try
        {
            string filePath = Path.Combine(dataFolder, FoodEntriesFileName);
            List<FoodEntry> foodEntries = LoadFoodEntries(filePath);
            
            double totalCalories = CalculateTotalCalories(foodEntries);
            
            Console.WriteLine("Total calorie intake: " + totalCalories + " kcal");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private List<FoodEntry> LoadFoodEntries(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<FoodEntry>();
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<FoodEntry>>(json) ?? new List<FoodEntry>();
    }

    private double CalculateTotalCalories(List<FoodEntry> foodEntries)
    {
        double total = 0;
        foreach (var entry in foodEntries)
        {
            total += entry.Calories;
        }
        return total;
    }
}

public class FoodEntry
{
    public string Name { get; set; } = string.Empty;
    public double Calories { get; set; }
}