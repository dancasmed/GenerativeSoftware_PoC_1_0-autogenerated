using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutPlannerModule : IGeneratedModule
{
    public string Name { get; set; } = "Workout Planner";
    
    private string _workoutDataPath;
    
    public bool Main(string dataFolder)
    {
        _workoutDataPath = Path.Combine(dataFolder, "workouts.json");
        
        Console.WriteLine("Workout Planner Module Started");
        Console.WriteLine("Data will be saved in: " + _workoutDataPath);
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    AddWorkout();
                    break;
                case "2":
                    ViewWorkouts();
                    break;
                case "3":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Workout Planner Module Finished");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nWorkout Planner Menu:");
        Console.WriteLine("1. Add Workout");
        Console.WriteLine("2. View Workouts");
        Console.WriteLine("3. Exit");
        Console.Write("Enter your choice: ");
    }
    
    private void AddWorkout()
    {
        Console.WriteLine("\nAdd New Workout");
        
        Console.Write("Exercise Name: ");
        string exerciseName = Console.ReadLine();
        
        Console.Write("Number of Sets: ");
        int sets = int.Parse(Console.ReadLine());
        
        Console.Write("Number of Reps: ");
        int reps = int.Parse(Console.ReadLine());
        
        Console.Write("Date (yyyy-MM-dd): ");
        string date = Console.ReadLine();
        
        var workout = new Workout
        {
            ExerciseName = exerciseName,
            Sets = sets,
            Reps = reps,
            Date = date
        };
        
        List<Workout> workouts = LoadWorkouts();
        workouts.Add(workout);
        SaveWorkouts(workouts);
        
        Console.WriteLine("Workout added successfully!");
    }
    
    private void ViewWorkouts()
    {
        var workouts = LoadWorkouts();
        
        if (workouts.Count == 0)
        {
            Console.WriteLine("No workouts found.");
            return;
        }
        
        Console.WriteLine("\nYour Workouts:");
        foreach (var workout in workouts)
        {
            Console.WriteLine($"{workout.Date} - {workout.ExerciseName}: {workout.Sets} sets x {workout.Reps} reps");
        }
    }
    
    private List<Workout> LoadWorkouts()
    {
        if (!File.Exists(_workoutDataPath))
        {
            return new List<Workout>();
        }
        
        string json = File.ReadAllText(_workoutDataPath);
        return JsonSerializer.Deserialize<List<Workout>>(json);
    }
    
    private void SaveWorkouts(List<Workout> workouts)
    {
        string json = JsonSerializer.Serialize(workouts);
        File.WriteAllText(_workoutDataPath, json);
    }
}

public class Workout
{
    public string ExerciseName { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public string Date { get; set; }
}