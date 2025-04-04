using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutTracker : IGeneratedModule
{
    public string Name { get; set; } = "Workout Tracker";

    private string _workoutsFilePath;
    private List<Workout> _workouts;

    public WorkoutTracker()
    {
        _workouts = new List<Workout>();
    }

    public bool Main(string dataFolder)
    {
        _workoutsFilePath = Path.Combine(dataFolder, "workouts.json");
        
        Console.WriteLine("Workout Tracker Module is running.");
        
        LoadWorkouts();
        
        bool exit = false;
        while (!exit)
        {
            DisplayMenu();
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddWorkout();
                    break;
                case "2":
                    ViewWorkouts();
                    break;
                case "3":
                    ViewProgress();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveWorkouts();
        Console.WriteLine("Workout Tracker Module finished.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nWorkout Tracker Menu:");
        Console.WriteLine("1. Add Workout");
        Console.WriteLine("2. View Workouts");
        Console.WriteLine("3. View Progress");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddWorkout()
    {
        Console.Write("Enter workout name: ");
        string name = Console.ReadLine();
        
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
        
        _workouts.Add(new Workout
        {
            Name = name,
            Duration = duration,
            Calories = calories,
            Date = DateTime.Now
        });
        
        Console.WriteLine("Workout added successfully.");
    }

    private void ViewWorkouts()
    {
        if (_workouts.Count == 0)
        {
            Console.WriteLine("No workouts recorded yet.");
            return;
        }
        
        Console.WriteLine("\nRecorded Workouts:");
        foreach (var workout in _workouts)
        {
            Console.WriteLine($"{workout.Date}: {workout.Name} - {workout.Duration} min, {workout.Calories} cal");
        }
    }

    private void ViewProgress()
    {
        if (_workouts.Count == 0)
        {
            Console.WriteLine("No workouts recorded yet.");
            return;
        }
        
        int totalDuration = 0;
        int totalCalories = 0;
        
        foreach (var workout in _workouts)
        {
            totalDuration += workout.Duration;
            totalCalories += workout.Calories;
        }
        
        Console.WriteLine("\nWorkout Progress:");
        Console.WriteLine($"Total Workouts: {_workouts.Count}");
        Console.WriteLine($"Total Duration: {totalDuration} minutes");
        Console.WriteLine($"Total Calories Burned: {totalCalories} cal");
    }

    private void LoadWorkouts()
    {
        if (File.Exists(_workoutsFilePath))
        {
            string json = File.ReadAllText(_workoutsFilePath);
            _workouts = JsonSerializer.Deserialize<List<Workout>>(json);
        }
    }

    private void SaveWorkouts()
    {
        string json = JsonSerializer.Serialize(_workouts);
        File.WriteAllText(_workoutsFilePath, json);
    }
}

public class Workout
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public int Calories { get; set; }
    public DateTime Date { get; set; }
}