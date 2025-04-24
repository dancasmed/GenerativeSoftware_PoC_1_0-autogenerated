using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class AttendanceModule : IGeneratedModule
{
    public string Name { get; set; }

    public AttendanceModule()
    {
        Name = "Teacher Attendance Manager";
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Attendance Management Module...");
        
        var dataStorage = new DataStorage(dataFolder);
        var groups = dataStorage.LoadData<Group>();
        var students = dataStorage.LoadData<Student>();
        var attendanceRecords = dataStorage.LoadData<Attendance>();

        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Manage Groups");
            Console.WriteLine("2. Manage Students");
            Console.WriteLine("3. Record Attendance");
            Console.WriteLine("4. Generate Weekly Report");
            Console.WriteLine("5. Exit");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ManageGroups(dataStorage, groups);
                    break;
                case "2":
                    ManageStudents(dataStorage, students, groups);
                    break;
                case "3":
                    RecordAttendance(dataStorage, attendanceRecords, groups, students);
                    break;
                case "4":
                    GenerateWeeklyReport(dataStorage, groups, attendanceRecords);
                    break;
                case "5":
                    Console.WriteLine("Saving data and exiting module...");
                    return true;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private void ManageGroups(DataStorage dataStorage, List<Group> groups)
    {
        while (true)
        {
            Console.WriteLine("\nGroup Management:");
            Console.WriteLine("1. Create Group");
            Console.WriteLine("2. List Groups");
            Console.WriteLine("3. Edit Group");
            Console.WriteLine("4. Delete Group");
            Console.WriteLine("5. Back to Main Menu");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter group name: ");
                var name = Console.ReadLine();
                Console.Write("Enter group description: ");
                var desc = Console.ReadLine();

                groups.Add(new Group
                {
                    GroupId = Guid.NewGuid().ToString(),
                    GroupName = name,
                    Description = desc,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

                dataStorage.SaveData(groups);
                Console.WriteLine("Group created successfully");
            }
            else if (choice == "2")
            {
                Console.WriteLine("\nExisting Groups:");
                foreach (var group in groups)
                {
                    Console.WriteLine($"ID: {group.GroupId}, Name: {group.GroupName}, Description: {group.Description}");
                }
            }
            else if (choice == "3")
            {
                Console.Write("Enter Group ID to edit: ");
                var groupId = Console.ReadLine();
                var group = groups.FirstOrDefault(g => g.GroupId == groupId);
                if (group == null)
                {
                    Console.WriteLine("Group not found.");
                    continue;
                }

                Console.Write("Enter new group name (leave blank to keep current): ");
                var newName = Console.ReadLine();
                Console.Write("Enter new group description (leave blank to keep current): ");
                var newDesc = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newName))
                    group.GroupName = newName;
                if (!string.IsNullOrWhiteSpace(newDesc))
                    group.Description = newDesc;

                group.UpdatedAt = DateTime.Now;
                dataStorage.SaveData(groups);
                Console.WriteLine("Group updated successfully.");
            }
            else if (choice == "4")
            {
                Console.Write("Enter Group ID to delete: ");
                var groupId = Console.ReadLine();
                var group = groups.FirstOrDefault(g => g.GroupId == groupId);
                if (group == null)
                {
                    Console.WriteLine("Group not found.");
                    continue;
                }

                Console.Write($"Are you sure you want to delete group {group.GroupName}? (Y/N): ");
                var confirm = Console.ReadLine().ToUpper();
                if (confirm == "Y")
                {
                    groups.Remove(group);
                    dataStorage.SaveData(groups);
                    Console.WriteLine("Group deleted successfully.");
                }
            }
            else if (choice == "5")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid option");
            }
        }
    }

    private void ManageStudents(DataStorage dataStorage, List<Student> students, List<Group> groups)
    {
        while (true)
        {
            Console.WriteLine("\nStudent Management:");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. List Students");
            Console.WriteLine("3. Edit Student Group");
            Console.WriteLine("4. Remove Student");
            Console.WriteLine("5. Back to Main Menu");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter student name: ");
                var name = Console.ReadLine();
                Console.Write("Enter student email: ");
                var email = Console.ReadLine();
                Console.Write("Enter group ID: ");
                var groupId = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(groupId) && !groups.Any(g => g.GroupId == groupId))
                {
                    Console.WriteLine("Invalid group ID");
                    continue;
                }

                students.Add(new Student
                {
                    StudentId = Guid.NewGuid().ToString(),
                    GroupId = groupId,
                    Name = name,
                    Email = email,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

                dataStorage.SaveData(students);
                Console.WriteLine("Student added successfully");
            }
            else if (choice == "2")
            {
                Console.WriteLine("\nExisting Students:");
                foreach (var student in students)
                {
                    Console.WriteLine($"ID: {student.StudentId}, Name: {student.Name}, Email: {student.Email}, Group: {groups.FirstOrDefault(g => g.GroupId == student.GroupId)?.GroupName ?? "[No Group]"}");
                }
            }
            else if (choice == "3")
            {
                Console.Write("Enter Student ID to edit: ");
                var studentId = Console.ReadLine();
                var student = students.FirstOrDefault(s => s.StudentId == studentId);
                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    continue;
                }

                Console.Write("Enter new group ID (leave blank to remove from group): ");
                var newGroupId = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newGroupId) && !groups.Any(g => g.GroupId == newGroupId))
                {
                    Console.WriteLine("Invalid group ID. No changes made.");
                    continue;
                }

                student.GroupId = string.IsNullOrWhiteSpace(newGroupId) ? null : newGroupId;
                student.UpdatedAt = DateTime.Now;
                dataStorage.SaveData(students);
                Console.WriteLine("Student group updated successfully.");
            }
            else if (choice == "4")
            {
                Console.Write("Enter Student ID to remove: ");
                var studentId = Console.ReadLine();
                var student = students.FirstOrDefault(s => s.StudentId == studentId);
                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    continue;
                }

                Console.Write($"Are you sure you want to delete {student.Name}? (Y/N): ");
                var confirm = Console.ReadLine().ToUpper();
                if (confirm == "Y")
                {
                    students.Remove(student);
                    dataStorage.SaveData(students);
                    Console.WriteLine("Student removed successfully.");
                }
            }
            else if (choice == "5")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid option");
            }
        }
    }

    private void RecordAttendance(DataStorage dataStorage, List<Attendance> attendanceRecords, List<Group> groups, List<Student> students)
    {
        Console.Write("Enter group ID: ");
        var groupId = Console.ReadLine();

        var groupStudents = students.Where(s => s.GroupId == groupId).ToList();
        if (!groupStudents.Any())
        {
            Console.WriteLine("No students in this group");
            return;
        }

        Console.WriteLine("Marking attendance for " + DateTime.Today.ToString("yyyy-MM-dd"));
        foreach (var student in groupStudents)
        {
            Console.Write("Is " + student.Name + " present? (Y/N): ");
            var status = Console.ReadLine().ToUpper() == "Y" ? "Present" : "Absent";

            attendanceRecords.Add(new Attendance
            {
                AttendanceId = Guid.NewGuid().ToString(),
                GroupId = groupId,
                StudentId = student.StudentId,
                Date = DateTime.Today,
                Status = status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }

        dataStorage.SaveData(attendanceRecords);
        Console.WriteLine("Attendance recorded successfully");
    }

    private void GenerateWeeklyReport(DataStorage dataStorage, List<Group> groups, List<Attendance> attendanceRecords)
    {
        var currentDate = DateTime.Today;
        var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
        var endOfWeek = startOfWeek.AddDays(6);

        Console.WriteLine("\nWeekly Report from " + startOfWeek.ToString("yyyy-MM-dd") + " to " + endOfWeek.ToString("yyyy-MM-dd"));

        foreach (var group in groups)
        {
            var groupAttendance = attendanceRecords
                .Where(a => a.GroupId == group.GroupId && a.Date >= startOfWeek && a.Date <= endOfWeek)
                .ToList();

            var presentCount = groupAttendance.Count(a => a.Status == "Present");
            var totalSessions = groupAttendance.Count;
            var percentage = totalSessions > 0 ? (presentCount * 100.0 / totalSessions) : 0;

            Console.WriteLine("Group: " + group.GroupName);
            Console.WriteLine("Attendance Percentage: " + percentage.ToString("0.00") + "%");
        }
    }
}

public class DataStorage
{
    private readonly string _dataFolder;

    public DataStorage(string dataFolder)
    {
        _dataFolder = dataFolder;
        Directory.CreateDirectory(dataFolder);
    }

    public List<T> LoadData<T>()
    {
        var fileName = Path.Combine(_dataFolder, typeof(T).Name + ".json");
        if (!File.Exists(fileName)) return new List<T>();

        var json = File.ReadAllText(fileName);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public void SaveData<T>(List<T> data)
    {
        var fileName = Path.Combine(_dataFolder, typeof(T).Name + ".json");
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(fileName, json);
    }
}

public class Group
{
    public string GroupId { get; set; }
    public string GroupName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Student
{
    public string StudentId { get; set; }
    public string GroupId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Attendance
{
    public string AttendanceId { get; set; }
    public string GroupId { get; set; }
    public string StudentId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
