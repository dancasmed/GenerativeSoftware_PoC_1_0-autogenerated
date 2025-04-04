using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutPlanGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Workout Plan Generator";

    public WorkoutPlanGenerator() { }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Weekly Workout Plan Generator is running...");

        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            Console.WriteLine("Please enter your fitness goal (e.g., Strength, Endurance, Weight Loss, General Fitness):");
            string goal = Console.ReadLine();

            Console.WriteLine("Enter your workout days per week (1-7):");
            int daysPerWeek;
            while (!int.TryParse(Console.ReadLine(), out daysPerWeek) || daysPerWeek < 1 || daysPerWeek > 7)
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 7:");
            }

            var workoutPlan = GenerateWorkoutPlan(goal, daysPerWeek);
            string json = JsonSerializer.Serialize(workoutPlan, new JsonSerializerOptions { WriteIndented = true });

            string filePath = Path.Combine(dataFolder, "workout_plan.json");
            File.WriteAllText(filePath, json);

            Console.WriteLine("Workout plan generated and saved to " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private Dictionary<string, List<string>> GenerateWorkoutPlan(string goal, int daysPerWeek)
    {
        var workoutPlan = new Dictionary<string, List<string>>();
        var exercises = GetExercisesByGoal(goal);
        var random = new Random();

        for (int i = 1; i <= daysPerWeek; i++)
        {
            var dayExercises = new List<string>();
            int exerciseCount = Math.Min(5, exercises.Count);

            for (int j = 0; j < exerciseCount; j++)
            {
                int index = random.Next(exercises.Count);
                dayExercises.Add(exercises[index]);
            }

            workoutPlan.Add("Day " + i, dayExercises);
        }

        return workoutPlan;
    }

    private List<string> GetExercisesByGoal(string goal)
    {
        switch (goal.ToLower())
        {
            case "strength":
                return new List<string>
                {
                    "Deadlifts", "Squats", "Bench Press", "Overhead Press", "Barbell Rows",
                    "Pull-Ups", "Dips", "Lunges", "Romanian Deadlifts"
                };
            case "endurance":
                return new List<string>
                {
                    "Running", "Cycling", "Swimming", "Rowing", "Jump Rope",
                    "Burpees", "Mountain Climbers", "Box Jumps", "High Knees"
                };
            case "weight loss":
                return new List<string>
                {
                    "HIIT Circuit", "Kettlebell Swings", "Battle Ropes", "Sprints",
                    "Jump Squats", "Plank", "Russian Twists", "Bicycle Crunches"
                };
            default:
                return new List<string>
                {
                    "Push-Ups", "Squats", "Lunges", "Plank", "Crunches",
                    "Superman", "Glute Bridges", "Arm Circles", "Leg Raises"
                };
        }
    }
}