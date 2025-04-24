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

    private void GenerateWeeklySummary(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        if (groups.Count == 0)
        {
            Console.WriteLine("No groups available. Please create a group first.");
            return;
        }

        Console.WriteLine("Select a group to generate summary:");
        ListGroups(groups);
        Console.WriteLine("Enter group ID:");
        var groupId = Console.ReadLine();
        var selectedGroup = groups.FirstOrDefault(g => g.GroupId == groupId);

        if (selectedGroup == null)
        {
            Console.WriteLine("Invalid group selection");
            return;
        }

        Console.WriteLine("Enter reference date (yyyy-MM-dd) or press Enter for current week:");
        var dateInput = Console.ReadLine();
        DateTime targetDate;
        if (!DateTime.TryParse(dateInput, out targetDate))
        {
            targetDate = DateTime.Today;
        }

        var startOfWeek = targetDate.AddDays(-(int)targetDate.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(6);
        
        var allRecords = DataStorageHelper.LoadData<AttendanceRecord>(dataFolder, "attendance.json")
            .Where(r => r.GroupId == groupId && r.Date >= startOfWeek && r.Date <= endOfWeek)
            .ToList();

        Console.WriteLine("\nWeekly Attendance Summary (" + startOfWeek.ToString("yyyy-MM-dd") + " to " + endOfWeek.ToString("yyyy-MM-dd") + ");");

        foreach (var student in selectedGroup.Students)
        {
            var studentRecords = allRecords.Where(r => r.StudentId == student.StudentId).ToList();
            var presentDays = studentRecords.Count(r => r.IsPresent);
            var totalDays = (endOfWeek - startOfWeek).Days + 1;

            Console.WriteLine("\nStudent: " + student.Name);
            Console.WriteLine("Present Days: " + presentDays);
            Console.WriteLine("Absent Days: " + (totalDays - presentDays));
            Console.WriteLine("Attendance Percentage: " + (presentDays * 100 / totalDays) + "%");

            Console.WriteLine("Daily Breakdown:");
            for (var day = startOfWeek; day <= endOfWeek; day = day.AddDays(1))
            {
                var status = studentRecords.FirstOrDefault(r => r.Date.Date == day.Date)?.IsPresent ?? false ? "Present" : "Absent";
                Console.WriteLine($"{day:yyyy-MM-dd}: {status}");
            }
        }
    }

    private void ManageGroups(string dataFolder)
    {
        Console.WriteLine("Group management functionality not implemented");
    }

    private void RecordAttendance(string dataFolder)
    {
        Console.WriteLine("Attendance recording functionality not implemented");
    }

    private void ViewEditAttendance(string dataFolder)
    {
        Console.WriteLine("Attendance view/edit functionality not implemented");
    }

    private void ListGroups(List<Group> groups)
    {
        foreach (var group in groups)
        {
            Console.WriteLine($"Group ID: {group.GroupId}, Name: {group.GroupName}");
        }
    }
}

public static class DataStorageHelper
{
    public static List<T> LoadData<T>(string dataFolder, string fileName)
    {
        var path = Path.Combine(dataFolder, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        return new List<T>();
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
    public string GroupId { get; set; } = "";
    public string GroupName { get; set; } = "";
    public List<Student> Students { get; set; } = new List<Student>();
}

public class Student
{
    public string StudentId { get; set; } = "";
    public string Name { get; set; } = "";
}

public class AttendanceRecord
{
    public string GroupId { get; set; } = "";
    public string StudentId { get; set; } = "";
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
}