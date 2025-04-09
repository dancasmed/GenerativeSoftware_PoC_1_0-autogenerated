using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GardeningModule : IGeneratedModule
{
    public string Name { get; set; } = "Gardening Schedule Manager";

    private string GetScheduleFilePath(string dataFolder)
    {
        return Path.Combine(dataFolder, "gardening_schedule.json");
    }

    private Dictionary<string, List<string>> LoadSchedule(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new Dictionary<string, List<string>>
            {
                { "Spring", new List<string>() },
                { "Summer", new List<string>() },
                { "Autumn", new List<string>() },
                { "Winter", new List<string>() }
            };
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
    }

    private void SaveSchedule(string filePath, Dictionary<string, List<string>> schedule)
    {
        string json = JsonSerializer.Serialize(schedule);
        File.WriteAllText(filePath, json);
    }

    private string GetCurrentSeason()
    {
        int month = DateTime.Now.Month;
        if (month >= 3 && month <= 5) return "Spring";
        if (month >= 6 && month <= 8) return "Summer";
        if (month >= 9 && month <= 11) return "Autumn";
        return "Winter";
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Gardening Schedule Manager is running");
        Console.WriteLine("Current season: " + GetCurrentSeason());

        string filePath = GetScheduleFilePath(dataFolder);
        var schedule = LoadSchedule(filePath);

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. View seasonal tasks");
            Console.WriteLine("2. Add task to season");
            Console.WriteLine("3. Remove task from season");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine("\nSeasonal Tasks:");
                    foreach (var season in schedule)
                    {
                        Console.WriteLine(season.Key + ":");
                        foreach (var task in season.Value)
                        {
                            Console.WriteLine(" - " + task);
                        }
                    }
                    break;

                case "2":
                    Console.Write("Enter season (Spring/Summer/Autumn/Winter): ");
                    string seasonToAdd = Console.ReadLine();
                    if (schedule.ContainsKey(seasonToAdd))
                    {
                        Console.Write("Enter task to add: ");
                        string taskToAdd = Console.ReadLine();
                        schedule[seasonToAdd].Add(taskToAdd);
                        SaveSchedule(filePath, schedule);
                        Console.WriteLine("Task added successfully");
                    }
                    else
                    {
                        Console.WriteLine("Invalid season");
                    }
                    break;

                case "3":
                    Console.Write("Enter season (Spring/Summer/Autumn/Winter): ");
                    string seasonToRemove = Console.ReadLine();
                    if (schedule.ContainsKey(seasonToRemove))
                    {
                        Console.WriteLine("Tasks for " + seasonToRemove + ":");
                        for (int i = 0; i < schedule[seasonToRemove].Count; i++)
                        {
                            Console.WriteLine(i + ". " + schedule[seasonToRemove][i]);
                        }
                        Console.Write("Enter task number to remove: ");
                        if (int.TryParse(Console.ReadLine(), out int taskIndex) && taskIndex >= 0 && taskIndex < schedule[seasonToRemove].Count)
                        {
                            schedule[seasonToRemove].RemoveAt(taskIndex);
                            SaveSchedule(filePath, schedule);
                            Console.WriteLine("Task removed successfully");
                        }
                        else
                        {
                            Console.WriteLine("Invalid task number");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid season");
                    }
                    break;

                case "4":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }

        return true;
    }
}