using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Workout Tracker Module";

    private string _workoutsFilePath;
    private List<Workout> _workouts;

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Initializing Workout Tracker Module...");
            _workoutsFilePath = Path.Combine(dataFolder, "workouts.json");
            LoadWorkouts();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nWorkout Tracker Menu:");
                Console.WriteLine("1. Add Workout");
                Console.WriteLine("2. View Workouts");
                Console.WriteLine("3. Exit");
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
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            SaveWorkouts();
            Console.WriteLine("Workout data saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadWorkouts()
    {
        if (File.Exists(_workoutsFilePath))
        {
            string json = File.ReadAllText(_workoutsFilePath);
            _workouts = JsonSerializer.Deserialize<List<Workout>>(json);
        }
        else
        {
            _workouts = new List<Workout>();
        }
    }

    private void SaveWorkouts()
    {
        string json = JsonSerializer.Serialize(_workouts);
        File.WriteAllText(_workoutsFilePath, json);
    }

    private void AddWorkout()
    {
        Console.Write("Enter workout name: ");
        string name = Console.ReadLine();

        Console.Write("Enter duration in minutes: ");
        int duration = int.Parse(Console.ReadLine());

        Console.Write("Enter calories burned: ");
        int calories = int.Parse(Console.ReadLine());

        Console.Write("Enter date (yyyy-MM-dd): ");
        DateTime date = DateTime.Parse(Console.ReadLine());

        _workouts.Add(new Workout
        {
            Name = name,
            Duration = duration,
            Calories = calories,
            Date = date
        });

        Console.WriteLine("Workout added successfully!");
    }

    private void ViewWorkouts()
    {
        if (_workouts.Count == 0)
        {
            Console.WriteLine("No workouts recorded yet.");
            return;
        }

        Console.WriteLine("\nWorkout History:");
        foreach (var workout in _workouts)
        {
            Console.WriteLine("Name: " + workout.Name);
            Console.WriteLine("Duration: " + workout.Duration + " minutes");
            Console.WriteLine("Calories: " + workout.Calories + " kcal");
            Console.WriteLine("Date: " + workout.Date.ToString("yyyy-MM-dd"));
            Console.WriteLine();
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