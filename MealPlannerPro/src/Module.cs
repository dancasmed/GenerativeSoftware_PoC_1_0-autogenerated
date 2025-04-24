using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class Meal
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public int Calories { get; set; }
    public int PreparationTime { get; set; }
    public List<string> DietaryTags { get; set; }
}

public class Ingredient
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; }
}

public class MealPlan
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Day> Days { get; set; }
}

public class Day
{
    public DateTime Date { get; set; }
    public List<Meal> Meals { get; set; }
    public int TotalCalories { get; set; }
}

public class GroceryList
{
    public string Id { get; set; }
    public string MealPlanId { get; set; }
    public List<GroceryItem> Items { get; set; }
    public int TotalItems { get; set; }
}

public class GroceryItem
{
    public string Id { get; set; }
    public string IngredientId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; }
    public bool Checked { get; set; }
}

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> DietaryPreferences { get; set; }
    public List<string> DietaryRestrictions { get; set; }
    public int CalorieTarget { get; set; }
}

public class DataStorage
{
    private string dataFolder;
    private JsonSerializerOptions jsonOptions;

    public DataStorage(string folder)
    {
        dataFolder = folder;
        jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
    }

    public List<User> LoadUsers() => Load<List<User>>("users.json");
    public List<Meal> LoadMeals() => Load<List<Meal>>("meals.json");
    public List<MealPlan> LoadMealPlans() => Load<List<MealPlan>>("mealplans.json");
    public List<GroceryList> LoadGroceryLists() => Load<List<GroceryList>>("grocerylists.json");

    public void SaveUsers(List<User> data) => Save("users.json", data);
    public void SaveMeals(List<Meal> data) => Save("meals.json", data);
    public void SaveMealPlans(List<MealPlan> data) => Save("mealplans.json", data);
    public void SaveGroceryLists(List<GroceryList> data) => Save("grocerylists.json", data);

    private T Load<T>(string filename) where T : new()
    {
        string path = Path.Combine(dataFolder, filename);
        if (File.Exists(path))
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path), jsonOptions);
        return new T();
    }

    private void Save<T>(string filename, T data)
    {
        string path = Path.Combine(dataFolder, filename);
        File.WriteAllText(path, JsonSerializer.Serialize(data, jsonOptions));
    }
}

public class MealPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Meal Planner Module";
    private DataStorage dataStorage;
    private User currentUser;
    private List<Meal> meals;
    private List<MealPlan> mealPlans;
    private List<GroceryList> groceryLists;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Meal Planner Module is running.");
        dataStorage = new DataStorage(dataFolder);
        LoadData();
        InitializeUser();
        MainMenu();
        return true;
    }

    private void LoadData()
    {
        meals = dataStorage.LoadMeals();
        mealPlans = dataStorage.LoadMealPlans();
        groceryLists = dataStorage.LoadGroceryLists();
    }

    private void SaveData()
    {
        dataStorage.SaveMeals(meals);
        dataStorage.SaveMealPlans(mealPlans);
        dataStorage.SaveGroceryLists(groceryLists);
    }

    private void InitializeUser()
    {
        var users = dataStorage.LoadUsers();
        if (!users.Any())
        {
            currentUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Default User",
                DietaryPreferences = new List<string>(),
                DietaryRestrictions = new List<string>(),
                CalorieTarget = 2000
            };
            users.Add(currentUser);
            dataStorage.SaveUsers(users);
        }
        else
        {
            currentUser = users.First();
        }
    }

    private void MainMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Manage Meals");
            Console.WriteLine("2. Manage Meal Plans");
            Console.WriteLine("3. Generate Grocery List");
            Console.WriteLine("4. View User Settings");
            Console.WriteLine("5. Exit");
            Console.Write("Enter choice: ");

            switch (Console.ReadLine())
            {
                case "1": ManageMeals(); break;
                case "2": ManageMealPlans(); break;
                case "3": GenerateGroceryList(); break;
                case "4": ViewUserSettings(); break;
                case "5": exit = true; break;
                default: Console.WriteLine("Invalid option"); break;
            }
        }
        SaveData();
    }

    private void ManageMeals()
    {
        Console.WriteLine("\nManage Meals:");
        Console.WriteLine("1. Create Meal");
        Console.WriteLine("2. List Meals");
        Console.Write("Enter choice: ");

        switch (Console.ReadLine())
        {
            case "1": CreateMeal(); break;
            case "2": ListMeals(); break;
            default: Console.WriteLine("Invalid option"); break;
        }
    }

    private void CreateMeal()
    {
        var meal = new Meal
        {
            Id = Guid.NewGuid().ToString(),
            Ingredients = new List<Ingredient>()
        };

        Console.Write("Enter meal name: ");
        meal.Name = Console.ReadLine();
        Console.Write("Enter description: ");
        meal.Description = Console.ReadLine();
        Console.Write("Enter calories: ");
        int.TryParse(Console.ReadLine(), out int calories);
        meal.Calories = calories;

        meals.Add(meal);
        Console.WriteLine("Meal created successfully.");
    }

    private void ListMeals()
    {
        Console.WriteLine("\nAll Meals:");
        foreach (var meal in meals)
            Console.WriteLine(meal.Name + " - " + meal.Calories + " calories");
    }

    private void ManageMealPlans()
    {
        Console.WriteLine("\nManage Meal Plans:");
        Console.WriteLine("1. Create Meal Plan");
        Console.WriteLine("2. View Meal Plans");
        Console.Write("Enter choice: ");

        switch (Console.ReadLine())
        {
            case "1": CreateMealPlan(); break;
            case "2": ViewMealPlans(); break;
            default: Console.WriteLine("Invalid option"); break;
        }
    }

    private void CreateMealPlan()
    {
        var plan = new MealPlan
        {
            Id = Guid.NewGuid().ToString(),
            UserId = currentUser.Id,
            Days = new List<Day>()
        };

        Console.Write("Enter start date (yyyy-mm-dd): ");
        DateTime.TryParse(Console.ReadLine(), out DateTime startDate);
        plan.StartDate = startDate;
        plan.EndDate = startDate.AddDays(6);

        mealPlans.Add(plan);
        Console.WriteLine("Meal plan created for week starting " + startDate.ToShortDateString());
    }

    private void ViewMealPlans()
    {
        Console.WriteLine("\nCurrent Meal Plans:");
        foreach (var plan in mealPlans)
            Console.WriteLine(plan.StartDate.ToShortDateString() + " to " + plan.EndDate.ToShortDateString());
    }

    private void GenerateGroceryList()
    {
        if (!mealPlans.Any())
        {
            Console.WriteLine("No meal plans available");
            return;
        }

        var plan = mealPlans.Last();
        var ingredients = plan.Days
            .SelectMany(d => d.Meals)
            .SelectMany(m => m.Ingredients)
            .GroupBy(i => i.Name)
            .Select(g => new GroceryItem
            {
                Id = Guid.NewGuid().ToString(),
                IngredientId = g.First().Id,
                Name = g.Key,
                Category = g.First().Category,
                Quantity = g.Sum(i => i.Quantity),
                Unit = g.First().Unit
            }).ToList();

        var groceryList = new GroceryList
        {
            Id = Guid.NewGuid().ToString(),
            MealPlanId = plan.Id,
            Items = ingredients,
            TotalItems = ingredients.Count
        };

        groceryLists.Add(groceryList);
        Console.WriteLine("Grocery list generated with " + ingredients.Count + " items");
    }

    private void ViewUserSettings()
    {
        Console.WriteLine("\nUser Settings:");
        Console.WriteLine("Name: " + currentUser.Name);
        Console.WriteLine("Calorie Target: " + currentUser.CalorieTarget);
        Console.WriteLine("Dietary Restrictions: " + string.Join(", ", currentUser.DietaryRestrictions));
    }
}
