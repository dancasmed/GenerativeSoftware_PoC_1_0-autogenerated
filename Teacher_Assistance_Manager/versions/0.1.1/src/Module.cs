using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class AttendanceModule : IGeneratedModule
{
    public string Name { get; set; } = "Attendance Management System";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Attendance Management Module...");
        Console.WriteLine("Data storage location: " + dataFolder);
        
        Directory.CreateDirectory(dataFolder);
        
        var exitRequested = false;
        while (!exitRequested)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Manage Groups");
            Console.WriteLine("2. Record Daily Attendance");
            Console.WriteLine("3. View/Edit Attendance Records");
            Console.WriteLine("4. Generate Weekly Summary");
            Console.WriteLine("5. Exit");
            
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ManageGroups(dataFolder);
                    break;
                case "2":
                    RecordAttendance(dataFolder);
                    break;
                case "3":
                    ViewEditAttendance(dataFolder);
                    break;
                case "4":
                    GenerateWeeklySummary(dataFolder);
                    break;
                case "5":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
        return true;
    }

    private void ManageGroups(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        
        Console.WriteLine("\nGroup Management:");
        Console.WriteLine("1. Create New Group");
        Console.WriteLine("2. List Groups");
        Console.WriteLine("3. Add Student to Group");
        Console.WriteLine("4. Edit Group Name");
        Console.WriteLine("5. Delete Group");
        
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                CreateNewGroup(dataFolder, groups);
                break;
            case "2":
                ListGroups(groups);
                break;
            case "3":
                AddStudentToGroup(dataFolder);
                break;
            case "4":
                EditGroup(dataFolder, groups);
                break;
            case "5":
                DeleteGroup(dataFolder, groups);
                break;
            default:
                Console.WriteLine("Invalid option");
                break;
        }
    }

    private void CreateNewGroup(string dataFolder, List<Group> groups)
    {
        Console.WriteLine("Enter group name:");
        var groupName = Console.ReadLine();
        
        var newGroup = new Group
        {
            GroupId = Guid.NewGuid().ToString(),
            GroupName = groupName,
            CreationDate = DateTime.Now,
            Students = new List<Student>()
        };
        
        groups.Add(newGroup);
        DataStorageHelper.SaveData(dataFolder, "groups.json", groups);
        Console.WriteLine("Group created successfully");
    }

    private void EditGroup(string dataFolder, List<Group> groups)
    {
        Console.WriteLine("Enter group ID to edit:");
        ListGroups(groups);
        var groupId = Console.ReadLine();
        
        var group = groups.FirstOrDefault(g => g.GroupId == groupId);
        if (group == null)
        {
            Console.WriteLine("Group not found!");
            return;
        }
        
        Console.WriteLine("Enter new group name:");
        group.GroupName = Console.ReadLine();
        DataStorageHelper.SaveData(dataFolder, "groups.json", groups);
        Console.WriteLine("Group updated successfully");
    }

    private void DeleteGroup(string dataFolder, List<Group> groups)
    {
        Console.WriteLine("Enter group ID to delete:");
        ListGroups(groups);
        var groupId = Console.ReadLine();
        
        var group = groups.FirstOrDefault(g => g.GroupId == groupId);
        if (group == null)
        {
            Console.WriteLine("Group not found!");
            return;
        }
        
        groups.Remove(group);
        DataStorageHelper.SaveData(dataFolder, "groups.json", groups);
        Console.WriteLine("Group deleted successfully");
    }

    private void ListGroups(List<Group> groups)
    {
        Console.WriteLine("\nGroups:");
        foreach (var group in groups)
        {
            Console.WriteLine($"{group.GroupName} ({group.GroupId})");
        }
    }

    private void AddStudentToGroup(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        ListGroups(groups);
        
        Console.WriteLine("Enter group ID to add student to:");
        var groupId = Console.ReadLine();
        
        var group = groups.FirstOrDefault(g => g.GroupId == groupId);
        if (group == null)
        {
            Console.WriteLine("Group not found!");
            return;
        }
        
        Console.WriteLine("Enter student name:");
        var studentName = Console.ReadLine();
        
        group.Students.Add(new Student
        {
            StudentId = Guid.NewGuid().ToString(),
            Name = studentName
        });
        
        DataStorageHelper.SaveData(dataFolder, "groups.json", groups);
        Console.WriteLine("Student added successfully");
    }

    private void RecordAttendance(string dataFolder)
    {
        // Implementation placeholder
        Console.WriteLine("Recording attendance...");
    }

    private void ViewEditAttendance(string dataFolder)
    {
        // Implementation placeholder
        Console.WriteLine("Viewing/editing attendance...");
    }

    private void GenerateWeeklySummary(string dataFolder)
    {
        // Implementation placeholder
        Console.WriteLine("Generating weekly summary...");
    }
}

public static class DataStorageHelper
{
    public static List<T> LoadData<T>(string dataFolder, string fileName)
    {
        var path = Path.Combine(dataFolder, fileName);
        if (!File.Exists(path)) return new List<T>();
        
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public static void SaveData<T>(string dataFolder, string fileName, List<T> data)
    {
        var path = Path.Combine(dataFolder, fileName);
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
    }
}

public class Group
{
    public string GroupId { get; set; }
    public string GroupName { get; set; }
    public DateTime CreationDate { get; set; }
    public List<Student> Students { get; set; } = new List<Student>();
}

public class Student
{
    public string StudentId { get; set; }
    public string Name { get; set; }
}
