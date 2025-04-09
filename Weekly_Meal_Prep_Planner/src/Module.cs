using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MealPrepPlanner : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Meal Prep Planner";

    private string _dataFilePath;
    private List<MealPlan> _mealPlans;

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Weekly Meal Prep Planner module is running...");
            _dataFilePath = Path.Combine(dataFolder, "mealPlans.json");
            _mealPlans = LoadMealPlans();

            bool continueRunning = true;
            while (continueRunning)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddMealPlan();
                        break;
                    case "2":
                        ViewMealPlans();
                        break;
                    case "3":
                        DeleteMealPlan();
                        break;
                    case "4":
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

            SaveMealPlans();
            Console.WriteLine("Meal plans saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nWeekly Meal Prep Planner");
        Console.WriteLine("1. Add Meal Plan");
        Console.WriteLine("2. View Meal Plans");
        Console.WriteLine("3. Delete Meal Plan");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private void AddMealPlan()
    {
        Console.Write("Enter day of the week (e.g., Monday): ");
        string day = Console.ReadLine();

        Console.Write("Enter meal description: ");
        string description = Console.ReadLine();

        Console.Write("Enter estimated calories: ");
        if (!int.TryParse(Console.ReadLine(), out int calories))
        {
            Console.WriteLine("Invalid calorie count. Please enter a number.");
            return;
        }

        _mealPlans.Add(new MealPlan { Day = day, Description = description, Calories = calories });
        Console.WriteLine("Meal plan added successfully.");
    }

    private void ViewMealPlans()
    {
        if (_mealPlans.Count == 0)
        {
            Console.WriteLine("No meal plans available.");
            return;
        }

        Console.WriteLine("\nWeekly Meal Plans:");
        foreach (var plan in _mealPlans)
        {
            Console.WriteLine($"{plan.Day}: {plan.Description} ({plan.Calories} calories)");
        }

        int totalCalories = 0;
        foreach (var plan in _mealPlans)
        {
            totalCalories += plan.Calories;
        }
        Console.WriteLine($"\nTotal estimated calories for the week: {totalCalories}");
    }

    private void DeleteMealPlan()
    {
        if (_mealPlans.Count == 0)
        {
            Console.WriteLine("No meal plans to delete.");
            return;
        }

        ViewMealPlans();
        Console.Write("Enter the index of the meal plan to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < _mealPlans.Count)
        {
            _mealPlans.RemoveAt(index);
            Console.WriteLine("Meal plan deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid index.");
        }
    }

    private List<MealPlan> LoadMealPlans()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<MealPlan>>(json) ?? new List<MealPlan>();
        }
        return new List<MealPlan>();
    }

    private void SaveMealPlans()
    {
        string json = JsonSerializer.Serialize(_mealPlans);
        File.WriteAllText(_dataFilePath, json);
    }
}

public class MealPlan
{
    public string Day { get; set; }
    public string Description { get; set; }
    public int Calories { get; set; }
}