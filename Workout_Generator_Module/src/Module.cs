using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Workout Generator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Workout Generator Module is running...");
        Console.WriteLine("Generating personalized workout routines based on fitness goals.");

        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string workoutFilePath = Path.Combine(dataFolder, "workout_routines.json");

            var fitnessGoals = new List<string> { "Weight Loss", "Muscle Gain", "Endurance", "General Fitness" };
            var workoutRoutines = new Dictionary<string, List<string>>();

            foreach (var goal in fitnessGoals)
            {
                workoutRoutines[goal] = GenerateWorkoutRoutine(goal);
            }

            string json = JsonSerializer.Serialize(workoutRoutines, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(workoutFilePath, json);

            Console.WriteLine("Workout routines have been generated and saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating workout routines: " + ex.Message);
            return false;
        }
    }

    private List<string> GenerateWorkoutRoutine(string fitnessGoal)
    {
        var routine = new List<string>();

        switch (fitnessGoal)
        {
            case "Weight Loss":
                routine.Add("Cardio: 30 minutes running");
                routine.Add("HIIT: 20 minutes");
                routine.Add("Strength: Bodyweight exercises (push-ups, squats)");
                break;
            case "Muscle Gain":
                routine.Add("Strength: Bench Press - 4 sets of 8 reps");
                routine.Add("Strength: Deadlifts - 4 sets of 6 reps");
                routine.Add("Strength: Pull-ups - 3 sets of 10 reps");
                break;
            case "Endurance":
                routine.Add("Cardio: 45 minutes cycling");
                routine.Add("Cardio: 30 minutes swimming");
                routine.Add("Strength: Light weights with high reps");
                break;
            case "General Fitness":
                routine.Add("Cardio: 20 minutes jogging");
                routine.Add("Strength: Full-body workout");
                routine.Add("Flexibility: 15 minutes stretching");
                break;
            default:
                routine.Add("No specific routine for the given goal.");
                break;
        }

        return routine;
    }
}