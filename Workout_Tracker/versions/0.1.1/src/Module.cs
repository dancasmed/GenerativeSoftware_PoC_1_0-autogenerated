using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class WorkoutTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Workout Tracker";

    private string _workoutsFilePath;
    private List<Workout> _workouts;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Workout Tracker Module...");

        _workoutsFilePath = Path.Combine(dataFolder, "workouts.json");
        _workouts = new List<Workout>();

        LoadWorkouts();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nWorkout Tracker Menu:");
            Console.WriteLine("1. Add Workout");
            Console.WriteLine("2. View Workouts");
            Console.WriteLine("3. View Progress");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

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
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveWorkouts();
        Console.WriteLine("Workout Tracker Module completed.");
        return true;
    }

    private void LoadWorkouts()
    {
        try
        {
            if (File.Exists(_workoutsFilePath))
            {
                string json = File.ReadAllText(_workoutsFilePath);
                _workouts = JsonSerializer.Deserialize<List<Workout>>(json);
                Console.WriteLine("Workout data loaded successfully.");
            }
            else
            {
                Console.WriteLine("No existing workout data found. Starting fresh.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading workouts: " + ex.Message);
        }
    }

    private void SaveWorkouts()
    {
        try
        {
            string json = JsonSerializer.Serialize(_workouts);
            File.WriteAllText(_workoutsFilePath, json);
            Console.WriteLine("Workout data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving workouts: " + ex.Message);
        }
    }

    private void AddWorkout()
    {
        Console.Write("Enter workout name: ");
        string name = Console.ReadLine();

        Console.Write("Enter workout date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("Invalid date format. Using today's date.");
            date = DateTime.Today;
        }

        Console.Write("Enter duration in minutes: ");
        if (!int.TryParse(Console.ReadLine(), out int duration))
        {
            Console.WriteLine("Invalid duration. Defaulting to 30 minutes.");
            duration = 30;
        }

        Console.Write("Enter calories burned: ");
        if (!int.TryParse(Console.ReadLine(), out int calories))
        {
            Console.WriteLine("Invalid calories. Defaulting to 0.");
            calories = 0;
        }

        var workout = new Workout
        {
            Name = name,
            Date = date,
            DurationMinutes = duration,
            CaloriesBurned = calories
        };

        _workouts.Add(workout);
        Console.WriteLine("Workout added successfully!");
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
            Console.WriteLine($"{workout.Date:yyyy-MM-dd}: {workout.Name} ({workout.DurationMinutes} min, {workout.CaloriesBurned} cal)");
        }
    }

    private void ViewProgress()
    {
        if (_workouts.Count == 0)
        {
            Console.WriteLine("No workouts recorded yet.");
            return;
        }

        var earliestDate = _workouts.Min(w => w.Date);
        var latestDate = _workouts.Max(w => w.Date);
        var totalWorkouts = _workouts.Count;
        var totalDuration = _workouts.Sum(w => w.DurationMinutes);
        var totalCalories = _workouts.Sum(w => w.CaloriesBurned);

        Console.WriteLine("\nWorkout Progress Summary:");
        Console.WriteLine($"Tracking period: {earliestDate:yyyy-MM-dd} to {latestDate:yyyy-MM-dd}");
        Console.WriteLine($"Total workouts: {totalWorkouts}");
        Console.WriteLine($"Total workout time: {totalDuration} minutes");
        Console.WriteLine($"Total calories burned: {totalCalories}");

        var workoutsByType = _workouts.GroupBy(w => w.Name)
                                    .Select(g => new { Name = g.Key, Count = g.Count() })
                                    .OrderByDescending(x => x.Count);

        Console.WriteLine("\nMost Frequent Workouts:");
        foreach (var type in workoutsByType)
        {
            Console.WriteLine($"{type.Name}: {type.Count} times");
        }
    }
}

public class Workout
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
}