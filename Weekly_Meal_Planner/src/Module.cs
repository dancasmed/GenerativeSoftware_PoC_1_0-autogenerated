using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MealPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Meal Planner";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Weekly Meal Planner module is running...");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var mealPlan = GenerateWeeklyMealPlan();
            SaveMealPlan(mealPlan, Path.Combine(dataFolder, "weekly_meal_plan.json"));
            
            Console.WriteLine("Weekly meal plan generated successfully!");
            Console.WriteLine("Check the data folder for the meal plan and grocery list.");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the meal plan: " + ex.Message);
            return false;
        }
    }

    private WeeklyMealPlan GenerateWeeklyMealPlan()
    {
        var mealPlan = new WeeklyMealPlan();
        var random = new Random();
        
        var meals = new List<Meal>
        {
            new Meal { Name = "Scrambled Eggs with Toast", Calories = 350, Ingredients = ["Eggs", "Bread", "Butter"] },
            new Meal { Name = "Chicken Salad", Calories = 450, Ingredients = ["Chicken Breast", "Lettuce", "Tomatoes", "Cucumber", "Olive Oil"] },
            new Meal { Name = "Pasta with Tomato Sauce", Calories = 500, Ingredients = ["Pasta", "Tomato Sauce", "Ground Beef", "Cheese"] },
            new Meal { Name = "Grilled Salmon with Vegetables", Calories = 400, Ingredients = ["Salmon", "Broccoli", "Carrots", "Lemon"] },
            new Meal { Name = "Vegetable Stir Fry", Calories = 350, Ingredients = ["Rice", "Bell Peppers", "Onions", "Soy Sauce"] },
            new Meal { Name = "Beef Tacos", Calories = 600, Ingredients = ["Ground Beef", "Tortillas", "Lettuce", "Cheese", "Sour Cream"] },
            new Meal { Name = "Greek Yogurt with Berries", Calories = 300, Ingredients = ["Greek Yogurt", "Mixed Berries", "Honey"] }
        };
        
        var daysOfWeek = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        
        foreach (var day in daysOfWeek)
        {
            var selectedMeal = meals[random.Next(meals.Count)];
            mealPlan.DailyPlans.Add(new DailyMealPlan 
            { 
                Day = day, 
                Meal = selectedMeal.Name, 
                Calories = selectedMeal.Calories,
                Ingredients = selectedMeal.Ingredients
            });
        }
        
        mealPlan.GenerateGroceryList();
        return mealPlan;
    }

    private void SaveMealPlan(WeeklyMealPlan mealPlan, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(mealPlan, options);
        File.WriteAllText(filePath, jsonString);
    }
}

public class WeeklyMealPlan
{
    public List<DailyMealPlan> DailyPlans { get; set; } = new List<DailyMealPlan>();
    public List<string> GroceryList { get; set; } = new List<string>();

    public void GenerateGroceryList()
    {
        var uniqueIngredients = new HashSet<string>();
        
        foreach (var day in DailyPlans)
        {
            foreach (var ingredient in day.Ingredients)
            {
                uniqueIngredients.Add(ingredient);
            }
        }
        
        GroceryList.AddRange(uniqueIngredients);
    }
}

public class DailyMealPlan
{
    public string Day { get; set; }
    public string Meal { get; set; }
    public int Calories { get; set; }
    public List<string> Ingredients { get; set; } = new List<string>();
}

public class Meal
{
    public string Name { get; set; }
    public int Calories { get; set; }
    public List<string> Ingredients { get; set; } = new List<string>();
}