using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RestaurantManager : IGeneratedModule
{
    public string Name { get; set; } = "Restaurant Manager";
    
    private List<Restaurant> _restaurants;
    private string _dataFilePath;
    
    public RestaurantManager()
    {
        _restaurants = new List<Restaurant>();
    }
    
    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "restaurants.json");
        
        Console.WriteLine("Restaurant Manager module is running.");
        Console.WriteLine("Loading restaurant data...");
        
        LoadRestaurants();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddRestaurant();
                    break;
                case "2":
                    ListRestaurants();
                    break;
                case "3":
                    SearchByCuisine();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveRestaurants();
        Console.WriteLine("Restaurant data saved. Exiting module.");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Restaurant Manager Menu:");
        Console.WriteLine("1. Add a new restaurant");
        Console.WriteLine("2. List all restaurants");
        Console.WriteLine("3. Search by cuisine type");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }
    
    private void AddRestaurant()
    {
        Console.Write("Enter restaurant name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter cuisine type: ");
        string cuisine = Console.ReadLine();
        
        _restaurants.Add(new Restaurant { Name = name, CuisineType = cuisine });
        Console.WriteLine("Restaurant added successfully.");
    }
    
    private void ListRestaurants()
    {
        if (_restaurants.Count == 0)
        {
            Console.WriteLine("No restaurants found.");
            return;
        }
        
        Console.WriteLine("Restaurant List:");
        foreach (var restaurant in _restaurants)
        {
            Console.WriteLine(restaurant.ToString());
        }
    }
    
    private void SearchByCuisine()
    {
        Console.Write("Enter cuisine type to search: ");
        string cuisine = Console.ReadLine();
        
        var results = _restaurants.FindAll(r => r.CuisineType.Equals(cuisine, StringComparison.OrdinalIgnoreCase));
        
        if (results.Count == 0)
        {
            Console.WriteLine("No restaurants found with that cuisine type.");
            return;
        }
        
        Console.WriteLine("Found restaurants:");
        foreach (var restaurant in results)
        {
            Console.WriteLine(restaurant.ToString());
        }
    }
    
    private void LoadRestaurants()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _restaurants = JsonSerializer.Deserialize<List<Restaurant>>(json);
                Console.WriteLine("Restaurant data loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading restaurant data: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("No existing restaurant data found. Starting with empty list.");
        }
    }
    
    private void SaveRestaurants()
    {
        try
        {
            string json = JsonSerializer.Serialize(_restaurants);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving restaurant data: " + ex.Message);
        }
    }
}

public class Restaurant
{
    public string Name { get; set; }
    public string CuisineType { get; set; }
    
    public override string ToString()
    {
        return "Name: " + Name + ", Cuisine: " + CuisineType;
    }
}