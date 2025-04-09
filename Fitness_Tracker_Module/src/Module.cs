using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FitnessTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Fitness Tracker Module";

    private string _dataFilePath;
    private FitnessData _fitnessData;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Fitness Tracker Module...");
        _dataFilePath = Path.Combine(dataFolder, "fitness_data.json");
        
        LoadData();
        
        bool running = true;
        while (running)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddWorkout();
                    break;
                case "2":
                    AddSteps();
                    break;
                case "3":
                    AddCaloriesBurned();
                    break;
                case "4":
                    ViewProgress();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveData();
        }
        
        Console.WriteLine("Fitness Tracker Module completed successfully.");
        return true;
    }
    
    private void LoadData()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _fitnessData = JsonSerializer.Deserialize<FitnessData>(jsonData);
            }
            else
            {
                _fitnessData = new FitnessData
                {
                    Workouts = new List<Workout>(),
                    DailySteps = new Dictionary<DateTime, int>(),
                    DailyCalories = new Dictionary<DateTime, int>()
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading fitness data: " + ex.Message);
            _fitnessData = new FitnessData
            {
                Workouts = new List<Workout>(),
                DailySteps = new Dictionary<DateTime, int>(),
                DailyCalories = new Dictionary<DateTime, int>()
            };
        }
    }
    
    private void SaveData()
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(_fitnessData);
            File.WriteAllText(_dataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving fitness data: " + ex.Message);
        }
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nFitness Tracker Menu:");
        Console.WriteLine("1. Add Workout");
        Console.WriteLine("2. Add Steps");
        Console.WriteLine("3. Add Calories Burned");
        Console.WriteLine("4. View Progress");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddWorkout()
    {
        Console.Write("Enter workout type: ");
        string type = Console.ReadLine();
        
        Console.Write("Enter duration in minutes: ");
        if (!int.TryParse(Console.ReadLine(), out int duration))
        {
            Console.WriteLine("Invalid duration. Please enter a number.");
            return;
        }
        
        Console.Write("Enter calories burned: ");
        if (!int.TryParse(Console.ReadLine(), out int calories))
        {
            Console.WriteLine("Invalid calories. Please enter a number.");
            return;
        }
        
        _fitnessData.Workouts.Add(new Workout
        {
            Date = DateTime.Today,
            Type = type,
            DurationMinutes = duration,
            CaloriesBurned = calories
        });
        
        Console.WriteLine("Workout added successfully.");
    }
    
    private void AddSteps()
    {
        Console.Write("Enter number of steps: ");
        if (!int.TryParse(Console.ReadLine(), out int steps))
        {
            Console.WriteLine("Invalid step count. Please enter a number.");
            return;
        }
        
        DateTime today = DateTime.Today;
        if (_fitnessData.DailySteps.ContainsKey(today))
        {
            _fitnessData.DailySteps[today] += steps;
        }
        else
        {
            _fitnessData.DailySteps.Add(today, steps);
        }
        
        Console.WriteLine("Steps added successfully.");
    }
    
    private void AddCaloriesBurned()
    {
        Console.Write("Enter calories burned: ");
        if (!int.TryParse(Console.ReadLine(), out int calories))
        {
            Console.WriteLine("Invalid calories. Please enter a number.");
            return;
        }
        
        DateTime today = DateTime.Today;
        if (_fitnessData.DailyCalories.ContainsKey(today))
        {
            _fitnessData.DailyCalories[today] += calories;
        }
        else
        {
            _fitnessData.DailyCalories.Add(today, calories);
        }
        
        Console.WriteLine("Calories burned added successfully.");
    }
    
    private void ViewProgress()
    {
        Console.WriteLine("\nFitness Progress Summary:");
        
        Console.WriteLine("\nWorkouts:");
        foreach (var workout in _fitnessData.Workouts)
        {
            Console.WriteLine(workout.Date.ToShortDateString() + " - " + workout.Type + ": " + 
                             workout.DurationMinutes + " mins, " + workout.CaloriesBurned + " calories");
        }
        
        Console.WriteLine("\nDaily Steps:");
        foreach (var entry in _fitnessData.DailySteps)
        {
            Console.WriteLine(entry.Key.ToShortDateString() + ": " + entry.Value + " steps");
        }
        
        Console.WriteLine("\nDaily Calories Burned:");
        foreach (var entry in _fitnessData.DailyCalories)
        {
            Console.WriteLine(entry.Key.ToShortDateString() + ": " + entry.Value + " calories");
        }
    }
}

public class FitnessData
{
    public List<Workout> Workouts { get; set; }
    public Dictionary<DateTime, int> DailySteps { get; set; }
    public Dictionary<DateTime, int> DailyCalories { get; set; }
}

public class Workout
{
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public int DurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
}