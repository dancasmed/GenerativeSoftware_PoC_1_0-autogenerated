namespace GenerativeSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RecipeManager : IGeneratedModule
{
    public string Name { get; set; } = "Recipe Manager";

    public bool Main(string dataFolder)
    {
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        while (true)
        {
            Console.WriteLine("\nRecipe Manager");
            Console.WriteLine("1. Add Recipe");
            Console.WriteLine("2. View Recipes");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddRecipe(dataFolder);
                    break;
                case "2":
                    ViewRecipes(dataFolder);
                    break;
                case "3":
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void AddRecipe(string dataFolder)
    {
        Console.Write("Enter recipe name: ");
        string name = Console.ReadLine();
        Console.Write("Enter ingredients (comma separated): ");
        string ingredients = Console.ReadLine();
        Console.Write("Enter instructions: ");
        string instructions = Console.ReadLine();

        var recipe = new Recipe
        {
            Name = name,
            Ingredients = ingredients.Split(','),
            Instructions = instructions
        };

        string filePath = Path.Combine(dataFolder, $"{name}.json");
        string json = JsonSerializer.Serialize(recipe);
        File.WriteAllText(filePath, json);

        Console.WriteLine("Recipe saved successfully!");
    }

    private void ViewRecipes(string dataFolder)
    {
        var files = Directory.GetFiles(dataFolder, "*.json");
        if (files.Length == 0)
        {
            Console.WriteLine("No recipes found.");
            return;
        }

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            var recipe = JsonSerializer.Deserialize<Recipe>(json);
            Console.WriteLine("\nRecipe Name: " + recipe.Name);
            Console.WriteLine("Ingredients: " + string.Join(", ", recipe.Ingredients));
            Console.WriteLine("Instructions: " + recipe.Instructions);
        }
    }
}

public class Recipe
{
    public string Name { get; set; }
    public string[] Ingredients { get; set; }
    public string Instructions { get; set; }
}