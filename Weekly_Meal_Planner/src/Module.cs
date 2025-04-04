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
            string mealsFilePath = Path.Combine(dataFolder, "meals.json");
            string groceryListFilePath = Path.Combine(dataFolder, "grocery_list.json");

            List<Meal> weeklyMeals = GenerateWeeklyMeals();
            SaveMealsToFile(weeklyMeals, mealsFilePath);
            
            List<GroceryItem> groceryList = GenerateGroceryList(weeklyMeals);
            SaveGroceryListToFile(groceryList, groceryListFilePath);
            
            Console.WriteLine("Weekly meal planning completed successfully.");
            Console.WriteLine("Meals saved to: " + mealsFilePath);
            Console.WriteLine("Grocery list saved to: " + groceryListFilePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred during meal planning: " + ex.Message);
            return false;
        }
    }

    private List<Meal> GenerateWeeklyMeals()
    {
        var meals = new List<Meal>
        {
            new Meal { Day = "Monday", Name = "Grilled Chicken Salad", Calories = 450, Ingredients = new List<string> { "Chicken breast", "Mixed greens", "Cherry tomatoes", "Cucumber", "Olive oil" } },
            new Meal { Day = "Tuesday", Name = "Vegetable Stir Fry", Calories = 380, Ingredients = new List<string> { "Broccoli", "Bell peppers", "Carrots", "Tofu", "Soy sauce" } },
            new Meal { Day = "Wednesday", Name = "Spaghetti Bolognese", Calories = 520, Ingredients = new List<string> { "Ground beef", "Spaghetti", "Tomato sauce", "Onion", "Garlic" } },
            new Meal { Day = "Thursday", Name = "Salmon with Roasted Vegetables", Calories = 480, Ingredients = new List<string> { "Salmon fillet", "Asparagus", "Sweet potatoes", "Lemon" } },
            new Meal { Day = "Friday", Name = "Vegetable Curry", Calories = 410, Ingredients = new List<string> { "Chickpeas", "Coconut milk", "Curry powder", "Cauliflower", "Peas" } },
            new Meal { Day = "Saturday", Name = "Homemade Pizza", Calories = 550, Ingredients = new List<string> { "Pizza dough", "Tomato sauce", "Mozzarella", "Pepperoni", "Mushrooms" } },
            new Meal { Day = "Sunday", Name = "Beef Stew", Calories = 490, Ingredients = new List<string> { "Beef chunks", "Potatoes", "Carrots", "Onion", "Beef broth" } }
        };

        return meals;
    }

    private List<GroceryItem> GenerateGroceryList(List<Meal> meals)
    {
        var groceryItems = new Dictionary<string, GroceryItem>();
        
        foreach (var meal in meals)
        {
            foreach (var ingredient in meal.Ingredients)
            {
                if (groceryItems.ContainsKey(ingredient))
                {
                    groceryItems[ingredient].Quantity++;
                }
                else
                {
                    groceryItems.Add(ingredient, new GroceryItem { Name = ingredient, Quantity = 1 });
                }
            }
        }
        
        return new List<GroceryItem>(groceryItems.Values);
    }

    private void SaveMealsToFile(List<Meal> meals, string filePath)
    {
        string json = JsonSerializer.Serialize(meals, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private void SaveGroceryListToFile(List<GroceryItem> groceryList, string filePath)
    {
        string json = JsonSerializer.Serialize(groceryList, new JsonSerializerOptions { WriteIndented = true });
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
    public int Quantity { get; set; }
}