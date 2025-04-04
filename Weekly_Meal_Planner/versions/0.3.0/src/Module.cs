using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MealPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Meal Planner";

    private List<Recipe> _recipes;
    private List<MealPlan> _mealPlans;
    private string _recipesFilePath;
    private string _mealPlansFilePath;

    public MealPlannerModule()
    {
        _recipes = new List<Recipe>();
        _mealPlans = new List<MealPlan>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Weekly Meal Planner module is running...");

        _recipesFilePath = Path.Combine(dataFolder, "recipes.json");
        _mealPlansFilePath = Path.Combine(dataFolder, "mealPlans.json");

        LoadRecipes();
        LoadMealPlans();

        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Recipe");
            Console.WriteLine("2. View Recipes");
            Console.WriteLine("3. Generate Meal Plan");
            Console.WriteLine("4. View Meal Plans");
            Console.WriteLine("5. Generate Grocery List");
            Console.WriteLine("6. Exit");

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddRecipe();
                    break;
                case "2":
                    ViewRecipes();
                    break;
                case "3":
                    GenerateMealPlan();
                    break;
                case "4":
                    ViewMealPlans();
                    break;
                case "5":
                    GenerateGroceryList();
                    break;
                case "6":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        SaveRecipes();
        SaveMealPlans();

        Console.WriteLine("Weekly Meal Planner module finished.");
        return true;
    }

    private void LoadRecipes()
    {
        if (File.Exists(_recipesFilePath))
        {
            string json = File.ReadAllText(_recipesFilePath);
            _recipes = JsonSerializer.Deserialize<List<Recipe>>(json);
        }
    }

    private void SaveRecipes()
    {
        string json = JsonSerializer.Serialize(_recipes);
        File.WriteAllText(_recipesFilePath, json);
    }

    private void LoadMealPlans()
    {
        if (File.Exists(_mealPlansFilePath))
        {
            string json = File.ReadAllText(_mealPlansFilePath);
            _mealPlans = JsonSerializer.Deserialize<List<MealPlan>>(json);
        }
    }

    private void SaveMealPlans()
    {
        string json = JsonSerializer.Serialize(_mealPlans);
        File.WriteAllText(_mealPlansFilePath, json);
    }

    private void AddRecipe()
    {
        Console.Write("Enter recipe name: ");
        string name = Console.ReadLine();

        Console.Write("Enter number of ingredients: ");
        int ingredientCount = int.Parse(Console.ReadLine());

        List<string> ingredients = new List<string>();
        for (int i = 0; i < ingredientCount; i++)
        {
            Console.Write("Enter ingredient " + (i + 1) + ": ");
            ingredients.Add(Console.ReadLine());
        }

        _recipes.Add(new Recipe { Name = name, Ingredients = ingredients });
        Console.WriteLine("Recipe added successfully.");
    }

    private void ViewRecipes()
    {
        Console.WriteLine("\nAvailable Recipes:");
        for (int i = 0; i < _recipes.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + _recipes[i].Name);
        }
    }

    private void GenerateMealPlan()
    {
        if (_recipes.Count == 0)
        {
            Console.WriteLine("No recipes available. Please add recipes first.");
            return;
        }

        Console.Write("Enter meal plan name: ");
        string name = Console.ReadLine();

        Dictionary<DayOfWeek, string> meals = new Dictionary<DayOfWeek, string>();

        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            ViewRecipes();
            Console.Write("Select recipe for " + day + " (enter number): ");
            int recipeIndex = int.Parse(Console.ReadLine()) - 1;

            if (recipeIndex >= 0 && recipeIndex < _recipes.Count)
            {
                meals.Add(day, _recipes[recipeIndex].Name);
            }
            else
            {
                Console.WriteLine("Invalid recipe selection.");
                return;
            }
        }

        _mealPlans.Add(new MealPlan { Name = name, Meals = meals });
        Console.WriteLine("Meal plan generated successfully.");
    }

    private void ViewMealPlans()
    {
        Console.WriteLine("\nAvailable Meal Plans:");
        for (int i = 0; i < _mealPlans.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + _mealPlans[i].Name);
            foreach (var meal in _mealPlans[i].Meals)
            {
                Console.WriteLine("   " + meal.Key + ": " + meal.Value);
            }
        }
    }

    private void GenerateGroceryList()
    {
        if (_mealPlans.Count == 0)
        {
            Console.WriteLine("No meal plans available. Please generate a meal plan first.");
            return;
        }

        ViewMealPlans();
        Console.Write("Select meal plan to generate grocery list (enter number): ");
        int planIndex = int.Parse(Console.ReadLine()) - 1;

        if (planIndex < 0 || planIndex >= _mealPlans.Count)
        {
            Console.WriteLine("Invalid meal plan selection.");
            return;
        }

        MealPlan selectedPlan = _mealPlans[planIndex];
        HashSet<string> groceryItems = new HashSet<string>();

        foreach (var meal in selectedPlan.Meals)
        {
            Recipe recipe = _recipes.Find(r => r.Name == meal.Value);
            if (recipe != null)
            {
                foreach (string ingredient in recipe.Ingredients)
                {
                    groceryItems.Add(ingredient);
                }
            }
        }

        Console.WriteLine("\nGrocery List:");
        foreach (string item in groceryItems)
        {
            Console.WriteLine("- " + item);
        }
    }
}

public class Recipe
{
    public string Name { get; set; }
    public List<string> Ingredients { get; set; }
}

public class MealPlan
{
    public string Name { get; set; }
    public Dictionary<DayOfWeek, string> Meals { get; set; }
}