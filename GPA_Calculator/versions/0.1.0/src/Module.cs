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
            }
            else
            {
                courses = GetSampleCourses();
                string json = JsonSerializer.Serialize(courses);
                File.WriteAllText(filePath, json);
            }

            double gpa = CalculateGPA(courses);
            Console.WriteLine("Calculated GPA: " + gpa.ToString("F2"));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error calculating GPA: " + ex.Message);
            return false;
        }
    }

    private List<Course> GetSampleCourses()
    {
        return new List<Course>
        {
            new Course { Name = "Mathematics", Grade = 3.7, Credits = 4 },
            new Course { Name = "Physics", Grade = 3.3, Credits = 3 },
            new Course { Name = "Chemistry", Grade = 3.0, Credits = 3 },
            new Course { Name = "Literature", Grade = 4.0, Credits = 2 }
        };
    }

    private double CalculateGPA(List<Course> courses)
    {
        double totalGradePoints = 0;
        int totalCredits = 0;

        foreach (var course in courses)
        {
            totalGradePoints += course.Grade * course.Credits;
            totalCredits += course.Credits;
        }

        return totalGradePoints / totalCredits;
    }
}