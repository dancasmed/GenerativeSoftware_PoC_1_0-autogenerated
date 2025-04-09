using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RecipeManager : IGeneratedModule
{
    public string Name { get; set; } = "Recipe Manager";

    private List<Recipe> recipes;
    private string recipesFilePath;

    public RecipeManager()
    {
        recipes = new List<Recipe>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Recipe Manager Module is running...");

        recipesFilePath = Path.Combine(dataFolder, "recipes.json");

        LoadRecipes();

        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddRecipe();
                    break;
                case "2":
                    ViewRecipes();
                    break;
                case "3":
                    DeleteRecipe();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveRecipes();
        Console.WriteLine("Recipe Manager Module finished.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nRecipe Manager Menu:");
        Console.WriteLine("1. Add a new recipe");
        Console.WriteLine("2. View all recipes");
        Console.WriteLine("3. Delete a recipe");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddRecipe()
    {
        Console.Write("Enter recipe name: ");
        string name = Console.ReadLine();

        var ingredients = new List<string>();
        Console.WriteLine("Enter ingredients (one per line, empty line to finish):");
        while (true)
        {
            string ingredient = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ingredient))
                break;
            ingredients.Add(ingredient);
        }

        var steps = new List<string>();
        Console.WriteLine("Enter preparation steps (one per line, empty line to finish):");
        while (true)
        {
            string step = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(step))
                break;
            steps.Add(step);
        }

        recipes.Add(new Recipe(name, ingredients, steps));
        Console.WriteLine("Recipe added successfully!");
    }

    private void ViewRecipes()
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.");
            return;
        }

        Console.WriteLine("\nAvailable Recipes:");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {recipes[i].Name}");
        }

        Console.Write("\nEnter recipe number to view details (0 to go back): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= recipes.Count)
        {
            var recipe = recipes[choice - 1];
            Console.WriteLine($"\nRecipe: {recipe.Name}");
            Console.WriteLine("\nIngredients:");
            foreach (var ingredient in recipe.Ingredients)
            {
                Console.WriteLine("- " + ingredient);
            }

            Console.WriteLine("\nPreparation Steps:");
            for (int i = 0; i < recipe.Steps.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {recipe.Steps[i]}");
            }
        }
    }

    private void DeleteRecipe()
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available to delete.");
            return;
        }

        ViewRecipes();
        Console.Write("\nEnter recipe number to delete (0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= recipes.Count)
        {
            recipes.RemoveAt(choice - 1);
            Console.WriteLine("Recipe deleted successfully!");
        }
    }

    private void LoadRecipes()
    {
        if (File.Exists(recipesFilePath))
        {
            try
            {
                string json = File.ReadAllText(recipesFilePath);
                recipes = JsonSerializer.Deserialize<List<Recipe>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading recipes: {ex.Message}");
            }
        }
    }

    private void SaveRecipes()
    {
        try
        {
            string json = JsonSerializer.Serialize(recipes);
            File.WriteAllText(recipesFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving recipes: {ex.Message}");
        }
    }
}

public class Recipe
{
    public string Name { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string> Steps { get; set; }

    public Recipe(string name, List<string> ingredients, List<string> steps)
    {
        Name = name;
        Ingredients = ingredients;
        Steps = steps;
    }

    // Parameterless constructor for JSON deserialization
    public Recipe()
    {
        Ingredients = new List<string>();
        Steps = new List<string>();
    }
}