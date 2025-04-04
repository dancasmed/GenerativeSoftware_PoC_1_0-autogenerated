using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class MealPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Meal Planner";

    private List<Meal> _meals = new List<Meal>();
    private List<GroceryItem> _groceryList = new List<GroceryItem>();

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Weekly Meal Planner module is running...");

        try
        {
            string mealsFilePath = Path.Combine(dataFolder, "meals.json");
            string groceryListFilePath = Path.Combine(dataFolder, "grocery_list.json");

            if (File.Exists(mealsFilePath))
            {
                string jsonData = File.ReadAllText(mealsFilePath);
                _meals = JsonSerializer.Deserialize<List<Meal>>(jsonData);
            }
            else
            {
                InitializeDefaultMeals();
                SaveMealsToFile(mealsFilePath);
            }

            PlanWeeklyMeals();
            GenerateGroceryList();
            SaveGroceryListToFile(groceryListFilePath);

            Console.WriteLine("Weekly meal plan and grocery list have been generated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void InitializeDefaultMeals()
    {
        _meals = new List<Meal>
        {
            new Meal { Name = "Grilled Chicken Salad", Calories = 450, Ingredients = new List<string> { "Chicken Breast", "Lettuce", "Tomato", "Cucumber", "Olive Oil" } },
            new Meal { Name = "Vegetable Stir Fry", Calories = 350, Ingredients = new List<string> { "Broccoli", "Bell Peppers", "Carrots", "Soy Sauce", "Rice" } },
            new Meal { Name = "Spaghetti Bolognese", Calories = 600, Ingredients = new List<string> { "Spaghetti", "Ground Beef", "Tomato Sauce", "Onion", "Garlic" } },
            new Meal { Name = "Avocado Toast", Calories = 300, Ingredients = new List<string> { "Bread", "Avocado", "Eggs", "Salt", "Pepper" } },
            new Meal { Name = "Beef Tacos", Calories = 500, Ingredients = new List<string> { "Tortillas", "Ground Beef", "Cheese", "Lettuce", "Sour Cream" } }
        };
    }

    private void PlanWeeklyMeals()
    {
        Console.WriteLine("\n=== Weekly Meal Plan ===");
        var random = new Random();
        var daysOfWeek = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        foreach (var day in daysOfWeek)
        {
            var randomMeal = _meals[random.Next(_meals.Count)];
            Console.WriteLine(day + ": " + randomMeal.Name + " (" + randomMeal.Calories + " calories)");
        }
    }

    private void GenerateGroceryList()
    {
        Console.WriteLine("\n=== Grocery List ===");
        _groceryList.Clear();

        foreach (var meal in _meals)
        {
            foreach (var ingredient in meal.Ingredients)
            {
                var existingItem = _groceryList.FirstOrDefault(item => item.Name.Equals(ingredient, StringComparison.OrdinalIgnoreCase));
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    _groceryList.Add(new GroceryItem { Name = ingredient, Quantity = 1 });
                }
            }
        }

        foreach (var item in _groceryList.OrderBy(item => item.Name))
        {
            Console.WriteLine(item.Name + " (x" + item.Quantity + ")");
        }
    }

    private void SaveMealsToFile(string filePath)
    {
        string jsonData = JsonSerializer.Serialize(_meals);
        File.WriteAllText(filePath, jsonData);
    }

    private void SaveGroceryListToFile(string filePath)
    {
        string jsonData = JsonSerializer.Serialize(_groceryList);
        File.WriteAllText(filePath, jsonData);
    }
}

public class Meal
{
    public string Name { get; set; }
    public int Calories { get; set; }
    public List<string> Ingredients { get; set; }
}

public class GroceryItem
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}