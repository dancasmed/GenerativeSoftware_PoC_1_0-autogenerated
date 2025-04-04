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
        Console.WriteLine("Recipe Manager module is running.");
        
        _dataFilePath = Path.Combine(dataFolder, "recipes.json");
        
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _recipes = JsonSerializer.Deserialize<List<Recipe>>(json) ?? new List<Recipe>();
                Console.WriteLine("Recipes loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading recipes: " + ex.Message);
                return false;
            }
        }
        else
        {
            Console.WriteLine("No existing recipe data found. Starting with empty collection.");
        }
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nRecipe Manager Menu:");
            Console.WriteLine("1. Add new recipe");
            Console.WriteLine("2. List all recipes");
            Console.WriteLine("3. Search recipes by ingredient");
            Console.WriteLine("4. Save and exit");
            Console.Write("Enter your choice: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddRecipe();
                    break;
                case "2":
                    ListRecipes();
                    break;
                case "3":
                    SearchByIngredient();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        
        try
        {
            string json = JsonSerializer.Serialize(_recipes);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Recipes saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving recipes: " + ex.Message);
            return false;
        }
    }
    
    private void AddRecipe()
    {
        Console.Write("Enter recipe name: ");
        string name = Console.ReadLine();
        
        List<string> ingredients = new List<string>();
        Console.WriteLine("Enter ingredients (one per line, empty line to finish):");
        
        while (true)
        {
            string ingredient = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ingredient))
                break;
            
            ingredients.Add(ingredient);
        }
        
        Console.Write("Enter instructions: ");
        string instructions = Console.ReadLine();
        
        _recipes.Add(new Recipe
        {
            Name = name,
            Ingredients = ingredients,
            Instructions = instructions
        });
        
        Console.WriteLine("Recipe added successfully.");
    }
    
    private void ListRecipes()
    {
        if (_recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.");
            return;
        }
        
        Console.WriteLine("\nAvailable Recipes:");
        foreach (var recipe in _recipes)
        {
            Console.WriteLine("Name: " + recipe.Name);
            Console.WriteLine("Ingredients: " + string.Join(", ", recipe.Ingredients));
            Console.WriteLine("Instructions: " + recipe.Instructions);
            Console.WriteLine();
        }
    }
    
    private void SearchByIngredient()
    {
        Console.Write("Enter ingredient to search for: ");
        string searchTerm = Console.ReadLine().ToLower();
        
        var matchingRecipes = _recipes
            .Where(r => r.Ingredients.Any(i => i.ToLower().Contains(searchTerm)))
            .ToList();
        
        if (matchingRecipes.Count == 0)
        {
            Console.WriteLine("No recipes found containing that ingredient.");
            return;
        }
        
        Console.WriteLine("\nRecipes containing " + searchTerm + ":");
        foreach (var recipe in matchingRecipes)
        {
            Console.WriteLine("Name: " + recipe.Name);
            Console.WriteLine("Ingredients: " + string.Join(", ", recipe.Ingredients));
            Console.WriteLine();
        }
    }
}

public class Recipe
{
    public string Name { get; set; }
    public List<string> Ingredients { get; set; }
    public string Instructions { get; set; }
}