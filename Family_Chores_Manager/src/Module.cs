using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FamilyChoresModule : IGeneratedModule
{
    public string Name { get; set; } = "Family Chores Manager";
    
    private string _choresFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Family Chores Manager module...");
        
        _choresFilePath = Path.Combine(dataFolder, "chores.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<Chore> chores = LoadChores();
        
        bool exitRequested = false;
        while (!exitRequested)
        {
            Console.WriteLine("\nFamily Chores Manager");
            Console.WriteLine("1. Add Chore");
            Console.WriteLine("2. View Chores");
            Console.WriteLine("3. Mark Chore Complete");
            Console.WriteLine("4. Remove Chore");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddChore(chores);
                    break;
                case "2":
                    ViewChores(chores);
                    break;
                case "3":
                    MarkChoreComplete(chores);
                    break;
                case "4":
                    RemoveChore(chores);
                    break;
                case "5":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SaveChores(chores);
        }
        
        Console.WriteLine("Family Chores Manager module finished.");
        return true;
    }
    
    private List<Chore> LoadChores()
    {
        if (!File.Exists(_choresFilePath))
        {
            return new List<Chore>();
        }
        
        string json = File.ReadAllText(_choresFilePath);
        return JsonSerializer.Deserialize<List<Chore>>(json);
    }
    
    private void SaveChores(List<Chore> chores)
    {
        string json = JsonSerializer.Serialize(chores);
        File.WriteAllText(_choresFilePath, json);
    }
    
    private void AddChore(List<Chore> chores)
    {
        Console.Write("Enter chore name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter assigned person: ");
        string assignedTo = Console.ReadLine();
        
        Console.Write("Enter due date (yyyy-MM-dd): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
        {
            chores.Add(new Chore
            {
                Id = Guid.NewGuid(),
                Name = name,
                AssignedTo = assignedTo,
                DueDate = dueDate,
                IsCompleted = false
            });
            
            Console.WriteLine("Chore added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid date format. Chore not added.");
        }
    }
    
    private void ViewChores(List<Chore> chores)
    {
        if (chores.Count == 0)
        {
            Console.WriteLine("No chores found.");
            return;
        }
        
        Console.WriteLine("\nChores List:");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("ID\tName\t\tAssigned To\tDue Date\tStatus");
        
        foreach (var chore in chores)
        {
            Console.WriteLine($"{chore.Id}\t{chore.Name}\t{chore.AssignedTo}\t{chore.DueDate:yyyy-MM-dd}\t{(chore.IsCompleted ? "Completed" : "Pending")}");
        }
    }
    
    private void MarkChoreComplete(List<Chore> chores)
    {
        ViewChores(chores);
        
        if (chores.Count == 0) return;
        
        Console.Write("Enter the ID of the chore to mark complete: ");
        if (Guid.TryParse(Console.ReadLine(), out Guid choreId))
        {
            var chore = chores.Find(c => c.Id == choreId);
            if (chore != null)
            {
                chore.IsCompleted = true;
                Console.WriteLine("Chore marked as complete.");
            }
            else
            {
                Console.WriteLine("Chore not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
    
    private void RemoveChore(List<Chore> chores)
    {
        ViewChores(chores);
        
        if (chores.Count == 0) return;
        
        Console.Write("Enter the ID of the chore to remove: ");
        if (Guid.TryParse(Console.ReadLine(), out Guid choreId))
        {
            var chore = chores.Find(c => c.Id == choreId);
            if (chore != null)
            {
                chores.Remove(chore);
                Console.WriteLine("Chore removed successfully.");
            }
            else
            {
                Console.WriteLine("Chore not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }
}

public class Chore
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string AssignedTo { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
}