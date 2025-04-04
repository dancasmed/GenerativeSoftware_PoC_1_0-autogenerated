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

        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddGoal();
                    break;
                case "2":
                    UpdateGoalProgress();
                    break;
                case "3":
                    ViewGoals();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveGoals();
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nGoal Tracker Menu:");
        Console.WriteLine("1. Add a new goal");
        Console.WriteLine("2. Update goal progress");
        Console.WriteLine("3. View all goals");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
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

    private void AddGoal()
    {
        Console.Write("Enter goal name: ");
        string name = Console.ReadLine();

        Console.Write("Enter goal description: ");
        string description = Console.ReadLine();

        Console.Write("Enter total milestones: ");
        if (!int.TryParse(Console.ReadLine(), out int totalMilestones))
        {
            Console.WriteLine("Invalid number of milestones.");
            return;
        }

        var goal = new Goal
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            TotalMilestones = totalMilestones,
            CompletedMilestones = 0,
            CreatedDate = DateTime.Now
        };

        _goals.Add(goal);
        Console.WriteLine("Goal added successfully!");
    }

    private void UpdateGoalProgress()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("No goals available to update.");
            return;
        }

        ViewGoals();
        Console.Write("Enter the number of the goal to update: ");
        if (!int.TryParse(Console.ReadLine(), out int goalIndex) || goalIndex < 1 || goalIndex > _goals.Count)
        {
            Console.WriteLine("Invalid goal selection.");
            return;
        }

        var goal = _goals[goalIndex - 1];
        Console.Write("Enter number of completed milestones to add: ");
        if (!int.TryParse(Console.ReadLine(), out int completedMilestones))
        {
            Console.WriteLine("Invalid number of milestones.");
            return;
        }

        goal.CompletedMilestones = Math.Min(goal.CompletedMilestones + completedMilestones, goal.TotalMilestones);
        Console.WriteLine("Goal progress updated successfully!");
    }

    private void ViewGoals()
    {
        if (_goals.Count == 0)
        {
            Console.WriteLine("No goals available.");
            return;
        }

        Console.WriteLine("\nCurrent Goals:");
        for (int i = 0; i < _goals.Count; i++)
        {
            var goal = _goals[i];
            double percentage = (double)goal.CompletedMilestones / goal.TotalMilestones * 100;
            Console.WriteLine($"{i + 1}. {goal.Name} - {percentage:F1}% complete ({goal.CompletedMilestones}/{goal.TotalMilestones} milestones)");
            Console.WriteLine($"   Description: {goal.Description}");
            Console.WriteLine($"   Created: {goal.CreatedDate:yyyy-MM-dd}");
        }
    }
}

public class Goal
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int TotalMilestones { get; set; }
    public int CompletedMilestones { get; set; }
    public DateTime CreatedDate { get; set; }
}