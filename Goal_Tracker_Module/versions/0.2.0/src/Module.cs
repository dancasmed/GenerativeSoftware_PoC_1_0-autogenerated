using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GoalTrackerModule : IGeneratedModule
{
    public string Name { get; set; } = "Goal Tracker Module";

    private string _goalsFilePath;
    private List<Goal> _goals;

    public GoalTrackerModule()
    {
        _goals = new List<Goal>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Goal Tracker Module is running...");
        _goalsFilePath = Path.Combine(dataFolder, "goals.json");

        LoadGoals();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddGoal();
                    break;
                case "2":
                    ViewGoals();
                    break;
                case "3":
                    UpdateGoalProgress();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveGoals();
        Console.WriteLine("Goal Tracker Module finished.");
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nGoal Tracker Menu:");
        Console.WriteLine("1. Add a new goal");
        Console.WriteLine("2. View all goals");
        Console.WriteLine("3. Update goal progress");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddGoal()
    {
        Console.Write("Enter goal name: ");
        string name = Console.ReadLine();

        Console.Write("Enter target value: ");
        if (!int.TryParse(Console.ReadLine(), out int target))
        {
            Console.WriteLine("Invalid target value. Goal not added.");
            return;
        }

        _goals.Add(new Goal { Name = name, Target = target, CurrentProgress = 0 });
        Console.WriteLine("Goal added successfully.");
    }

    private void ViewGoals()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("No goals found.");
            return;
        }

        Console.WriteLine("\nCurrent Goals:");
        for (int i = 0; i < _goals.Count; i++)
        {
            Goal goal = _goals[i];
            Console.WriteLine($"{i + 1}. {goal.Name} - Progress: {goal.CurrentProgress}/{goal.Target} ({(goal.CurrentProgress * 100 / goal.Target)}%)");
        }
    }

    private void UpdateGoalProgress()
    {
        ViewGoals();
        if (_goals.Count == 0) return;

        Console.Write("Select goal to update: ");
        if (!int.TryParse(Console.ReadLine(), out int goalIndex) || goalIndex < 1 || goalIndex > _goals.Count)
        {
            Console.WriteLine("Invalid goal selection.");
            return;
        }

        Console.Write("Enter progress to add: ");
        if (!int.TryParse(Console.ReadLine(), out int progress))
        {
            Console.WriteLine("Invalid progress value.");
            return;
        }

        Goal selectedGoal = _goals[goalIndex - 1];
        selectedGoal.CurrentProgress = Math.Min(selectedGoal.CurrentProgress + progress, selectedGoal.Target);
        Console.WriteLine("Progress updated successfully.");
    }

    private void LoadGoals()
    {
        if (File.Exists(_goalsFilePath))
        {
            try
            {
                string json = File.ReadAllText(_goalsFilePath);
                _goals = JsonSerializer.Deserialize<List<Goal>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading goals: " + ex.Message);
            }
        }
    }

    private void SaveGoals()
    {
        try
        {
            string json = JsonSerializer.Serialize(_goals);
            File.WriteAllText(_goalsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving goals: " + ex.Message);
        }
    }

    private class Goal
    {
        public string Name { get; set; }
        public int Target { get; set; }
        public int CurrentProgress { get; set; }
    }
}