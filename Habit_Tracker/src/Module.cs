using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HabitTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Habit Tracker";

    private string habitsFilePath;
    private Dictionary<string, List<DateTime>> habits;

    public HabitTrackerModule()
    {
        habits = new Dictionary<string, List<DateTime>>();
    }

    public bool Main(string dataFolder)
    {
        habitsFilePath = Path.Combine(dataFolder, "habits.json");
        
        try
        {
            LoadHabits();
            Console.WriteLine("Habit Tracker module started successfully.");
            
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nHabit Tracker Menu:");
                Console.WriteLine("1. Add Habit");
                Console.WriteLine("2. Log Habit Completion");
                Console.WriteLine("3. View Progress");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");
                
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }
                
                switch (choice)
                {
                    case 1:
                        AddHabit();
                        break;
                    case 2:
                        LogHabitCompletion();
                        break;
                    case 3:
                        ViewProgress();
                        break;
                    case 4:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            
            SaveHabits();
            Console.WriteLine("Habit Tracker module finished.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in Habit Tracker module: " + ex.Message);
            return false;
        }
    }

    private void LoadHabits()
    {
        if (File.Exists(habitsFilePath))
        {
            string json = File.ReadAllText(habitsFilePath);
            habits = JsonSerializer.Deserialize<Dictionary<string, List<DateTime>>>(json);
        }
    }

    private void SaveHabits()
    {
        string json = JsonSerializer.Serialize(habits);
        File.WriteAllText(habitsFilePath, json);
    }

    private void AddHabit()
    {
        Console.Write("Enter habit name: ");
        string habitName = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(habitName))
        {
            Console.WriteLine("Habit name cannot be empty.");
            return;
        }
        
        if (!habits.ContainsKey(habitName))
        {
            habits.Add(habitName, new List<DateTime>());
            Console.WriteLine("Habit added successfully.");
        }
        else
        {
            Console.WriteLine("Habit already exists.");
        }
    }

    private void LogHabitCompletion()
    {
        if (habits.Count == 0)
        {
            Console.WriteLine("No habits available. Please add a habit first.");
            return;
        }
        
        Console.WriteLine("Available habits:");
        int index = 1;
        foreach (var habit in habits.Keys)
        {
            Console.WriteLine(index + ". " + habit);
            index++;
        }
        
        Console.Write("Select habit to log (number): ");
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > habits.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }
        
        string selectedHabit = new List<string>(habits.Keys)[choice - 1];
        habits[selectedHabit].Add(DateTime.Today);
        Console.WriteLine("Habit logged successfully for today.");
    }

    private void ViewProgress()
    {
        if (habits.Count == 0)
        {
            Console.WriteLine("No habits available. Please add a habit first.");
            return;
        }
        
        Console.WriteLine("\nHabit Progress Charts:");
        
        foreach (var habit in habits)
        {
            Console.WriteLine("\nHabit: " + habit.Key);
            
            // Calculate streak
            int streak = CalculateStreak(habit.Value);
            Console.WriteLine("Current Streak: " + streak + " days");
            
            // Simple bar chart for last 7 days
            Console.WriteLine("Last 7 Days:");
            for (int i = 6; i >= 0; i--)
            {
                DateTime date = DateTime.Today.AddDays(-i);
                bool completed = habit.Value.Contains(date);
                
                Console.Write(date.ToString("ddd") + ": ");
                Console.Write(completed ? "[X]" : "[ ]");
                Console.WriteLine();
            }
        }
    }

    private int CalculateStreak(List<DateTime> dates)
    {
        if (dates.Count == 0) return 0;
        
        dates.Sort();
        dates.Reverse();
        
        int streak = 0;
        DateTime currentDate = DateTime.Today;
        
        foreach (var date in dates)
        {
            if (date.Date == currentDate.Date)
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }
            else if (date.Date < currentDate.Date)
            {
                break;
            }
        }
        
        return streak;
    }
}