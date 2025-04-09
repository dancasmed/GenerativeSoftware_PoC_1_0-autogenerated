using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class AcademicGradeTracker : IGeneratedModule
{
    public string Name { get; set; } = "Academic Grade Tracker";

    private string _gradesFilePath;
    private List<Course> _courses;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Academic Grade Tracker module is running.");
        _gradesFilePath = Path.Combine(dataFolder, "grades.json");
        _courses = LoadGrades();

        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddCourse();
                    break;
                case "2":
                    ViewGrades();
                    break;
                case "3":
                    CalculateGPA();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveGrades();
        return true;
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nAcademic Grade Tracker");
        Console.WriteLine("1. Add Course");
        Console.WriteLine("2. View Grades");
        Console.WriteLine("3. Calculate GPA");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }

    private void AddCourse()
    {
        Console.Write("Enter course name: ");
        string name = Console.ReadLine();

        Console.Write("Enter credit hours: ");
        if (!int.TryParse(Console.ReadLine(), out int credits))
        {
            Console.WriteLine("Invalid credit hours. Please enter a number.");
            return;
        }

        Console.Write("Enter grade (A, B, C, D, F): ");
        string grade = Console.ReadLine().ToUpper();

        if (!IsValidGrade(grade))
        {
            Console.WriteLine("Invalid grade. Please enter A, B, C, D, or F.");
            return;
        }

        _courses.Add(new Course { Name = name, Credits = credits, Grade = grade });
        Console.WriteLine("Course added successfully.");
    }

    private bool IsValidGrade(string grade)
    {
        return grade == "A" || grade == "B" || grade == "C" || grade == "D" || grade == "F";
    }

    private void ViewGrades()
    {
        if (_courses.Count == 0)
        {
            Console.WriteLine("No courses available.");
            return;
        }

        Console.WriteLine("\nCurrent Grades:");
        foreach (var course in _courses)
        {
            Console.WriteLine($"{course.Name} - Credits: {course.Credits}, Grade: {course.Grade}");
        }
    }

    private void CalculateGPA()
    {
        if (_courses.Count == 0)
        {
            Console.WriteLine("No courses available to calculate GPA.");
            return;
        }

        double totalPoints = 0;
        int totalCredits = 0;

        foreach (var course in _courses)
        {
            double gradePoints = GetGradePoints(course.Grade);
            totalPoints += gradePoints * course.Credits;
            totalCredits += course.Credits;
        }

        double gpa = totalPoints / totalCredits;
        Console.WriteLine($"\nYour GPA is: {gpa:F2}");
    }

    private double GetGradePoints(string grade)
    {
        switch (grade)
        {
            case "A": return 4.0;
            case "B": return 3.0;
            case "C": return 2.0;
            case "D": return 1.0;
            case "F": return 0.0;
            default: return 0.0;
        }
    }

    private List<Course> LoadGrades()
    {
        if (!File.Exists(_gradesFilePath))
        {
            return new List<Course>();
        }

        string json = File.ReadAllText(_gradesFilePath);
        return JsonSerializer.Deserialize<List<Course>>(json);
    }

    private void SaveGrades()
    {
        string json = JsonSerializer.Serialize(_courses);
        File.WriteAllText(_gradesFilePath, json);
    }
}

public class Course
{
    public string Name { get; set; }
    public int Credits { get; set; }
    public string Grade { get; set; }
}