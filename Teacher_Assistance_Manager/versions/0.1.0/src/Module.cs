using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public enum AttendanceStatus { Present, Absent, Late, Excused }

public class Group
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string GroupId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class Attendance
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string StudentId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public AttendanceStatus Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class WeeklySummary
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GroupId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public int TotalStudents { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public int TotalExcused { get; set; }
    public float AttendancePercentage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class GroupService
{
    private readonly string _dataPath;

    public GroupService(string dataFolder)
    {
        _dataPath = Path.Combine(dataFolder, "groups.json");
    }

    public List<Group> LoadGroups() => File.Exists(_dataPath) 
        ? JsonConvert.DeserializeObject<List<Group>>(File.ReadAllText(_dataPath)) 
        : new List<Group>();

    public void SaveGroups(List<Group> groups) => File.WriteAllText(_dataPath, JsonConvert.SerializeObject(groups));
}

public class StudentService
{
    private readonly string _dataPath;

    public StudentService(string dataFolder)
    {
        _dataPath = Path.Combine(dataFolder, "students.json");
    }

    public List<Student> LoadStudents() => File.Exists(_dataPath)
        ? JsonConvert.DeserializeObject<List<Student>>(File.ReadAllText(_dataPath))
        : new List<Student>();

    public void SaveStudents(List<Student> students) => File.WriteAllText(_dataPath, JsonConvert.SerializeObject(students));
}

public class AttendanceService
{
    private readonly string _dataPath;

    public AttendanceService(string dataFolder)
    {
        _dataPath = Path.Combine(dataFolder, "attendances.json");
    }

    public List<Attendance> LoadAttendances() => File.Exists(_dataPath)
        ? JsonConvert.DeserializeObject<List<Attendance>>(File.ReadAllText(_dataPath))
        : new List<Attendance>();

    public void SaveAttendances(List<Attendance> attendances) => File.WriteAllText(_dataPath, JsonConvert.SerializeObject(attendances));
}

public class WeeklySummaryService
{
    private readonly string _dataPath;

    public WeeklySummaryService(string dataFolder)
    {
        _dataPath = Path.Combine(dataFolder, "summaries.json");
    }

    public List<WeeklySummary> LoadSummaries() => File.Exists(_dataPath)
        ? JsonConvert.DeserializeObject<List<WeeklySummary>>(File.ReadAllText(_dataPath))
        : new List<WeeklySummary>();

    public void SaveSummaries(List<WeeklySummary> summaries) => File.WriteAllText(_dataPath, JsonConvert.SerializeObject(summaries));
}

public class AttendanceModule : IGeneratedModule
{
    public string Name { get; set; } = "Attendance Management System";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Attendance Management Module...");

        var groupService = new GroupService(dataFolder);
        var studentService = new StudentService(dataFolder);
        var attendanceService = new AttendanceService(dataFolder);
        var summaryService = new WeeklySummaryService(dataFolder);

        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Group Management");
            Console.WriteLine("2. Student Management");
            Console.WriteLine("3. Record Attendance");
            Console.WriteLine("4. View Daily Attendance");
            Console.WriteLine("5. Generate Weekly Summary");
            Console.WriteLine("6. Exit Module");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": ManageGroups(groupService); break;
                case "2": ManageStudents(studentService, groupService); break;
                case "3": RecordAttendance(attendanceService, studentService, groupService); break;
                case "4": ViewDailyAttendance(attendanceService, groupService); break;
                case "5": GenerateWeeklySummary(summaryService, groupService, attendanceService); break;
                case "6": Console.WriteLine("Exiting module..."); return true;
                default: Console.WriteLine("Invalid option"); break;
            }
        }
    }

    private void ManageGroups(GroupService service)
    {
        while (true)
        {
            Console.WriteLine("\nGroup Management:");
            Console.WriteLine("1. Create Group");
            Console.WriteLine("2. List All Groups");
            Console.WriteLine("3. Update Group");
            Console.WriteLine("4. Delete Group");
            Console.WriteLine("5. Search Groups");
            Console.WriteLine("6. Return to Main Menu");

            var choice = Console.ReadLine();
            var groups = service.LoadGroups();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter group name: ");
                    var name = Console.ReadLine();
                    Console.Write("Enter group description: ");
                    var description = Console.ReadLine();

                    groups.Add(new Group { Name = name, Description = description });
                    service.SaveGroups(groups);
                    Console.WriteLine("Group created successfully!");
                    break;

                case "2":
                    Console.WriteLine("\nAll Groups:");
                    foreach (var group in groups)
                    {
                        Console.WriteLine($"ID: {group.Id} | Name: {group.Name} | Description: {group.Description} | Created: {group.CreatedAt}");
                    }
                    break;

                case "3":
                    Console.Write("Enter group ID to update: ");
                    var updateId = Console.ReadLine();
                    var groupToUpdate = groups.FirstOrDefault(g => g.Id == updateId);
                    if (groupToUpdate != null)
                    {
                        Console.Write("New name (press enter to keep current): ");
                        var newName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newName)) groupToUpdate.Name = newName;

                        Console.Write("New description (press enter to keep current): ");
                        var newDesc = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newDesc)) groupToUpdate.Description = newDesc;

                        groupToUpdate.UpdatedAt = DateTime.Now;
                        service.SaveGroups(groups);
                        Console.WriteLine("Group updated successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Group not found!");
                    }
                    break;

                case "4":
                    Console.Write("Enter group ID to delete: ");
                    var deleteId = Console.ReadLine();
                    var groupToDelete = groups.FirstOrDefault(g => g.Id == deleteId);
                    if (groupToDelete != null)
                    {
                        groups.Remove(groupToDelete);
                        service.SaveGroups(groups);
                        Console.WriteLine("Group deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Group not found!");
                    }
                    break;

                case "5":
                    Console.Write("Enter search term: ");
                    var searchTerm = Console.ReadLine().ToLower();
                    var results = groups.Where(g => 
                        g.Name.ToLower().Contains(searchTerm) || 
                        g.Description.ToLower().Contains(searchTerm))
                        .ToList();

                    Console.WriteLine($"\nFound {results.Count} groups:");
                    foreach (var group in results)
                    {
                        Console.WriteLine($"ID: {group.Id} | Name: {group.Name} | Description: {group.Description}");
                    }
                    break;

                case "6":
                    return;

                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private void ManageStudents(StudentService studentService, GroupService groupService)
    {
        while (true)
        {
            Console.WriteLine("\nStudent Management:");
            Console.WriteLine("1. Create Student");
            Console.WriteLine("2. List All Students");
            Console.WriteLine("3. Update Student");
            Console.WriteLine("4. Delete Student");
            Console.WriteLine("5. Search Students");
            Console.WriteLine("6. Return to Main Menu");

            var choice = Console.ReadLine();
            var students = studentService.LoadStudents();
            var groups = groupService.LoadGroups();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter student name: ");
                    var name = Console.ReadLine();
                    
                    Console.WriteLine("Available Groups:");
                    foreach (var group in groups)
                    {
                        Console.WriteLine($"{group.Id}: {group.Name}");
                    }
                    Console.Write("Enter group ID: ");
                    var groupId = Console.ReadLine();

                    if (groups.Any(g => g.Id == groupId))
                    {
                        students.Add(new Student { Name = name, GroupId = groupId });
                        studentService.SaveStudents(students);
                        Console.WriteLine("Student created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid group ID!");
                    }
                    break;

                case "2":
                    Console.WriteLine("\nAll Students:");
                    foreach (var student in students)
                    {
                        var groupName = groups.FirstOrDefault(g => g.Id == student.GroupId)?.Name ?? "Unknown Group";
                        Console.WriteLine($"ID: {student.Id} | Name: {student.Name} | Group: {groupName} | Created: {student.CreatedAt}");
                    }
                    break;

                case "3":
                    Console.Write("Enter student ID to update: ");
                    var updateId = Console.ReadLine();
                    var studentToUpdate = students.FirstOrDefault(s => s.Id == updateId);
                    if (studentToUpdate != null)
                    {
                        Console.Write("New name (press enter to keep current): ");
                        var newName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newName)) studentToUpdate.Name = newName;

                        Console.WriteLine("Available Groups:");
                        foreach (var group in groups)
                        {
                            Console.WriteLine($"{group.Id}: {group.Name}");
                        }
                        Console.Write("New group ID (press enter to keep current): ");
                        var newGroupId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newGroupId) 
                        {
                            if (groups.Any(g => g.Id == newGroupId))
                            {
                                studentToUpdate.GroupId = newGroupId;
                            }
                            else
                            {
                                Console.WriteLine("Invalid group ID, keeping current group");
                            }
                        }

                        studentToUpdate.UpdatedAt = DateTime.Now;
                        studentService.SaveStudents(students);
                        Console.WriteLine("Student updated successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Student not found!");
                    }
                    break;

                case "4":
                    Console.Write("Enter student ID to delete: ");
                    var deleteId = Console.ReadLine();
                    var studentToDelete = students.FirstOrDefault(s => s.Id == deleteId);
                    if (studentToDelete != null)
                    {
                        students.Remove(studentToDelete);
                        studentService.SaveStudents(students);
                        Console.WriteLine("Student deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Student not found!");
                    }
                    break;

                case "5":
                    Console.Write("Enter search term: ");
                    var searchTerm = Console.ReadLine().ToLower();
                    var results = students.Where(s => 
                        s.Name.ToLower().Contains(searchTerm) || 
                        s.GroupId.ToLower().Contains(searchTerm))
                        .ToList();

                    Console.WriteLine($"\nFound {results.Count} students:");
                    foreach (var student in results)
                    {
                        var groupName = groups.FirstOrDefault(g => g.Id == student.GroupId)?.Name ?? "Unknown Group";
                        Console.WriteLine($"ID: {student.Id} | Name: {student.Name} | Group: {groupName}");
                    }
                    break;

                case "6":
                    return;

                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private void RecordAttendance(AttendanceService attendanceService, StudentService studentService, GroupService groupService)
    {
        // Attendance recording implementation
    }

    private void ViewDailyAttendance(AttendanceService attendanceService, GroupService groupService)
    {
        // Attendance viewing implementation
    }

    private void GenerateWeeklySummary(WeeklySummaryService summaryService, GroupService groupService, AttendanceService attendanceService)
    {
        // Summary generation implementation
    }
}
