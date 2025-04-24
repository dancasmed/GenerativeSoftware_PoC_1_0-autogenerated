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
            Console.WriteLine("4. Manage User Settings");  // Changed from "View" to "Manage"
            Console.WriteLine("5. Exit");
            Console.Write("Enter choice: ");

            switch (Console.ReadLine())
            {
                case "1": ManageMeals(); break;
                case "2": ManageMealPlans(); break;
                case "3": GenerateGroceryList(); break;
                case "4": ManageUserSettings(); break;  // Changed to ManageUserSettings
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

    // ... (Keep existing CreateMeal, ListMeals, ManageMealPlans methods unchanged)

    private void ManageUserSettings()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nUser Settings:");
            Console.WriteLine("1. View Current Settings");
            Console.WriteLine("2. Edit Settings");
            Console.WriteLine("3. Back to Main Menu");
            Console.Write("Enter choice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    ViewUserSettings();
                    break;
                case "2":
                    EditUserSettings();
                    break;
                case "3":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private void ViewUserSettings()
    {
        Console.WriteLine("\nCurrent User Settings:");
        Console.WriteLine($"Name: {currentUser.Name}");
        Console.WriteLine($"Calorie Target: {currentUser.CalorieTarget}");
        Console.WriteLine($"Dietary Preferences: {string.Join(", ", currentUser.DietaryPreferences)}");
        Console.WriteLine($"Dietary Restrictions: {string.Join(", ", currentUser.DietaryRestrictions)}");
    }

    private void EditUserSettings()
    {
        bool editing = true;
        while (editing)
        {
            Console.WriteLine("\nEdit Settings:");
            Console.WriteLine("1. Change Name");
            Console.WriteLine("2. Change Calorie Target");
            Console.WriteLine("3. Manage Dietary Preferences");
            Console.WriteLine("4. Manage Dietary Restrictions");
            Console.WriteLine("5. Finish Editing");
            Console.Write("Enter choice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Enter new name: ");
                    currentUser.Name = Console.ReadLine();
                    break;
                case "2":
                    Console.Write("Enter new calorie target: ");
                    int.TryParse(Console.ReadLine(), out int newTarget);
                    currentUser.CalorieTarget = newTarget;
                    break;
                case "3":
                    ManageDietaryItems(currentUser.DietaryPreferences, "preferences");
                    break;
                case "4":
                    ManageDietaryItems(currentUser.DietaryRestrictions, "restrictions");
                    break;
                case "5":
                    editing = false;
                    // Save updated user data
                    var users = dataStorage.LoadUsers();
                    users[users.FindIndex(u => u.Id == currentUser.Id)] = currentUser;
                    dataStorage.SaveUsers(users);
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private void ManageDietaryItems(List<string> items, string type)
    {
        bool managing = true;
        while (managing)
        {
            Console.WriteLine($"\nCurrent Dietary {type}:");
            if (items.Any())
                Console.WriteLine(string.Join(", ", items));
            else
                Console.WriteLine("None specified");

            Console.WriteLine("1. Add item");
            Console.WriteLine("2. Remove item");
            Console.WriteLine("3. Back");
            Console.Write("Enter choice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Enter item to add: ");
                    var addItem = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(addItem) && !items.Contains(addItem))
                        items.Add(addItem);
                    break;
                case "2":
                    if (items.Any())
                    {
                        Console.Write("Enter item to remove: ");
                        var removeItem = Console.ReadLine();
                        items.Remove(removeItem);
                    }
                    break;
                case "3":
                    managing = false;
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    // ... (Keep remaining existing methods unchanged)
}
