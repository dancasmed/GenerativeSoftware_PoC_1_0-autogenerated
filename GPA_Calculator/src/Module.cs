using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class GPACalculator : IGeneratedModule
{
    public string Name { get; set; } = "GPA Calculator";
    
    private class Course
    {
        public string Name { get; set; }
        public double Grade { get; set; }
        public int Credits { get; set; }
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("GPA Calculator module is running...");
        
        string filePath = Path.Combine(dataFolder, "courses.json");
        List<Course> courses;
        
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                courses = JsonSerializer.Deserialize<List<Course>>(json);
                Console.WriteLine("Loaded existing course data.");
            }
            else
            {
                courses = new List<Course>();
                Console.WriteLine("No existing course data found. Starting fresh.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading course data: " + ex.Message);
            return false;
        }
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Course");
            Console.WriteLine("2. Calculate GPA");
            Console.WriteLine("3. View Courses");
            Console.WriteLine("4. Save and Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddCourse(courses);
                    break;
                case "2":
                    CalculateGPA(courses);
                    break;
                case "3":
                    ViewCourses(courses);
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        try
        {
            string json = JsonSerializer.Serialize(courses);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Course data saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving course data: " + ex.Message);
            return false;
        }
    }
    
    private void AddCourse(List<Course> courses)
    {
        Console.Write("Enter course name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter grade (0-4 scale): ");
        if (!double.TryParse(Console.ReadLine(), out double grade) || grade < 0 || grade > 4)
        {
            Console.WriteLine("Invalid grade. Must be between 0 and 4.");
            return;
        }
        
        Console.Write("Enter credits: ");
        if (!int.TryParse(Console.ReadLine(), out int credits) || credits <= 0)
        {
            Console.WriteLine("Invalid credits. Must be a positive integer.");
            return;
        }
        
        courses.Add(new Course { Name = name, Grade = grade, Credits = credits });
        Console.WriteLine("Course added successfully.");
    }
    
    private void CalculateGPA(List<Course> courses)
    {
        if (courses.Count == 0)
        {
            Console.WriteLine("No courses available to calculate GPA.");
            return;
        }
        
        double totalGradePoints = 0;
        int totalCredits = 0;
        
        foreach (var course in courses)
        {
            totalGradePoints += course.Grade * course.Credits;
            totalCredits += course.Credits;
        }
        
        double gpa = totalGradePoints / totalCredits;
        Console.WriteLine("Calculated GPA: " + gpa.ToString("0.00"));
    }
    
    private void ViewCourses(List<Course> courses)
    {
        if (courses.Count == 0)
        {
            Console.WriteLine("No courses available.");
            return;
        }
        
        Console.WriteLine("Current Courses:");
        Console.WriteLine("----------------");
        foreach (var course in courses)
        {
            Console.WriteLine("Name: " + course.Name);
            Console.WriteLine("Grade: " + course.Grade);
            Console.WriteLine("Credits: " + course.Credits);
            Console.WriteLine("----------------");
        }
    }
}