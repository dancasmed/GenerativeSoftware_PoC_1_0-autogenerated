using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class MealPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Meal Planner";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Weekly Meal Planner module is running...");

        try
        {
            string mealsFilePath = Path.Combine(dataFolder, "meals.json");
            string groceryListFilePath = Path.Combine(dataFolder, "grocery_list.json");

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            List<Meal> weeklyMeals = GenerateWeeklyMeals();
            SaveMealsToFile(weeklyMeals, mealsFilePath);

            List<GroceryItem> groceryList = GenerateGroceryList(weeklyMeals);
            SaveGroceryListToFile(groceryList, groceryListFilePath);

            Console.WriteLine("Weekly meal plan and grocery list generated successfully.");
            Console.WriteLine("Meals saved to: " + mealsFilePath);
            Console.WriteLine("Grocery list saved to: " + groceryListFilePath);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            return false;
        }
    }

    private List<Meal> GenerateWeeklyMeals()
    {
        var meals = new List<Meal>
        {
            new Meal { Day = "Monday", Name = "Grilled Chicken Salad", Calories = 450, Ingredients = new List<string> { "Chicken Breast", "Mixed Greens", "Cherry Tomatoes", "Cucumber", "Olive Oil" } },
            new Meal { Day = "Tuesday", Name = "Vegetable Stir Fry", Calories = 400, Ingredients = new List<string> { "Broccoli", "Bell Peppers", "Carrots", "Tofu", "Soy Sauce" } },
            new Meal { Day = "Wednesday", Name = "Spaghetti Bolognese", Calories = 600, Ingredients = new List<string> { "Ground Beef", "Spaghetti", "Tomato Sauce", "Onion", "Garlic" } },
            new Meal { Day = "Thursday", Name = "Quinoa Bowl", Calories = 500, Ingredients = new List<string> { "Quinoa", "Avocado", "Black Beans", "Corn", "Lime" } },
            new Meal { Day = "Friday", Name = "Salmon with Roasted Vegetables", Calories = 550, Ingredients = new List<string> { "Salmon Fillet", "Asparagus", "Sweet Potatoes", "Lemon" } },
            new Meal { Day = "Saturday", Name = "Homemade Pizza", Calories = 700, Ingredients = new List<string> { "Pizza Dough", "Tomato Sauce", "Mozzarella", "Pepperoni", "Mushrooms" } },
            new Meal { Day = "Sunday", Name = "Beef Stew", Calories = 650, Ingredients = new List<string> { "Beef Chuck", "Potatoes", "Carrots", "Onion", "Beef Broth" } }
        };

        return meals;
    }

    private List<GroceryItem> GenerateGroceryList(List<Meal> meals)
    {
        var allIngredients = meals.SelectMany(m => m.Ingredients).ToList();
        var groupedItems = allIngredients.GroupBy(i => i)
                                        .Select(g => new GroceryItem { Name = g.Key, Quantity = g.Count() + " " + GetMeasurementUnit(g.Key) })
                                        .ToList();

        return groupedItems;
    }

    private string GetMeasurementUnit(string ingredient)
    {
        if (ingredient.Contains("Oil") || ingredient.Contains("Sauce"))
            return "bottles";
        if (ingredient.Contains("Beef") || ingredient.Contains("Chicken") || ingredient.Contains("Salmon") || ingredient.Contains("Tofu"))
            return "pieces";
        if (ingredient.Contains("Greens") || ingredient.Contains("Vegetables"))
            return "bundles";
        return "units";
    }

    private void SaveMealsToFile(List<Meal> meals, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(meals, options);
        File.WriteAllText(filePath, json);
    }

    private void SaveGroceryListToFile(List<GroceryItem> groceryList, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(groceryList, options);
        File.WriteAllText(filePath, json);
    }
}

public class Meal
{
    public string Day { get; set; }
    public string Name { get; set; }
    public int Calories { get; set; }
    public List<string> Ingredients { get; set; }
}

public class GroceryItem
{
    public string Name { get; set; }
    public string Quantity { get; set; }
}