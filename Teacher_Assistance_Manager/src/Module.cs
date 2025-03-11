namespace GenerativeSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class TeacherModule : IGeneratedModule
{
    public string Name { get; set; } = "Teacher Assistance Manager";

    private Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
    private Dictionary<string, Dictionary<DateTime, List<string>>> attendanceRecords = new Dictionary<string, Dictionary<DateTime, List<string>>>();

    public bool Main(string dataFolder)
    {
        LoadData(dataFolder);

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Create a new group");
            Console.WriteLine("2. Add student to a group");
            Console.WriteLine("3. Record attendance for today");
            Console.WriteLine("4. Generate weekly summary");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    CreateGroup();
                    break;
                case "2":
                    AddStudentToGroup();
                    break;
                case "3":
                    RecordAttendance();
                    break;
                case "4":
                    GenerateWeeklySummary();
                    break;
                case "5":
                    SaveData(dataFolder);
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void CreateGroup()
    {
        Console.Write("Enter group name: ");
        var groupName = Console.ReadLine();
        if (!groups.ContainsKey(groupName))
        {
            groups[groupName] = new List<string>();
            attendanceRecords[groupName] = new Dictionary<DateTime, List<string>>();
            Console.WriteLine("Group created successfully.");
        }
        else
        {
            Console.WriteLine("Group already exists.");
        }
    }

    private void AddStudentToGroup()
    {
        Console.Write("Enter group name: ");
        var groupName = Console.ReadLine();
        if (groups.ContainsKey(groupName))
        {
            Console.Write("Enter student name: ");
            var studentName = Console.ReadLine();
            groups[groupName].Add(studentName);
            Console.WriteLine("Student added successfully.");
        }
        else
        {
            Console.WriteLine("Group does not exist.");
        }
    }

    private void RecordAttendance()
    {
        Console.Write("Enter group name: ");
        var groupName = Console.ReadLine();
        if (groups.ContainsKey(groupName))
        {
            var today = DateTime.Today;
            if (!attendanceRecords[groupName].ContainsKey(today))
            {
                attendanceRecords[groupName][today] = new List<string>();
            }

            Console.WriteLine("Mark attendance for each student (Y/N):");
            foreach (var student in groups[groupName])
            {
                Console.Write($"{student}: ");
                var response = Console.ReadLine().ToUpper();
                if (response == "Y")
                {
                    attendanceRecords[groupName][today].Add(student);
                }
            }
            Console.WriteLine("Attendance recorded for today.");
        }
        else
        {
            Console.WriteLine("Group does not exist.");
        }
    }

    private void GenerateWeeklySummary()
    {
        var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(6);

        foreach (var group in groups)
        {
            var groupName = group.Key;
            var totalDays = 0;
            var totalAttendance = 0;

            foreach (var day in attendanceRecords[groupName].Keys)
            {
                if (day >= startOfWeek && day <= endOfWeek)
                {
                    totalDays++;
                    totalAttendance += attendanceRecords[groupName][day].Count;
                }
            }

            var totalPossibleAttendance = groups[groupName].Count * totalDays;
            var attendancePercentage = totalPossibleAttendance > 0 ? (double)totalAttendance / totalPossibleAttendance * 100 : 0;

            Console.WriteLine($"Group: {groupName}, Weekly Attendance: {attendancePercentage:F2}%");
        }
    }

    private void LoadData(string dataFolder)
    {
        var groupsPath = Path.Combine(dataFolder, "groups.json");
        var attendancePath = Path.Combine(dataFolder, "attendance.json");

        if (File.Exists(groupsPath))
        {
            var json = File.ReadAllText(groupsPath);
            groups = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
        }

        if (File.Exists(attendancePath))
        {
            var json = File.ReadAllText(attendancePath);
            attendanceRecords = JsonSerializer.Deserialize<Dictionary<string, Dictionary<DateTime, List<string>>>>(json);
        }
    }

    private void SaveData(string dataFolder)
    {
        var groupsPath = Path.Combine(dataFolder, "groups.json");
        var attendancePath = Path.Combine(dataFolder, "attendance.json");

        var groupsJson = JsonSerializer.Serialize(groups);
        File.WriteAllText(groupsPath, groupsJson);

        var attendanceJson = JsonSerializer.Serialize(attendanceRecords);
        File.WriteAllText(attendancePath, attendanceJson);
    }
}
