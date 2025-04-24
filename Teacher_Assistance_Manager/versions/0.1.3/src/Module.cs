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
        Console.WriteLine("4. Edit Student");
        Console.WriteLine("5. Delete Student");
        Console.WriteLine("6. Edit Group Name");
        Console.WriteLine("7. Delete Group");
        
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
                AddStudentToGroup(dataFolder, groups);
                break;
            case "4":
                EditStudentInGroup(dataFolder, groups);
                break;
            case "5":
                DeleteStudentFromGroup(dataFolder, groups);
                break;
            case "6":
                EditGroup(dataFolder, groups);
                break;
            case "7":
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

    private void AddStudentToGroup(string dataFolder, List<Group> groups)
    {
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

    private void EditStudentInGroup(string dataFolder, List<Group> groups)
    {
        ListGroups(groups);
        Console.WriteLine("Enter group ID to edit students:");
        var groupId = Console.ReadLine();
        
        var group = groups.FirstOrDefault(g => g.GroupId == groupId);
        if (group == null)
        {
            Console.WriteLine("Group not found!");
            return;
        }
        
        Console.WriteLine("Students in group:");
        foreach (var student in group.Students)
        {
            Console.WriteLine($"{student.Name} ({student.StudentId})");
        }
        
        Console.WriteLine("Enter student ID to edit:");
        var studentId = Console.ReadLine();
        var selectedStudent = group.Students.FirstOrDefault(s => s.StudentId == studentId);
        if (selectedStudent == null)
        {
            Console.WriteLine("Student not found!");
            return;
        }
        
        Console.WriteLine("Enter new student name:");
        selectedStudent.Name = Console.ReadLine();
        DataStorageHelper.SaveData(dataFolder, "groups.json", groups);
        Console.WriteLine("Student updated successfully");
    }

    private void DeleteStudentFromGroup(string dataFolder, List<Group> groups)
    {
        ListGroups(groups);
        Console.WriteLine("Enter group ID to delete student from:");
        var groupId = Console.ReadLine();
        
        var group = groups.FirstOrDefault(g => g.GroupId == groupId);
        if (group == null)
        {
            Console.WriteLine("Group not found!");
            return;
        }
        
        Console.WriteLine("Students in group:");
        foreach (var student in group.Students)
        {
            Console.WriteLine($"{student.Name} ({student.StudentId})");
        }
        
        Console.WriteLine("Enter student ID to delete:");
        var studentId = Console.ReadLine();
        var selectedStudent = group.Students.FirstOrDefault(s => s.StudentId == studentId);
        if (selectedStudent == null)
        {
            Console.WriteLine("Student not found!");
            return;
        }
        
        group.Students.Remove(selectedStudent);
        DataStorageHelper.SaveData(dataFolder, "groups.json", groups);
        Console.WriteLine("Student deleted successfully");
    }

    private void RecordAttendance(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        if (groups.Count == 0)
        {
            Console.WriteLine("No groups available. Please create a group first.");
            return;
        }

        Console.WriteLine("Select a group to record attendance:");
        ListGroups(groups);
        Console.WriteLine("Enter group ID:");
        var groupId = Console.ReadLine();
        var selectedGroup = groups.FirstOrDefault(g => g.GroupId == groupId);

        if (selectedGroup == null)
        {
            Console.WriteLine("Invalid group selection");
            return;
        }

        var attendanceDate = DateTime.Today;
        var attendanceRecords = DataStorageHelper.LoadData<AttendanceRecord>(dataFolder, "attendance.json");
        var existingRecords = attendanceRecords
            .Where(r => r.GroupId == groupId && r.Date.Date == attendanceDate.Date)
            .ToList();

        if (existingRecords.Any())
        {
            Console.WriteLine("Attendance already recorded for today. Overwrite? (Y/N)");
            if (Console.ReadLine().Trim().ToUpper() != "Y") return;
            attendanceRecords.RemoveAll(r => r.GroupId == groupId && r.Date.Date == attendanceDate.Date);
        }

        foreach (var student in selectedGroup.Students)
        {
            Console.WriteLine($"Is {student.Name} present? (Y/N)");
            var isPresent = Console.ReadLine().Trim().ToUpper() == "Y";
            attendanceRecords.Add(new AttendanceRecord
            {
                GroupId = groupId,
                StudentId = student.StudentId,
                Date = attendanceDate,
                IsPresent = isPresent
            });
        }

        DataStorageHelper.SaveData(dataFolder, "attendance.json", attendanceRecords);
        Console.WriteLine("Attendance recorded successfully!");
    }

    private void ViewEditAttendance(string dataFolder)
    {
        var groups = DataStorageHelper.LoadData<Group>(dataFolder, "groups.json");
        if (groups.Count == 0)
        {
            Console.WriteLine("No groups available. Please create a group first.");
            return;
        }

        Console.WriteLine("Select a group to view attendance:");
        ListGroups(groups);
        Console.WriteLine("Enter group ID:");
        var groupId = Console.ReadLine();
        var selectedGroup = groups.FirstOrDefault(g => g.GroupId == groupId);

        if (selectedGroup == null)
        {
            Console.WriteLine("Invalid group selection");
            return;
        }

        Console.WriteLine("Enter date to view (yyyy-MM-dd) or press Enter for today:");
        var dateInput = Console.ReadLine();
        DateTime targetDate;
        if (!DateTime.TryParse(dateInput, out targetDate))
        {
            targetDate = DateTime.Today;
        }

        var attendanceRecords = DataStorageHelper.LoadData<AttendanceRecord>(dataFolder, "attendance.json")
            .Where(r => r.GroupId == groupId && r.Date.Date == targetDate.Date)
            .ToList();

        if (attendanceRecords.Count == 0)
        {
            Console.WriteLine("No attendance records found for this date.");
            return;
        }

        Console.WriteLine($"\nAttendance for {targetDate:yyyy-MM-dd}:");
        foreach (var record in attendanceRecords)
        {
            var student = selectedGroup.Students.FirstOrDefault(s => s.StudentId == record.StudentId);
            Console.WriteLine($"{student?.Name ?? "Unknown"}: {(record.IsPresent ? "Present" : "Absent")}");
        }

        Console.WriteLine("\nEdit records? (Y/N)");
        if (Console.ReadLine().Trim().ToUpper() == "Y")
        {
            foreach (var record in attendanceRecords.ToList())
            {
                var student = selectedGroup.Students.FirstOrDefault(s => s.StudentId == record.StudentId);
                Console.WriteLine($"{student?.Name ?? "Unknown"} - Current status: {(record.IsPresent ? "Present" : "Absent")}");
                Console.WriteLine("New status (P/A/Cancel):");
                var input = Console.ReadLine().Trim().ToUpper();
                
                if (input == "P")
                {
                    record.IsPresent = true;
                }
                else if (input == "A")
                {
                    record.IsPresent = false;
                }
                else
                {
                    Console.WriteLine("Edit cancelled for this student.");
                }
            }
            
            var allRecords = DataStorageHelper.LoadData<AttendanceRecord>(dataFolder, "attendance.json")
                .Where(r => !(r.GroupId == groupId && r.Date.Date == targetDate.Date))
                .Concat(attendanceRecords)
                .ToList();
            
            DataStorageHelper.SaveData(dataFolder, "attendance.json", allRecords);
            Console.WriteLine("Attendance records updated successfully!");
        }
    }

    private void GenerateWeeklySummary(string dataFolder)
    {
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

public class AttendanceRecord
{
    public string GroupId { get; set; }
    public string StudentId { get; set; }
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
}
