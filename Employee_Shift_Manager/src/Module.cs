using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class EmployeeShiftManager : IGeneratedModule
{
    public string Name { get; set; } = "Employee Shift Manager";
    
    private string _shiftsFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Employee Shift Manager module is running.");
        
        _shiftsFilePath = Path.Combine(dataFolder, "shifts.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddShift();
                    break;
                case "2":
                    ViewShifts();
                    break;
                case "3":
                    DeleteShift();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nEmployee Shift Management");
        Console.WriteLine("1. Add Shift");
        Console.WriteLine("2. View Shifts");
        Console.WriteLine("3. Delete Shift");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddShift()
    {
        Console.Write("Enter employee name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter shift date (yyyy-MM-dd): ");
        string dateInput = Console.ReadLine();
        
        Console.Write("Enter start time (HH:mm): ");
        string startTime = Console.ReadLine();
        
        Console.Write("Enter end time (HH:mm): ");
        string endTime = Console.ReadLine();
        
        var shifts = LoadShifts();
        
        shifts.Add(new EmployeeShift
        {
            EmployeeName = name,
            Date = dateInput,
            StartTime = startTime,
            EndTime = endTime
        });
        
        SaveShifts(shifts);
        Console.WriteLine("Shift added successfully.");
    }
    
    private void ViewShifts()
    {
        var shifts = LoadShifts();
        
        if (shifts.Count == 0)
        {
            Console.WriteLine("No shifts found.");
            return;
        }
        
        Console.WriteLine("\nCurrent Shifts:");
        foreach (var shift in shifts)
        {
            Console.WriteLine($"{shift.EmployeeName} - {shift.Date} {shift.StartTime} to {shift.EndTime}");
        }
    }
    
    private void DeleteShift()
    {
        ViewShifts();
        
        var shifts = LoadShifts();
        if (shifts.Count == 0) return;
        
        Console.Write("Enter the index of the shift to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < shifts.Count)
        {
            shifts.RemoveAt(index);
            SaveShifts(shifts);
            Console.WriteLine("Shift deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid index.");
        }
    }
    
    private List<EmployeeShift> LoadShifts()
    {
        if (!File.Exists(_shiftsFilePath))
        {
            return new List<EmployeeShift>();
        }
        
        string json = File.ReadAllText(_shiftsFilePath);
        return JsonSerializer.Deserialize<List<EmployeeShift>>(json);
    }
    
    private void SaveShifts(List<EmployeeShift> shifts)
    {
        string json = JsonSerializer.Serialize(shifts);
        File.WriteAllText(_shiftsFilePath, json);
    }
}

public class EmployeeShift
{
    public string EmployeeName { get; set; }
    public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}