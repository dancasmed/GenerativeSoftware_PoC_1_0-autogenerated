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
            CreationDate = DateTime.Now
        };
        
        groups.Add(newGroup);
        DataStorageHelper.SaveData(dataFolder, "groups.json", groups);
        Console.WriteLine("Group created successfully");
    }

    private void RecordAttendance(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        var students = DataStorageHelper.LoadData<Student>(dataFolder, "students.json");
        
        Console.WriteLine("Select group:");
        ListGroups(groups);
        var groupId = Console.ReadLine();
        
        var groupStudents = students.Where(s => s.GroupId == groupId).ToList();
        var attendanceRecords = new List<Attendance>();
        
        foreach (var student in groupStudents)
        {
            Console.WriteLine("Mark attendance for " + student.FirstName + " " + student.LastName + " (P/A):");
            var status = Console.ReadLine().ToUpper() == "P" ? "Present" : "Absent";
            
            attendanceRecords.Add(new Attendance
            {
                AttendanceId = Guid.NewGuid().ToString(),
                StudentId = student.StudentId,
                Date = DateTime.Now,
                Status = status,
                GroupId = groupId
            });
        }
        
        var existingAttendances = DataStorageHelper.LoadData<Attendance>(dataFolder, "attendances.json");
        existingAttendances.AddRange(attendanceRecords);
        DataStorageHelper.SaveData(dataFolder, "attendances.json", existingAttendances);
        Console.WriteLine("Attendance recorded successfully");
    }

    private void GenerateWeeklySummary(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        var attendances = DataStorageHelper.LoadData<Attendance>(dataFolder, "attendances.json");
        
        Console.WriteLine("Select group:");
        ListGroups(groups);
        var groupId = Console.ReadLine();
        
        var weekStart = DateTime.Now.AddDays(-7);
        var groupAttendances = attendances
            .Where(a => a.GroupId == groupId && a.Date >= weekStart)
            .ToList();
        
        var summary = new WeeklySummary
        {
            SummaryId = Guid.NewGuid().ToString(),
            GroupId = groupId,
            WeekStartDate = weekStart,
            WeekEndDate = DateTime.Now,
            TotalStudents = groupAttendances.Select(a => a.StudentId).Distinct().Count(),
            TotalPresent = groupAttendances.Count(a => a.Status == "Present"),
            Percentage = (float)groupAttendances.Count(a => a.Status == "Present") / groupAttendances.Count
        };
        
        var summaries = DataStorageHelper.LoadData<WeeklySummary>(dataFolder, "summaries.json");
        summaries.Add(summary);
        DataStorageHelper.SaveData(dataFolder, "summaries.json", summaries);
        
        Console.WriteLine("Weekly Summary Generated:");
        Console.WriteLine("Percentage: " + summary.Percentage.ToString("P"));
    }

    private static void ListGroups(List<Group> groups)
    {
        foreach (var group in groups)
        {
            Console.WriteLine(group.GroupId + " - " + group.GroupName);
        }
    }

    private void AddStudentToGroup(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        Console.WriteLine("Available Groups:");
        ListGroups(groups);

        Console.WriteLine("\nEnter group ID to add student to:");
        var groupId = Console.ReadLine();

        if (!groups.Any(g => g.GroupId == groupId))
        {
            Console.WriteLine("Invalid group ID!");
            return;
        }

        var students = DataStorageHelper.LoadData<Student>(dataFolder, "students.json");
        
        Console.WriteLine("Enter student first name:");
        var firstName = Console.ReadLine();
        Console.WriteLine("Enter student last name:");
        var lastName = Console.ReadLine();
        
        var newStudent = new Student
        {
            StudentId = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            GroupId = groupId
        };
        
        students.Add(newStudent);
        DataStorageHelper.SaveData(dataFolder, "students.json", students);
        Console.WriteLine("Student added to group successfully");
    }

    private void ViewEditAttendance(string dataFolder)
    {
        var attendances = DataStorageHelper.LoadData<Attendance>(dataFolder, "attendances.json");
        
        Console.WriteLine("Enter date (yyyy-MM-dd):");
        if (DateTime.TryParse(Console.ReadLine(), out var date))
        {
            var dailyAttendances = attendances.Where(a => a.Date.Date == date.Date).ToList();
            foreach (var att in dailyAttendances)
            {
                Console.WriteLine(att.Date + " - " + att.Status);
            }
        }
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
    public string TeacherId { get; set; }
}

public class Student
{
    public string StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string GroupId { get; set; }
}

public class Attendance
{
    public string AttendanceId { get; set; }
    public string StudentId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public string GroupId { get; set; }
}

public class WeeklySummary
{
    public string SummaryId { get; set; }
    public string GroupId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public int TotalStudents { get; set; }
    public int TotalPresent { get; set; }
    public float Percentage { get; set; }
}