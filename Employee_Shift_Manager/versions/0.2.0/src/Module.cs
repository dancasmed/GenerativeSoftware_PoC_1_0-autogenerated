using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class EmployeeShiftManager : IGeneratedModule
{
    public string Name { get; set; } = "Employee Shift Manager";

    private string _dataFilePath;
    private List<Employee> _employees;
    private List<Shift> _shifts;

    public EmployeeShiftManager()
    {
        _employees = new List<Employee>();
        _shifts = new List<Shift>();
    }

    public bool Main(string dataFolder)
    {
        _dataFilePath = Path.Combine(dataFolder, "employee_shifts.json");
        Console.WriteLine("Employee Shift Manager module is running.");

        try
        {
            LoadData();
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\nEmployee Shift Manager");
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. Add Shift");
                Console.WriteLine("3. View Employees");
                Console.WriteLine("4. View Shifts");
                Console.WriteLine("5. Assign Shift to Employee");
                Console.WriteLine("6. Save and Exit");
                Console.Write("Select an option: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddEmployee();
                        break;
                    case "2":
                        AddShift();
                        break;
                    case "3":
                        ViewEmployees();
                        break;
                    case "4":
                        ViewShifts();
                        break;
                    case "5":
                        AssignShift();
                        break;
                    case "6":
                        SaveData();
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            var data = JsonSerializer.Deserialize<EmployeeShiftData>(json);
            _employees = data.Employees;
            _shifts = data.Shifts;
        }
    }

    private void SaveData()
    {
        var data = new EmployeeShiftData { Employees = _employees, Shifts = _shifts };
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(_dataFilePath, json);
        Console.WriteLine("Data saved successfully.");
    }

    private void AddEmployee()
    {
        Console.Write("Enter employee name: ");
        string name = Console.ReadLine();
        Console.Write("Enter employee ID: ");
        string id = Console.ReadLine();

        _employees.Add(new Employee { Name = name, Id = id });
        Console.WriteLine("Employee added successfully.");
    }

    private void AddShift()
    {
        Console.Write("Enter shift date (yyyy-MM-dd): ");
        string date = Console.ReadLine();
        Console.Write("Enter start time (HH:mm): ");
        string startTime = Console.ReadLine();
        Console.Write("Enter end time (HH:mm): ");
        string endTime = Console.ReadLine();

        _shifts.Add(new Shift { Date = date, StartTime = startTime, EndTime = endTime });
        Console.WriteLine("Shift added successfully.");
    }

    private void ViewEmployees()
    {
        if (_employees.Count == 0)
        {
            Console.WriteLine("No employees found.");
            return;
        }

        Console.WriteLine("\nEmployees:");
        foreach (var employee in _employees)
        {
            Console.WriteLine($"ID: {employee.Id}, Name: {employee.Name}");
        }
    }

    private void ViewShifts()
    {
        if (_shifts.Count == 0)
        {
            Console.WriteLine("No shifts found.");
            return;
        }

        Console.WriteLine("\nShifts:");
        foreach (var shift in _shifts)
        {
            Console.WriteLine($"Date: {shift.Date}, Start: {shift.StartTime}, End: {shift.EndTime}");
        }
    }

    private void AssignShift()
    {
        ViewEmployees();
        ViewShifts();

        if (_employees.Count == 0 || _shifts.Count == 0)
        {
            Console.WriteLine("No employees or shifts available to assign.");
            return;
        }

        Console.Write("Enter employee ID: ");
        string employeeId = Console.ReadLine();
        Console.Write("Enter shift index: ");
        string shiftIndexInput = Console.ReadLine();

        if (!int.TryParse(shiftIndexInput, out int shiftIndex) || shiftIndex < 0 || shiftIndex >= _shifts.Count)
        {
            Console.WriteLine("Invalid shift index.");
            return;
        }

        var employee = _employees.Find(e => e.Id == employeeId);
        if (employee == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        employee.AssignedShifts.Add(_shifts[shiftIndex]);
        Console.WriteLine("Shift assigned successfully.");
    }
}

public class Employee
{
    public string Name { get; set; }
    public string Id { get; set; }
    public List<Shift> AssignedShifts { get; set; } = new List<Shift>();
}

public class Shift
{
    public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}

public class EmployeeShiftData
{
    public List<Employee> Employees { get; set; }
    public List<Shift> Shifts { get; set; }
}