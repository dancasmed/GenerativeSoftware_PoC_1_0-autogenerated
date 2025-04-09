using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class StudentGradeManager : IGeneratedModule
{
    public string Name { get; set; } = "Student Grade Manager";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Student Grade Manager module is running.");
        
        _dataFilePath = Path.Combine(dataFolder, "student_grades.json");
        
        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Add Course Grade");
            Console.WriteLine("3. Calculate GPA");
            Console.WriteLine("4. List All Students");
            Console.WriteLine("5. Exit Module");
            Console.Write("Select an option: ");
            
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }
            
            switch (input.Trim())
            {
                case "1":
                    AddStudent();
                    break;
                case "2":
                    AddCourseGrade();
                    break;
                case "3":
                    CalculateGPA();
                    break;
                case "4":
                    ListAllStudents();
                    break;
                case "5":
                    Console.WriteLine("Exiting Student Grade Manager module.");
                    return true;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
    
    private List<Student> LoadStudents()
    {
        if (!File.Exists(_dataFilePath))
        {
            return new List<Student>();
        }
        
        var json = File.ReadAllText(_dataFilePath);
        return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
    }
    
    private void SaveStudents(List<Student> students)
    {
        var json = JsonSerializer.Serialize(students);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddStudent()
    {
        Console.Write("Enter student ID: ");
        var id = Console.ReadLine();
        
        Console.Write("Enter student name: ");
        var name = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Invalid input. Student not added.");
            return;
        }
        
        var students = LoadStudents();
        
        if (students.Exists(s => s.Id == id))
        {
            Console.WriteLine("Student with this ID already exists.");
            return;
        }
        
        students.Add(new Student { Id = id, Name = name, Courses = new List<CourseGrade>() });
        SaveStudents(students);
        
        Console.WriteLine("Student added successfully.");
    }
    
    private void AddCourseGrade()
    {
        var students = LoadStudents();
        
        if (students.Count == 0)
        {
            Console.WriteLine("No students available. Please add a student first.");
            return;
        }
        
        Console.WriteLine("Select a student:");
        for (int i = 0; i < students.Count; i++)
        {
            Console.WriteLine(string.Format("{0}. {1} (ID: {2})", i + 1, students[i].Name, students[i].Id));
        }
        
        if (!int.TryParse(Console.ReadLine(), out int studentIndex) || studentIndex < 1 || studentIndex > students.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }
        
        var student = students[studentIndex - 1];
        
        Console.Write("Enter course name: ");
        var courseName = Console.ReadLine();
        
        Console.Write("Enter credit hours: ");
        if (!int.TryParse(Console.ReadLine(), out int creditHours) || creditHours <= 0)
        {
            Console.WriteLine("Invalid credit hours. Must be a positive integer.");
            return;
        }
        
        Console.Write("Enter grade (A, B, C, D, F): ");
        var gradeInput = Console.ReadLine()?.ToUpper();
        
        if (string.IsNullOrWhiteSpace(gradeInput) || !Enum.TryParse(gradeInput, out Grade grade))
        {
            Console.WriteLine("Invalid grade. Must be A, B, C, D, or F.");
            return;
        }
        
        student.Courses.Add(new CourseGrade 
        { 
            CourseName = courseName, 
            CreditHours = creditHours, 
            Grade = grade 
        });
        
        SaveStudents(students);
        Console.WriteLine("Course grade added successfully.");
    }
    
    private void CalculateGPA()
    {
        var students = LoadStudents();
        
        if (students.Count == 0)
        {
            Console.WriteLine("No students available.");
            return;
        }
        
        Console.WriteLine("Select a student:");
        for (int i = 0; i < students.Count; i++)
        {
            Console.WriteLine(string.Format("{0}. {1} (ID: {2})", i + 1, students[i].Name, students[i].Id));
        }
        
        if (!int.TryParse(Console.ReadLine(), out int studentIndex) || studentIndex < 1 || studentIndex > students.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }
        
        var student = students[studentIndex - 1];
        
        if (student.Courses.Count == 0)
        {
            Console.WriteLine("No courses available for this student.");
            return;
        }
        
        double totalQualityPoints = 0;
        int totalCreditHours = 0;
        
        foreach (var course in student.Courses)
        {
            double gradePoints = course.Grade switch
            {
                Grade.A => 4.0,
                Grade.B => 3.0,
                Grade.C => 2.0,
                Grade.D => 1.0,
                Grade.F => 0.0,
                _ => 0.0
            };
            
            totalQualityPoints += gradePoints * course.CreditHours;
            totalCreditHours += course.CreditHours;
        }
        
        double gpa = totalQualityPoints / totalCreditHours;
        Console.WriteLine(string.Format("GPA for {0}: {1:F2}", student.Name, gpa));
    }
    
    private void ListAllStudents()
    {
        var students = LoadStudents();
        
        if (students.Count == 0)
        {
            Console.WriteLine("No students available.");
            return;
        }
        
        Console.WriteLine("\nList of Students:");
        foreach (var student in students)
        {
            Console.WriteLine(string.Format("ID: {0}, Name: {1}", student.Id, student.Name));
            
            if (student.Courses.Count > 0)
            {
                Console.WriteLine("  Courses:");
                foreach (var course in student.Courses)
                {
                    Console.WriteLine(string.Format("    {0}: {1} (Credit Hours: {2})", 
                        course.CourseName, course.Grade, course.CreditHours));
                }
            }
            else
            {
                Console.WriteLine("  No courses registered.");
            }
        }
    }
}

public class Student
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<CourseGrade> Courses { get; set; }
}

public class CourseGrade
{
    public string CourseName { get; set; }
    public int CreditHours { get; set; }
    public Grade Grade { get; set; }
}

public enum Grade
{
    A, B, C, D, F
}