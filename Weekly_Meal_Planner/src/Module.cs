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
            // Ensure data directory exists
            Directory.CreateDirectory(dataFolder);
            
            // File paths
            string mealsFilePath = Path.Combine(dataFolder, "meals.json");
            string groceryListFilePath = Path.Combine(dataFolder, "grocery_list.json");
            
            // Sample meal data (could be loaded from file if exists)
            List<Meal> meals = LoadOrCreateMeals(mealsFilePath);
            
            // Generate weekly plan
            WeeklyPlan weeklyPlan = GenerateWeeklyPlan(meals);
            
            // Generate grocery list
            List<GroceryItem> groceryList = GenerateGroceryList(weeklyPlan);
            
            // Save data
            SaveWeeklyPlan(weeklyPlan, dataFolder);
            SaveGroceryList(groceryList, groceryListFilePath);
            
            // Display summary
            DisplaySummary(weeklyPlan, groceryList);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private List<Meal> LoadOrCreateMeals(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Meal>>(json);
        }
        
        // Create sample meals if file doesn't exist
        var meals = new List<Meal>
        {
            new Meal { Name = "Grilled Chicken Salad", Calories = 450, Ingredients = new List<string> { "Chicken Breast", "Lettuce", "Tomato", "Cucumber", "Olive Oil" } },
            new Meal { Name = "Vegetable Stir Fry", Calories = 350, Ingredients = new List<string> { "Broccoli", "Carrots", "Bell Peppers", "Soy Sauce", "Rice" } },
            new Meal { Name = "Pasta Carbonara", Calories = 600, Ingredients = new List<string> { "Pasta", "Bacon", "Eggs", "Parmesan", "Black Pepper" } },
            new Meal { Name = "Beef Tacos", Calories = 500, Ingredients = new List<string> { "Ground Beef", "Taco Shells", "Lettuce", "Cheese", "Salsa" } },
            new Meal { Name = "Salmon with Vegetables", Calories = 400, Ingredients = new List<string> { "Salmon Fillet", "Asparagus", "Lemon", "Butter", "Potatoes" } }
        };
        
        File.WriteAllText(filePath, JsonSerializer.Serialize(meals));
        return meals;
    }
    
    private WeeklyPlan GenerateWeeklyPlan(List<Meal> meals)
    {
        var random = new Random();
        var plan = new WeeklyPlan();
        
        var days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        
        foreach (var day in days)
        {
            // Select 2 different random meals for each day (lunch and dinner)
            var availableMeals = meals.Where(m => !plan.DailyPlans.Any(dp => dp.Meal1 == m || dp.Meal2 == m)).ToList();
            
            if (availableMeals.Count < 2)
                availableMeals = meals.ToList(); // Reset if we're running out of unique meals
            
            int index1 = random.Next(availableMeals.Count);
            int index2;
            do { index2 = random.Next(availableMeals.Count); } while (index2 == index1);
            
            plan.DailyPlans.Add(new DailyPlan 
            { 
                Day = day, 
                Meal1 = availableMeals[index1], 
                Meal2 = availableMeals[index2] 
            });
        }
        
        return plan;
    }
    
    private List<GroceryItem> GenerateGroceryList(WeeklyPlan weeklyPlan)
    {
        var groceryItems = new Dictionary<string, GroceryItem>();
        
        foreach (var dayPlan in weeklyPlan.DailyPlans)
        {
            AddMealIngredientsToGroceryList(dayPlan.Meal1, groceryItems);
            AddMealIngredientsToGroceryList(dayPlan.Meal2, groceryItems);
        }
        
        return groceryItems.Values.ToList();
    }
    
    private void AddMealIngredientsToGroceryList(Meal meal, Dictionary<string, GroceryItem> groceryItems)
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
    
    private void SaveWeeklyPlan(WeeklyPlan plan, string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, "weekly_plan.json");
        File.WriteAllText(filePath, JsonSerializer.Serialize(plan));
    }
    
    private void SaveGroceryList(List<GroceryItem> groceryList, string filePath)
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(groceryList));
    }
    
    private void DisplaySummary(WeeklyPlan weeklyPlan, List<GroceryItem> groceryList)
    {
        Console.WriteLine("\n=== Weekly Meal Plan ===");
        
        foreach (var dayPlan in weeklyPlan.DailyPlans)
        {
            Console.WriteLine(dayPlan.Day + ":");
            Console.WriteLine("  Lunch: " + dayPlan.Meal1.Name + " (" + dayPlan.Meal1.Calories + " calories)");
            Console.WriteLine("  Dinner: " + dayPlan.Meal2.Name + " (" + dayPlan.Meal2.Calories + " calories)");
        }
        
        Console.WriteLine("\nTotal estimated calories for the week: " + 
            weeklyPlan.DailyPlans.Sum(dp => dp.Meal1.Calories + dp.Meal2.Calories));
        
        Console.WriteLine("\n=== Grocery List ===");
        foreach (var item in groceryList.OrderBy(i => i.Name))
        {
            Console.WriteLine(item.Name + " (x" + item.Quantity + ")");
        }
    }
}

public class Meal
{
    public string Name { get; set; }
    public int Calories { get; set; }
    public List<string> Ingredients { get; set; }
}

public class DailyPlan
{
    public DayOfWeek Day { get; set; }
    public Meal Meal1 { get; set; }
    public Meal Meal2 { get; set; }
}

public class WeeklyPlan
{
    public List<DailyPlan> DailyPlans { get; set; } = new List<DailyPlan>();
}

public class GroceryItem
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}