using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class AttendanceManager : IGeneratedModule
{
    public string Name { get; set; } = "Teacher Attendance Manager";

    private Dictionary<string, List<DateTime>> _groupAttendance;
    private string _dataFilePath;

    public AttendanceManager()
    {
        _groupAttendance = new Dictionary<string, List<DateTime>>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Teacher Attendance Manager module is running.");
        _dataFilePath = Path.Combine(dataFolder, "attendance_data.json");

        LoadAttendanceData();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Record attendance");
            Console.WriteLine("2. View weekly summary");
            Console.WriteLine("3. Add new group");
            Console.WriteLine("4. Exit module");
            Console.Write("Select an option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        RecordAttendance();
                        break;
                    case 2:
                        ShowWeeklySummary();
                        break;
                    case 3:
                        AddNewGroup();
                        break;
                    case 4:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }

        SaveAttendanceData();
        Console.WriteLine("Attendance data saved. Module execution completed.");
        return true;
    }

    private void LoadAttendanceData()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                _groupAttendance = JsonSerializer.Deserialize<Dictionary<string, List<DateTime>>>(jsonData);
                Console.WriteLine("Attendance data loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading attendance data: " + ex.Message);
            }
        }
        else
        {
            Console.WriteLine("No existing attendance data found. Starting with empty records.");
        }
    }

    private void SaveAttendanceData()
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(_groupAttendance);
            File.WriteAllText(_dataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving attendance data: " + ex.Message);
        }
    }

    private void RecordAttendance()
    {
        if (_groupAttendance.Count == 0)
        {
            Console.WriteLine("No groups available. Please add a group first.");
            return;
        }

        Console.WriteLine("Available groups:");
        int index = 1;
        foreach (var group in _groupAttendance.Keys)
        {
            Console.WriteLine(index + ". " + group);
            index++;
        }

        Console.Write("Select group: ");
        if (int.TryParse(Console.ReadLine(), out int groupIndex) && groupIndex > 0 && groupIndex <= _groupAttendance.Count)
        {
            string selectedGroup = _groupAttendance.Keys.ElementAt(groupIndex - 1);
            DateTime today = DateTime.Today;

            if (!_groupAttendance[selectedGroup].Contains(today))
            {
                _groupAttendance[selectedGroup].Add(today);
                Console.WriteLine("Attendance recorded for " + selectedGroup + " on " + today.ToShortDateString());
            }
            else
            {
                Console.WriteLine("Attendance already recorded for " + selectedGroup + " today.");
            }
        }
        else
        {
            Console.WriteLine("Invalid group selection.");
        }
    }

    private void ShowWeeklySummary()
    {
        if (_groupAttendance.Count == 0)
        {
            Console.WriteLine("No attendance data available.");
            return;
        }

        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        DateTime endOfWeek = startOfWeek.AddDays(6);

        Console.WriteLine("\nWeekly Attendance Summary (" + startOfWeek.ToShortDateString() + " to " + endOfWeek.ToShortDateString() + "):");

        foreach (var group in _groupAttendance)
        {
            int daysPresent = group.Value.Count(date => date >= startOfWeek && date <= endOfWeek);
            double attendancePercentage = (daysPresent / 5.0) * 100; // Assuming 5 school days per week

            Console.WriteLine(group.Key + ": " + daysPresent + " days present (" + attendancePercentage.ToString("0.00") + "% attendance)");
        }
    }

    private void AddNewGroup()
    {
        Console.Write("Enter new group name: ");
        string groupName = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(groupName))
        {
            if (!_groupAttendance.ContainsKey(groupName))
            {
                _groupAttendance.Add(groupName, new List<DateTime>());
                Console.WriteLine("Group '" + groupName + "' added successfully.");
            }
            else
            {
                Console.WriteLine("Group '" + groupName + "' already exists.");
            }
        }
        else
        {
            Console.WriteLine("Group name cannot be empty.");
        }
    }
}