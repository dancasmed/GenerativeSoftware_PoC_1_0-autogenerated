using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Weekly Workout Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Weekly Workout Generator module is running...");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string filePath = Path.Combine(dataFolder, "workout_plan.json");
            
            var workoutPlan = GenerateWorkoutPlan();
            string json = JsonSerializer.Serialize(workoutPlan, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            
            Console.WriteLine("Workout plan generated successfully and saved to " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the workout plan: " + ex.Message);
            return false;
        }
    }

    private WorkoutPlan GenerateWorkoutPlan()
    {
        var workoutPlan = new WorkoutPlan
        {
            WeekStartDate = DateTime.Now.Date,
            WeekEndDate = DateTime.Now.Date.AddDays(6),
            Days = new List<WorkoutDay>()
        };

        string[] goals = { "Strength", "Endurance", "Flexibility", "Cardio" };
        Random random = new Random();
        
        for (int i = 0; i < 7; i++)
        {
            string goal = goals[random.Next(goals.Length)];
            
            var workoutDay = new WorkoutDay
            {
                Date = DateTime.Now.Date.AddDays(i),
                Goal = goal,
                Exercises = GenerateExercises(goal)
            };
            
            workoutPlan.Days.Add(workoutDay);
        }
        
        return workoutPlan;
    }

    private List<Exercise> GenerateExercises(string goal)
    {
        var exercises = new List<Exercise>();
        
        switch (goal)
        {
            case "Strength":
                exercises.Add(new Exercise { Name = "Squats", Sets = 4, Reps = 8 });
                exercises.Add(new Exercise { Name = "Deadlifts", Sets = 4, Reps = 6 });
                exercises.Add(new Exercise { Name = "Bench Press", Sets = 4, Reps = 8 });
                break;
            case "Endurance":
                exercises.Add(new Exercise { Name = "Running", Duration = "30 minutes" });
                exercises.Add(new Exercise { Name = "Cycling", Duration = "45 minutes" });
                break;
            case "Flexibility":
                exercises.Add(new Exercise { Name = "Yoga", Duration = "30 minutes" });
                exercises.Add(new Exercise { Name = "Dynamic Stretching", Duration = "20 minutes" });
                break;
            case "Cardio":
                exercises.Add(new Exercise { Name = "HIIT", Duration = "20 minutes" });
                exercises.Add(new Exercise { Name = "Jump Rope", Duration = "15 minutes" });
                break;
        }
        
        return exercises;
    }
}

public class WorkoutPlan
{
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public List<WorkoutDay> Days { get; set; }
}

public class WorkoutDay
{
    public DateTime Date { get; set; }
    public string Goal { get; set; }
    public List<Exercise> Exercises { get; set; }
}

public class Exercise
{
    public string Name { get; set; }
    public int? Sets { get; set; }
    public int? Reps { get; set; }
    public string Duration { get; set; }
}