using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HouseCleaningModule : IGeneratedModule
{
    public string Name { get; set; } = "House Cleaning Schedule Manager";
    
    private string scheduleFilePath;
    
    public HouseCleaningModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        scheduleFilePath = Path.Combine(dataFolder, "cleaning_schedule.json");
        
        Console.WriteLine("House Cleaning Schedule Manager is running");
        Console.WriteLine("Loading cleaning schedule...");
        
        var schedule = LoadSchedule();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. View cleaning schedule");
            Console.WriteLine("2. Add cleaning task");
            Console.WriteLine("3. Mark task as completed");
            Console.WriteLine("4. Remove task");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ViewSchedule(schedule);
                    break;
                case "2":
                    schedule = AddTask(schedule);
                    break;
                case "3":
                    schedule = MarkTaskCompleted(schedule);
                    break;
                case "4":
                    schedule = RemoveTask(schedule);
                    break;
                case "5":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveSchedule(schedule);
        Console.WriteLine("Cleaning schedule saved. Exiting...");
        
        return true;
    }
    
    private List<CleaningTask> LoadSchedule()
    {
        try
        {
            if (File.Exists(scheduleFilePath))
            {
                var json = File.ReadAllText(scheduleFilePath);
                return JsonSerializer.Deserialize<List<CleaningTask>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading schedule: " + ex.Message);
        }
        
        return new List<CleaningTask>();
    }
    
    private void SaveSchedule(List<CleaningTask> schedule)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(schedule, options);
            File.WriteAllText(scheduleFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving schedule: " + ex.Message);
        }
    }
    
    private void ViewSchedule(List<CleaningTask> schedule)
    {
        if (schedule.Count == 0)
        {
            Console.WriteLine("No cleaning tasks scheduled.");
            return;
        }
        
        Console.WriteLine("\nCleaning Schedule:");
        for (int i = 0; i < schedule.Count; i++)
        {
            var task = schedule[i];
            Console.WriteLine($"{i + 1}. {task.TaskName} - {task.Frequency} ({(task.IsCompleted ? "Completed" : "Pending")})");
        }
    }
    
    private List<CleaningTask> AddTask(List<CleaningTask> schedule)
    {
        Console.Write("Enter task name: ");
        var taskName = Console.ReadLine();
        
        Console.Write("Enter frequency (e.g., Daily, Weekly, Monthly): ");
        var frequency = Console.ReadLine();
        
        schedule.Add(new CleaningTask
        {
            TaskName = taskName,
            Frequency = frequency,
            IsCompleted = false
        });
        
        Console.WriteLine("Task added successfully.");
        return schedule;
    }
    
    private List<CleaningTask> MarkTaskCompleted(List<CleaningTask> schedule)
    {
        ViewSchedule(schedule);
        
        if (schedule.Count == 0) return schedule;
        
        Console.Write("Enter task number to mark as completed: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= schedule.Count)
        {
            schedule[taskNumber - 1].IsCompleted = true;
            Console.WriteLine("Task marked as completed.");
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
        
        return schedule;
    }
    
    private List<CleaningTask> RemoveTask(List<CleaningTask> schedule)
    {
        ViewSchedule(schedule);
        
        if (schedule.Count == 0) return schedule;
        
        Console.Write("Enter task number to remove: ");
        if (int.TryParse(Console.ReadLine(), out int taskNumber) && taskNumber > 0 && taskNumber <= schedule.Count)
        {
            schedule.RemoveAt(taskNumber - 1);
            Console.WriteLine("Task removed successfully.");
        }
        else
        {
            Console.WriteLine("Invalid task number.");
        }
        
        return schedule;
    }
}

public class CleaningTask
{
    public string TaskName { get; set; }
    public string Frequency { get; set; }
    public bool IsCompleted { get; set; }
}