using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class JobPortalModule : IGeneratedModule
{
    public string Name { get; set; } = "Job Portal Simulator";

    private string resumesFilePath;
    private string applicationsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Job Portal Module is running...");

        resumesFilePath = Path.Combine(dataFolder, "resumes.json");
        applicationsFilePath = Path.Combine(dataFolder, "applications.json");

        InitializeFiles();

        bool exitRequested = false;
        while (!exitRequested)
        {
            DisplayMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddResume();
                    break;
                case "2":
                    ViewResumes();
                    break;
                case "3":
                    ApplyForJob();
                    break;
                case "4":
                    ViewApplications();
                    break;
                case "5":
                    exitRequested = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Job Portal Module finished.");
        return true;
    }

    private void InitializeFiles()
    {
        if (!File.Exists(resumesFilePath))
        {
            File.WriteAllText(resumesFilePath, "[]");
        }

        if (!File.Exists(applicationsFilePath))
        {
            File.WriteAllText(applicationsFilePath, "[]");
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nJob Portal Menu:");
        Console.WriteLine("1. Add Resume");
        Console.WriteLine("2. View Resumes");
        Console.WriteLine("3. Apply for Job");
        Console.WriteLine("4. View Applications");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice: ");
    }

    private void AddResume()
    {
        Console.Write("Enter your name: ");
        var name = Console.ReadLine();

        Console.Write("Enter your skills (comma separated): ");
        var skills = Console.ReadLine();

        Console.Write("Enter your experience (years): ");
        var experienceInput = Console.ReadLine();
        if (!int.TryParse(experienceInput, out int experience))
        {
            Console.WriteLine("Invalid experience value. Resume not added.");
            return;
        }

        var resumes = LoadResumes();
        resumes.Add(new Resume
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Skills = skills.Split(','),
            Experience = experience
        });

        SaveResumes(resumes);
        Console.WriteLine("Resume added successfully.");
    }

    private void ViewResumes()
    {
        var resumes = LoadResumes();

        if (resumes.Count == 0)
        {
            Console.WriteLine("No resumes found.");
            return;
        }

        Console.WriteLine("\nAvailable Resumes:");
        foreach (var resume in resumes)
        {
            Console.WriteLine("ID: " + resume.Id);
            Console.WriteLine("Name: " + resume.Name);
            Console.WriteLine("Skills: " + string.Join(", ", resume.Skills));
            Console.WriteLine("Experience: " + resume.Experience + " years");
            Console.WriteLine("--------------------");
        }
    }

    private void ApplyForJob()
    {
        var resumes = LoadResumes();
        if (resumes.Count == 0)
        {
            Console.WriteLine("No resumes available to apply with.");
            return;
        }

        ViewResumes();
        Console.Write("Enter resume ID to apply with: ");
        var resumeId = Console.ReadLine();

        Console.Write("Enter job title: ");
        var jobTitle = Console.ReadLine();

        Console.Write("Enter company name: ");
        var companyName = Console.ReadLine();

        var applications = LoadApplications();
        applications.Add(new JobApplication
        {
            Id = Guid.NewGuid().ToString(),
            ResumeId = resumeId,
            JobTitle = jobTitle,
            CompanyName = companyName,
            ApplicationDate = DateTime.Now
        });

        SaveApplications(applications);
        Console.WriteLine("Application submitted successfully.");
    }

    private void ViewApplications()
    {
        var applications = LoadApplications();
        var resumes = LoadResumes();

        if (applications.Count == 0)
        {
            Console.WriteLine("No applications found.");
            return;
        }

        Console.WriteLine("\nJob Applications:");
        foreach (var app in applications)
        {
            var resume = resumes.Find(r => r.Id == app.ResumeId);
            Console.WriteLine("Application ID: " + app.Id);
            Console.WriteLine("Job Title: " + app.JobTitle);
            Console.WriteLine("Company: " + app.CompanyName);
            Console.WriteLine("Applicant: " + (resume?.Name ?? "Unknown"));
            Console.WriteLine("Date: " + app.ApplicationDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("--------------------");
        }
    }

    private List<Resume> LoadResumes()
    {
        var json = File.ReadAllText(resumesFilePath);
        return JsonSerializer.Deserialize<List<Resume>>(json) ?? new List<Resume>();
    }

    private void SaveResumes(List<Resume> resumes)
    {
        var json = JsonSerializer.Serialize(resumes);
        File.WriteAllText(resumesFilePath, json);
    }

    private List<JobApplication> LoadApplications()
    {
        var json = File.ReadAllText(applicationsFilePath);
        return JsonSerializer.Deserialize<List<JobApplication>>(json) ?? new List<JobApplication>();
    }

    private void SaveApplications(List<JobApplication> applications)
    {
        var json = JsonSerializer.Serialize(applications);
        File.WriteAllText(applicationsFilePath, json);
    }
}

public class Resume
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string[] Skills { get; set; }
    public int Experience { get; set; }
}

public class JobApplication
{
    public string Id { get; set; }
    public string ResumeId { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public DateTime ApplicationDate { get; set; }
}