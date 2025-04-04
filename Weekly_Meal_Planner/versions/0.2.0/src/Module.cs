using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MealPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Meal Planner";

    private readonly List<string> _breakfastOptions = new List<string>
    {
        "Oatmeal with fruits",
        "Scrambled eggs with toast",
        "Yogurt with granola",
        "Pancakes with maple syrup",
        "Smoothie bowl",
        "Avocado toast",
        "Bagel with cream cheese"
    };

    private readonly List<string> _lunchOptions = new List<string>
    {
        "Chicken salad",
        "Vegetable stir fry",
        "Grilled cheese sandwich",
        "Pasta with tomato sauce",
        "Quinoa bowl",
        "Tuna sandwich",
        "Burrito"
    };

    private readonly List<string> _dinnerOptions = new List<string>
    {
        "Grilled salmon with vegetables",
        "Beef stew",
        "Vegetable curry",
        "Roast chicken with potatoes",
        "Spaghetti carbonara",
        "Stuffed bell peppers",
        "Pizza"
    };

    private readonly Random _random = new Random();

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Generating weekly meal plan...");

        try
        {
            var mealPlan = GenerateWeeklyMealPlan();
            SaveMealPlan(mealPlan, dataFolder);
            DisplayMealPlan(mealPlan);
            Console.WriteLine("Meal plan generated successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating meal plan: " + ex.Message);
            return false;
        }
    }

    private Dictionary<string, Dictionary<string, string>> GenerateWeeklyMealPlan()
    {
        var daysOfWeek = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        var mealPlan = new Dictionary<string, Dictionary<string, string>>();

        foreach (var day in daysOfWeek)
        {
            mealPlan[day] = new Dictionary<string, string>
            {
                { "Breakfast", GetRandomMeal(_breakfastOptions) },
                { "Lunch", GetRandomMeal(_lunchOptions) },
                { "Dinner", GetRandomMeal(_dinnerOptions) }
            };
        }

        return mealPlan;
    }

    private string GetRandomMeal(List<string> options)
    {
        return options[_random.Next(options.Count)];
    }

    private void SaveMealPlan(Dictionary<string, Dictionary<string, string>> mealPlan, string dataFolder)
    {
        var filePath = Path.Combine(dataFolder, "weekly_meal_plan.json");
        var json = JsonSerializer.Serialize(mealPlan, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private void DisplayMealPlan(Dictionary<string, Dictionary<string, string>> mealPlan)
    {
        foreach (var day in mealPlan)
        {
            Console.WriteLine(day.Key + ":");
            Console.WriteLine("  Breakfast: " + day.Value["Breakfast"]);
            Console.WriteLine("  Lunch: " + day.Value["Lunch"]);
            Console.WriteLine("  Dinner: " + day.Value["Dinner"]);
            Console.WriteLine();
        }
    }
}