using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RecipeManager : IGeneratedModule
{
    public string Name { get; set; } = "Recipe Manager";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Recipe Manager module is running.");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        string recipesFilePath = Path.Combine(dataFolder, "recipes.json");
        List<Recipe> recipes = LoadRecipes(recipesFilePath);

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nRecipe Manager Menu:");
            Console.WriteLine("1. Add Recipe");
            Console.WriteLine("2. View Recipes");
            Console.WriteLine("3. Delete Recipe");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                switch (choice)
                {
                    case 1:
                        AddRecipe(recipes);
                        break;
                    case 2:
                        ViewRecipes(recipes);
                        break;
                    case 3:
                        DeleteRecipe(recipes);
                        break;
                    case 4:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }

            SaveRecipes(recipes, recipesFilePath);
        }

        return true;
    }

    private List<Recipe> LoadRecipes(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<Recipe>();
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Recipe>>(json) ?? new List<Recipe>();
    }

    private void SaveRecipes(List<Recipe> recipes, string filePath)
    {
        string json = JsonSerializer.Serialize(recipes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private void AddRecipe(List<Recipe> recipes)
    {
        Console.Write("Enter recipe name: ");
        string name = Console.ReadLine();

        Console.Write("Enter recipe description: ");
        string description = Console.ReadLine();

        List<Ingredient> ingredients = new List<Ingredient>();
        bool addingIngredients = true;
        while (addingIngredients)
        {
            Console.Write("Enter ingredient name (or 'done' to finish): ");
            string ingredientName = Console.ReadLine();
            if (ingredientName.ToLower() == "done")
            {
                addingIngredients = false;
                continue;
            }

            Console.Write("Enter quantity: ");
            string quantity = Console.ReadLine();

            Console.Write("Enter unit (e.g., grams, cups): ");
            string unit = Console.ReadLine();

            ingredients.Add(new Ingredient
            {
                Name = ingredientName,
                Quantity = quantity,
                Unit = unit
            });
        }

        Console.Write("Enter instructions (one step per line, type 'done' to finish):\n");
        List<string> instructions = new List<string>();
        while (true)
        {
            string step = Console.ReadLine();
            if (step.ToLower() == "done")
                break;
            instructions.Add(step);
        }

        NutritionalInfo nutritionalInfo = new NutritionalInfo();
        Console.Write("Enter calories: ");
        if (int.TryParse(Console.ReadLine(), out int calories))
            nutritionalInfo.Calories = calories;

        Console.Write("Enter protein (g): ");
        if (double.TryParse(Console.ReadLine(), out double protein))
            nutritionalInfo.Protein = protein;

        Console.Write("Enter carbs (g): ");
        if (double.TryParse(Console.ReadLine(), out double carbs))
            nutritionalInfo.Carbs = carbs;

        Console.Write("Enter fat (g): ");
        if (double.TryParse(Console.ReadLine(), out double fat))
            nutritionalInfo.Fat = fat;

        recipes.Add(new Recipe
        {
            Name = name,
            Description = description,
            Ingredients = ingredients,
            Instructions = instructions,
            NutritionalInfo = nutritionalInfo
        });

        Console.WriteLine("Recipe added successfully.");
    }

    private void ViewRecipes(List<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.");
            return;
        }

        Console.WriteLine("\nAvailable Recipes:");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine(string.Format("{0}. {1}", i + 1, recipes[i].Name));
        }

        Console.Write("\nEnter recipe number to view details (or 0 to go back): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= recipes.Count)
        {
            Recipe recipe = recipes[choice - 1];
            Console.WriteLine(string.Format("\nRecipe: {0}", recipe.Name));
            Console.WriteLine(string.Format("Description: {0}", recipe.Description));
            Console.WriteLine("\nIngredients:");
            foreach (var ingredient in recipe.Ingredients)
            {
                Console.WriteLine(string.Format("- {0} {1} {2}", ingredient.Quantity, ingredient.Unit, ingredient.Name));
            }
            Console.WriteLine("\nInstructions:");
            for (int i = 0; i < recipe.Instructions.Count; i++)
            {
                Console.WriteLine(string.Format("{0}. {1}", i + 1, recipe.Instructions[i]));
            }
            Console.WriteLine("\nNutritional Information:");
            Console.WriteLine(string.Format("Calories: {0}", recipe.NutritionalInfo.Calories));
            Console.WriteLine(string.Format("Protein: {0}g", recipe.NutritionalInfo.Protein));
            Console.WriteLine(string.Format("Carbs: {0}g", recipe.NutritionalInfo.Carbs));
            Console.WriteLine(string.Format("Fat: {0}g", recipe.NutritionalInfo.Fat));
        }
    }

    private void DeleteRecipe(List<Recipe> recipes)
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available to delete.");
            return;
        }

        Console.WriteLine("\nAvailable Recipes:");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine(string.Format("{0}. {1}", i + 1, recipes[i].Name));
        }

        Console.Write("\nEnter recipe number to delete (or 0 to cancel): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= recipes.Count)
        {
            recipes.RemoveAt(choice - 1);
            Console.WriteLine("Recipe deleted successfully.");
        }
    }
}

public class Recipe
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public List<string> Instructions { get; set; } = new List<string>();
    public NutritionalInfo NutritionalInfo { get; set; } = new NutritionalInfo();
}

public class Ingredient
{
    public string Name { get; set; }
    public string Quantity { get; set; }
    public string Unit { get; set; }
}

public class NutritionalInfo
{
    public int Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
}