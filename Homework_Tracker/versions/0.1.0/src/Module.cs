using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HomeworkTracker : IGeneratedModule
{
    public string Name { get; set; } = "Homework Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Homework Tracker Module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "homework_assignments.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<HomeworkAssignment> assignments = LoadAssignments();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nHomework Tracker Menu:");
            Console.WriteLine("1. Add new assignment");
            Console.WriteLine("2. View all assignments");
            Console.WriteLine("3. Mark assignment as completed");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddAssignment(assignments);
                    break;
                case "2":
                    ViewAssignments(assignments);
                    break;
                case "3":
                    MarkAssignmentCompleted(assignments);
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveAssignments(assignments);
        Console.WriteLine("Homework assignments saved. Exiting module...");
        return true;
    }
    
    private List<HomeworkAssignment> LoadAssignments()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<HomeworkAssignment>();
        }
        
        string json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<HomeworkAssignment>>(json);
    }
    
    private void SaveAssignments(List<HomeworkAssignment> assignments)
    {
        string json = JsonSerializer.Serialize(assignments, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddAssignment(List<HomeworkAssignment> assignments)
    {
        Console.Write("Enter assignment name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter course name: ");
        string course = Console.ReadLine();
        
        Console.Write("Enter due date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
        {
            Console.WriteLine("Invalid date format. Assignment not added.");
            return;
        }
        
        assignments.Add(new HomeworkAssignment
        {
            Name = name,
            Course = course,
            DueDate = dueDate,
            IsCompleted = false
        });
        
        Console.WriteLine("Assignment added successfully.");
    }
    
    private void ViewAssignments(List<HomeworkAssignment> assignments)
    {
        if (assignments.Count == 0)
        {
            Console.WriteLine("No assignments found.");
            return;
        }
        
        Console.WriteLine("\nCurrent Assignments:");
        foreach (var assignment in assignments)
        {
            string status = assignment.IsCompleted ? "[Completed]" : "[Pending]";
            Console.WriteLine($"{assignment.Course}: {assignment.Name} - Due: {assignment.DueDate:yyyy-MM-dd} {status}");
        }
    }
    
    private void MarkAssignmentCompleted(List<HomeworkAssignment> assignments)
    {
        if (assignments.Count == 0)
        {
            Console.WriteLine("No assignments available to mark as completed.");
            return;
        }
        
        ViewAssignments(assignments);
        Console.Write("Enter the number of the assignment to mark as completed: ");
        
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= assignments.Count)
        {
            assignments[index - 1].IsCompleted = true;
            Console.WriteLine("Assignment marked as completed.");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }
}

public class HomeworkAssignment
{
    public string Name { get; set; }
    public string Course { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
}