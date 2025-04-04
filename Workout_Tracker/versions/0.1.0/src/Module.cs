using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Workout Tracker";
    
    private string _workoutsFilePath;
    
    public bool Main(string dataFolder)
    {
        _workoutsFilePath = Path.Combine(dataFolder, "workouts.json");
        
        Console.WriteLine("Workout Tracker Module is running.");
        Console.WriteLine("Data will be saved to: " + _workoutsFilePath);
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<Workout> workouts = LoadWorkouts();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddWorkout(workouts);
                    break;
                case "2":
                    ViewWorkouts(workouts);
                    break;
                case "3":
                    SaveWorkouts(workouts);
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private List<Workout> LoadWorkouts()
    {
        if (File.Exists(_workoutsFilePath))
        {
            string json = File.ReadAllText(_workoutsFilePath);
            return JsonSerializer.Deserialize<List<Workout>>(json);
        }
        
        return new List<Workout>();
    }
    
    private void SaveWorkouts(List<Workout> workouts)
    {
        string json = JsonSerializer.Serialize(workouts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_workoutsFilePath, json);
        Console.WriteLine("Workouts saved successfully.");
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nWorkout Tracker Menu:");
        Console.WriteLine("1. Add Workout");
        Console.WriteLine("2. View Workouts");
        Console.WriteLine("3. Exit and Save");
        Console.Write("Select an option: ");
    }
    
    private void AddWorkout(List<Workout> workouts)
    {
        Console.Write("Enter workout name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter duration in minutes: ");
        int duration = int.Parse(Console.ReadLine());
        
        Console.Write("Enter calories burned: ");
        int calories = int.Parse(Console.ReadLine());
        
        Console.Write("Enter date (yyyy-MM-dd): ");
        DateTime date = DateTime.Parse(Console.ReadLine());
        
        workouts.Add(new Workout
        {
            Name = name,
            Duration = duration,
            Calories = calories,
            Date = date
        });
        
        Console.WriteLine("Workout added successfully.");
    }
    
    private void ViewWorkouts(List<Workout> workouts)
    {
        if (workouts.Count == 0)
        {
            Console.WriteLine("No workouts recorded yet.");
            return;
        }
        
        Console.WriteLine("\nRecorded Workouts:");
        foreach (var workout in workouts)
        {
            Console.WriteLine($"{workout.Date:yyyy-MM-dd} - {workout.Name}");
            Console.WriteLine($"  Duration: {workout.Duration} mins");
            Console.WriteLine($"  Calories: {workout.Calories}");
        }
    }
}

public class Workout
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public int Calories { get; set; }
    public DateTime Date { get; set; }
}