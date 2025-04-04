using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class RecipeManager : IGeneratedModule
{
    public string Name { get; set; } = "Recipe Manager";
    
    private List<Recipe> _recipes;
    private string _dataFilePath;
    
    public RecipeManager()
    {
        _recipes = new List<Recipe>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Recipe Manager module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "recipes.json");
        
        try
        {
            LoadRecipes();
            
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nRecipe Manager Menu:");
                Console.WriteLine("1. Add Recipe");
                Console.WriteLine("2. List All Recipes");
                Console.WriteLine("3. Search Recipes by Ingredient");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");
                
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddRecipe();
                        break;
                    case "2":
                        ListAllRecipes();
                        break;
                    case "3":
                        SearchRecipesByIngredient();
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
            Console.WriteLine("Recipe Manager module finished.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void LoadRecipes()
    {
        if (File.Exists(_dataFilePath))
        {
            var json = File.ReadAllText(_dataFilePath);
            _recipes = JsonSerializer.Deserialize<List<Recipe>>(json) ?? new List<Recipe>();
            Console.WriteLine("Recipes loaded successfully.");
        }
        else
        {
            Console.WriteLine("No existing recipes found. Starting with empty collection.");
        }
    }
    
    private void SaveRecipes()
    {
        var json = JsonSerializer.Serialize(_recipes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFilePath, json);
        Console.WriteLine("Recipes saved successfully.");
    }
    
    private void AddRecipe()
    {
        Console.Write("Enter recipe name: ");
        var name = Console.ReadLine();
        
        Console.Write("Enter recipe description: ");
        var description = Console.ReadLine();
        
        var ingredients = new List<string>();
        Console.WriteLine("Enter ingredients (one per line, empty line to finish):");
        while (true)
        {
            var ingredient = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ingredient))
                break;
            ingredients.Add(ingredient);
        }
        
        _recipes.Add(new Recipe
        {
            Name = name,
            Description = description,
            Ingredients = ingredients
        });
        
        Console.WriteLine("Recipe added successfully.");
    }
    
    private void ListAllRecipes()
    {
        if (_recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.");
            return;
        }
        
        Console.WriteLine("\nAll Recipes:");
        foreach (var recipe in _recipes)
        {
            Console.WriteLine("Name: " + recipe.Name);
            Console.WriteLine("Description: " + recipe.Description);
            Console.WriteLine("Ingredients: " + string.Join(", ", recipe.Ingredients));
            Console.WriteLine();
        }
    }
    
    private void SearchRecipesByIngredient()
    {
        Console.Write("Enter ingredient to search: ");
        var searchTerm = Console.ReadLine()?.Trim().ToLower();
        
        if (string.IsNullOrEmpty(searchTerm))
        {
            Console.WriteLine("Search term cannot be empty.");
            return;
        }
        
        var matchingRecipes = _recipes
            .Where(r => r.Ingredients.Any(i => i.ToLower().Contains(searchTerm)))
            .ToList();
        
        if (matchingRecipes.Count == 0)
        {
            Console.WriteLine("No recipes found with that ingredient.");
            return;
        }
        
        Console.WriteLine("\nMatching Recipes:");
        foreach (var recipe in matchingRecipes)
        {
            Console.WriteLine("Name: " + recipe.Name);
            Console.WriteLine("Description: " + recipe.Description);
            Console.WriteLine("Ingredients: " + string.Join(", ", recipe.Ingredients));
            Console.WriteLine();
        }
    }
}

public class Recipe
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Ingredients { get; set; } = new List<string>();
}