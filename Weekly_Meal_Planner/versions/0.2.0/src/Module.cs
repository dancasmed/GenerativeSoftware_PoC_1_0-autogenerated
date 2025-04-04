using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MealPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Meal Planner";

    private static readonly List<string> BreakfastOptions = new List<string>
    {
        "Oatmeal with fruits",
        "Scrambled eggs with toast",
        "Yogurt with granola",
        "Pancakes with maple syrup",
        "Smoothie bowl",
        "Avocado toast",
        "French toast"
    };

    private static readonly List<string> LunchOptions = new List<string>
    {
        "Chicken Caesar salad",
        "Vegetable stir fry with rice",
        "Grilled cheese sandwich with tomato soup",
        "Quinoa bowl with roasted vegetables",
        "Tuna salad wrap",
        "Pasta with pesto",
        "Burrito bowl"
    };

    private static readonly List<string> DinnerOptions = new List<string>
    {
        "Grilled salmon with asparagus",
        "Beef tacos with guacamole",
        "Vegetable lasagna",
        "Roast chicken with mashed potatoes",
        "Shrimp stir fry with noodles",
        "Mushroom risotto",
        "Homemade pizza"
    };

    private static readonly Random random = new Random();

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Generating weekly meal plan...");

        try
        {
            var mealPlan = new Dictionary<string, Dictionary<string, string>>();
            var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            foreach (var day in daysOfWeek)
            {
                mealPlan[day] = new Dictionary<string, string>
                {
                    { "Breakfast", GetRandomMeal(BreakfastOptions) },
                    { "Lunch", GetRandomMeal(LunchOptions) },
                    { "Dinner", GetRandomMeal(DinnerOptions) }
                };
            }

            string json = JsonSerializer.Serialize(mealPlan, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(dataFolder, "weekly_meal_plan.json");
            File.WriteAllText(filePath, json);

            Console.WriteLine("Weekly meal plan generated successfully!");
            Console.WriteLine("Saved to: " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating meal plan: " + ex.Message);
            return false;
        }
    }

    private static string GetRandomMeal(List<string> options)
    {
        return options[random.Next(options.Count)];
    }
}