namespace GenerativeSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TeacherAssistantModule : IGeneratedModule
{
    public string Name { get; set; } = "Teacher Assistant Module";

    private Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
    private Dictionary<string, Dictionary<DateTime, List<string>>> attendanceRecords = new Dictionary<string, Dictionary<DateTime, List<string>>>();

    public bool Main(string dataFolder)
    {
        LoadData(dataFolder);

        while (true)
        {
            Console.WriteLine("\n1. Create Group");
            Console.WriteLine("2. Add Student to Group");
            Console.WriteLine("3. Record Attendance");
            Console.WriteLine("4. Generate Weekly Summary");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
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
                    GenerateWeeklySummary(dataFolder);
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
        string groupName = Console.ReadLine();
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
        string groupName = Console.ReadLine();
        if (groups.ContainsKey(groupName))
        {
            Console.Write("Enter student name: ");
            string studentName = Console.ReadLine();
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
        string groupName = Console.ReadLine();
        if (groups.ContainsKey(groupName))
        {
            DateTime today = DateTime.Today;
            if (!attendanceRecords[groupName].ContainsKey(today))
            {
                attendanceRecords[groupName][today] = new List<string>();
            }

            Console.WriteLine("Mark attendance for each student (P for Present, A for Absent):");
            foreach (var student in groups[groupName])
            {
                Console.Write($"{student}: ");
                string attendanceStatus = Console.ReadLine().ToUpper();
                if (attendanceStatus == "P")
                {
                    attendanceRecords[groupName][today].Add(student);
                }
            }
            Console.WriteLine("Attendance recorded successfully.");
        }
        else
        {
            Console.WriteLine("Group does not exist.");
        }
    }

    private void GenerateWeeklySummary(string dataFolder)
    {
        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        DateTime endOfWeek = startOfWeek.AddDays(6);

        foreach (var group in groups)
        {
            string groupName = group.Key;
            var students = group.Value;
            int totalDays = 0;
            int totalPresent = 0;

            foreach (var date in attendanceRecords[groupName].Keys)
            {
                if (date >= startOfWeek && date <= endOfWeek)
                {
                    totalDays++;
                    totalPresent += attendanceRecords[groupName][date].Count;
                }
            }

            double attendancePercentage = (double)totalPresent / (students.Count * totalDays) * 100;
            Console.WriteLine($"Group: {groupName}, Attendance Percentage: {attendancePercentage:F2}%");
        }
    }

    private void LoadData(string dataFolder)
    {
        string groupsFilePath = Path.Combine(dataFolder, "groups.json");
        string attendanceFilePath = Path.Combine(dataFolder, "attendance.json");

        if (File.Exists(groupsFilePath))
        {
            string groupsJson = File.ReadAllText(groupsFilePath);
            groups = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(groupsJson);
        }

        if (File.Exists(attendanceFilePath))
        {
            string attendanceJson = File.ReadAllText(attendanceFilePath);
            attendanceRecords = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<DateTime, List<string>>>>(attendanceJson);
        }
    }

    private void SaveData(string dataFolder)
    {
        string groupsFilePath = Path.Combine(dataFolder, "groups.json");
        string attendanceFilePath = Path.Combine(dataFolder, "attendance.json");

        string groupsJson = System.Text.Json.JsonSerializer.Serialize(groups);
        File.WriteAllText(groupsFilePath, groupsJson);

        string attendanceJson = System.Text.Json.JsonSerializer.Serialize(attendanceRecords);
        File.WriteAllText(attendanceFilePath, attendanceJson);
    }
}
