using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FitnessCenterModule : IGeneratedModule
{
    public string Name { get; set; } = "Fitness Center Management System";

    private string membersFilePath;
    private string classesFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Fitness Center Management System...");

        membersFilePath = Path.Combine(dataFolder, "members.json");
        classesFilePath = Path.Combine(dataFolder, "classes.json");

        InitializeDataFiles();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddMember();
                    break;
                case "2":
                    ViewMembers();
                    break;
                case "3":
                    AddClass();
                    break;
                case "4":
                    ViewClasses();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Fitness Center Management System is shutting down...");
        return true;
    }

    private void InitializeDataFiles()
    {
        if (!Directory.Exists(Path.GetDirectoryName(membersFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(membersFilePath));
        }

        if (!File.Exists(membersFilePath))
        {
            File.WriteAllText(membersFilePath, "[]");
        }

        if (!File.Exists(classesFilePath))
        {
            File.WriteAllText(classesFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nFitness Center Management System");
        Console.WriteLine("1. Add Member");
        Console.WriteLine("2. View Members");
        Console.WriteLine("3. Add Class");
        Console.WriteLine("4. View Classes");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void AddMember()
    {
        Console.Write("Enter member name: ");
        var name = Console.ReadLine();

        Console.Write("Enter member email: ");
        var email = Console.ReadLine();

        Console.Write("Enter membership type (Basic/Premium): ");
        var membershipType = Console.ReadLine();

        var members = LoadMembers();
        members.Add(new Member
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            MembershipType = membershipType,
            JoinDate = DateTime.Now
        });

        SaveMembers(members);
        Console.WriteLine("Member added successfully.");
    }

    private void ViewMembers()
    {
        var members = LoadMembers();

        Console.WriteLine("\nMembers List:");
        foreach (var member in members)
        {
            Console.WriteLine("ID: " + member.Id);
            Console.WriteLine("Name: " + member.Name);
            Console.WriteLine("Email: " + member.Email);
            Console.WriteLine("Membership: " + member.MembershipType);
            Console.WriteLine("Join Date: " + member.JoinDate);
            Console.WriteLine();
        }
    }

    private void AddClass()
    {
        Console.Write("Enter class name: ");
        var className = Console.ReadLine();

        Console.Write("Enter instructor name: ");
        var instructor = Console.ReadLine();

        Console.Write("Enter schedule (e.g., Mon/Wed 5-6pm): ");
        var schedule = Console.ReadLine();

        Console.Write("Enter maximum capacity: ");
        var capacity = int.Parse(Console.ReadLine());

        var classes = LoadClasses();
        classes.Add(new FitnessClass
        {
            Id = Guid.NewGuid(),
            ClassName = className,
            Instructor = instructor,
            Schedule = schedule,
            Capacity = capacity,
            CurrentEnrollment = 0
        });

        SaveClasses(classes);
        Console.WriteLine("Class added successfully.");
    }

    private void ViewClasses()
    {
        var classes = LoadClasses();

        Console.WriteLine("\nClass Schedule:");
        foreach (var fitnessClass in classes)
        {
            Console.WriteLine("ID: " + fitnessClass.Id);
            Console.WriteLine("Class: " + fitnessClass.ClassName);
            Console.WriteLine("Instructor: " + fitnessClass.Instructor);
            Console.WriteLine("Schedule: " + fitnessClass.Schedule);
            Console.WriteLine("Capacity: " + fitnessClass.Capacity + " (" + fitnessClass.CurrentEnrollment + " enrolled)");
            Console.WriteLine();
        }
    }

    private List<Member> LoadMembers()
    {
        var json = File.ReadAllText(membersFilePath);
        return JsonSerializer.Deserialize<List<Member>>(json) ?? new List<Member>();
    }

    private void SaveMembers(List<Member> members)
    {
        var json = JsonSerializer.Serialize(members);
        File.WriteAllText(membersFilePath, json);
    }

    private List<FitnessClass> LoadClasses()
    {
        var json = File.ReadAllText(classesFilePath);
        return JsonSerializer.Deserialize<List<FitnessClass>>(json) ?? new List<FitnessClass>();
    }

    private void SaveClasses(List<FitnessClass> classes)
    {
        var json = JsonSerializer.Serialize(classes);
        File.WriteAllText(classesFilePath, json);
    }
}

public class Member
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string MembershipType { get; set; }
    public DateTime JoinDate { get; set; }
}

public class FitnessClass
{
    public Guid Id { get; set; }
    public string ClassName { get; set; }
    public string Instructor { get; set; }
    public string Schedule { get; set; }
    public int Capacity { get; set; }
    public int CurrentEnrollment { get; set; }
}