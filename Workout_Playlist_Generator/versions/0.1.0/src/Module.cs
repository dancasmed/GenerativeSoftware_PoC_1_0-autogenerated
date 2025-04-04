using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WorkoutPlaylistGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Workout Playlist Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Workout Playlist Generator module is running.");

        try
        {
            string exercisesFile = Path.Combine(dataFolder, "exercises.json");
            string playlistFile = Path.Combine(dataFolder, "playlist.json");

            if (!File.Exists(exercisesFile))
            {
                Console.WriteLine("No exercises file found. Creating a sample exercises file.");
                CreateSampleExercisesFile(exercisesFile);
            }

            List<Exercise> exercises = LoadExercises(exercisesFile);
            List<PlaylistItem> playlist = GeneratePlaylist(exercises);

            SavePlaylist(playlistFile, playlist);
            Console.WriteLine("Workout playlist generated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating workout playlist: " + ex.Message);
            return false;
        }
    }

    private List<Exercise> LoadExercises(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Exercise>>(json);
    }

    private List<PlaylistItem> GeneratePlaylist(List<Exercise> exercises)
    {
        List<PlaylistItem> playlist = new List<PlaylistItem>();
        TimeSpan totalDuration = TimeSpan.Zero;

        foreach (var exercise in exercises)
        {
            playlist.Add(new PlaylistItem
            {
                ExerciseName = exercise.Name,
                Duration = exercise.Duration,
                StartTime = totalDuration
            });

            totalDuration = totalDuration.Add(exercise.Duration);
        }

        return playlist;
    }

    private void SavePlaylist(string filePath, List<PlaylistItem> playlist)
    {
        string json = JsonSerializer.Serialize(playlist, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private void CreateSampleExercisesFile(string filePath)
    {
        var sampleExercises = new List<Exercise>
        {
            new Exercise { Name = "Warm-up", Duration = TimeSpan.FromMinutes(5) },
            new Exercise { Name = "Push-ups", Duration = TimeSpan.FromMinutes(3) },
            new Exercise { Name = "Squats", Duration = TimeSpan.FromMinutes(4) },
            new Exercise { Name = "Plank", Duration = TimeSpan.FromMinutes(2) },
            new Exercise { Name = "Cool-down", Duration = TimeSpan.FromMinutes(5) }
        };

        string json = JsonSerializer.Serialize(sampleExercises, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

public class Exercise
{
    public string Name { get; set; }
    public TimeSpan Duration { get; set; }
}

public class PlaylistItem
{
    public string ExerciseName { get; set; }
    public TimeSpan Duration { get; set; }
    public TimeSpan StartTime { get; set; }
}