using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Workout Tracker Module";
    
    private string _workoutsFilePath;
    
    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Workout Tracker Module is running.");
            
            _workoutsFilePath = Path.Combine(dataFolder, "workouts.json");
            
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            bool continueRunning = true;
            while (continueRunning)
            {
                Console.WriteLine("\nWorkout Tracker Menu:");
                Console.WriteLine("1. Add Workout");
                Console.WriteLine("2. View Workout History");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddWorkout();
                        break;
                    case "2":
                        ViewWorkoutHistory();
                        break;
                    case "3":
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void AddWorkout()
    {
        Console.Write("Enter workout name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter workout duration (minutes): ");
        int duration = int.Parse(Console.ReadLine());
        
        Console.Write("Enter calories burned: ");
        int calories = int.Parse(Console.ReadLine());
        
        Console.Write("Enter notes (optional): ");
        string notes = Console.ReadLine();
        
        var workout = new Workout
        {
            Name = name,
            Duration = duration,
            CaloriesBurned = calories,
            Notes = notes,
            Date = DateTime.Now
        };
        
        List<Workout> workouts = LoadWorkouts();
        workouts.Add(workout);
        SaveWorkouts(workouts);
        
        Console.WriteLine("Workout added successfully!");
    }
    
    private void ViewWorkoutHistory()
    {
        List<Workout> workouts = LoadWorkouts();
        
        if (workouts.Count == 0)
        {
            Console.WriteLine("No workouts recorded yet.");
            return;
        }
        
        Console.WriteLine("\nWorkout History:");
        foreach (var workout in workouts)
        {
            Console.WriteLine("\nName: " + workout.Name);
            Console.WriteLine("Date: " + workout.Date.ToString("yyyy-MM-dd HH:mm"));
            Console.WriteLine("Duration: " + workout.Duration + " minutes");
            Console.WriteLine("Calories Burned: " + workout.CaloriesBurned);
            if (!string.IsNullOrEmpty(workout.Notes))
            {
                Console.WriteLine("Notes: " + workout.Notes);
            }
        }
    }
    
    private List<Workout> LoadWorkouts()
    {
        if (!File.Exists(_workoutsFilePath))
        {
            return new List<Workout>();
        }
        
        string json = File.ReadAllText(_workoutsFilePath);
        return JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();
    }
    
    private void SaveWorkouts(List<Workout> workouts)
    {
        string json = JsonSerializer.Serialize(workouts);
        File.WriteAllText(_workoutsFilePath, json);
    }
}

public class Workout
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public int CaloriesBurned { get; set; }
    public string Notes { get; set; }
    public DateTime Date { get; set; }
}