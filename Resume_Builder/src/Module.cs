using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ResumeBuilder : IGeneratedModule
{
    public string Name { get; set; } = "Resume Builder";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Resume Builder Module...");
        
        try
        {
            var resume = new Resume();
            
            Console.WriteLine("Enter your full name:");
            resume.FullName = Console.ReadLine();
            
            Console.WriteLine("Enter your email address:");
            resume.Email = Console.ReadLine();
            
            Console.WriteLine("Enter your phone number:");
            resume.Phone = Console.ReadLine();
            
            Console.WriteLine("Enter your professional summary (press Enter twice to finish):");
            resume.Summary = ReadMultiLineInput();
            
            bool addMore = true;
            while (addMore)
            {
                Console.WriteLine("Add an experience? (Y/N)");
                if (Console.ReadLine().Trim().ToUpper() != "Y")
                {
                    addMore = false;
                    continue;
                }
                
                var experience = new Experience();
                Console.WriteLine("Enter job title:");
                experience.JobTitle = Console.ReadLine();
                
                Console.WriteLine("Enter company name:");
                experience.Company = Console.ReadLine();
                
                Console.WriteLine("Enter start date (MM/YYYY):");
                experience.StartDate = Console.ReadLine();
                
                Console.WriteLine("Enter end date (MM/YYYY or 'Present'):");
                experience.EndDate = Console.ReadLine();
                
                Console.WriteLine("Enter job description (press Enter twice to finish):");
                experience.Description = ReadMultiLineInput();
                
                resume.Experiences.Add(experience);
            }
            
            addMore = true;
            while (addMore)
            {
                Console.WriteLine("Add an education entry? (Y/N)");
                if (Console.ReadLine().Trim().ToUpper() != "Y")
                {
                    addMore = false;
                    continue;
                }
                
                var education = new Education();
                Console.WriteLine("Enter degree:");
                education.Degree = Console.ReadLine();
                
                Console.WriteLine("Enter institution:");
                education.Institution = Console.ReadLine();
                
                Console.WriteLine("Enter graduation year:");
                education.Year = Console.ReadLine();
                
                resume.Educations.Add(education);
            }
            
            addMore = true;
            while (addMore)
            {
                Console.WriteLine("Add a skill? (Y/N)");
                if (Console.ReadLine().Trim().ToUpper() != "Y")
                {
                    addMore = false;
                    continue;
                }
                
                Console.WriteLine("Enter skill name:");
                resume.Skills.Add(Console.ReadLine());
            }
            
            string filePath = Path.Combine(dataFolder, $"{resume.FullName.Replace(" ", "_")}_resume.json");
            string json = JsonSerializer.Serialize(resume, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            
            Console.WriteLine("Resume saved successfully!");
            Console.WriteLine("File location: " + filePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while building the resume: " + ex.Message);
            return false;
        }
    }
    
    private string ReadMultiLineInput()
    {
        string input = "";
        string line;
        while ((line = Console.ReadLine()) != "")
        {
            input += line + "\n";
        }
        return input.Trim();
    }
}

public class Resume
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<Experience> Experiences { get; set; } = new List<Experience>();
    public List<Education> Educations { get; set; } = new List<Education>();
    public List<string> Skills { get; set; } = new List<string>();
}

public class Experience
{
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Education
{
    public string Degree { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
}