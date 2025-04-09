using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LearningScheduleGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Personalized Learning Schedule Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Personalized Learning Schedule Generator...");

        try
        {
            string scheduleFilePath = Path.Combine(dataFolder, "learning_schedule.json");
            
            Console.WriteLine("Please enter your name:");
            string userName = Console.ReadLine();
            
            Console.WriteLine("How many subjects do you want to study?");
            int subjectCount = int.Parse(Console.ReadLine());
            
            List<string> subjects = new List<string>();
            for (int i = 0; i < subjectCount; i++)
            {
                Console.WriteLine("Enter subject name " + (i + 1) + ":");
                subjects.Add(Console.ReadLine());
            }
            
            Console.WriteLine("How many days per week can you study?");
            int studyDays = int.Parse(Console.ReadLine());
            
            Console.WriteLine("How many hours per day can you dedicate?");
            int studyHours = int.Parse(Console.ReadLine());
            
            var schedule = GenerateSchedule(userName, subjects, studyDays, studyHours);
            
            string json = JsonSerializer.Serialize(schedule, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(scheduleFilePath, json);
            
            Console.WriteLine("Learning schedule generated successfully!");
            Console.WriteLine("File saved to: " + scheduleFilePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating learning schedule: " + ex.Message);
            return false;
        }
    }
    
    private LearningSchedule GenerateSchedule(string userName, List<string> subjects, int studyDays, int studyHours)
    {
        var schedule = new LearningSchedule
        {
            UserName = userName,
            GeneratedDate = DateTime.Now,
            StudyDays = studyDays,
            DailyHours = studyHours,
            WeeklySubjects = new List<SubjectSchedule>()
        };
        
        int totalStudySlots = studyDays * studyHours;
        int slotsPerSubject = totalStudySlots / subjects.Count;
        int remainingSlots = totalStudySlots % subjects.Count;
        
        var random = new Random();
        
        foreach (var subject in subjects)
        {
            int subjectSlots = slotsPerSubject;
            if (remainingSlots > 0)
            {
                subjectSlots++;
                remainingSlots--;
            }
            
            var subjectSchedule = new SubjectSchedule
            {
                SubjectName = subject,
                WeeklySlots = subjectSlots,
                RecommendedDuration = studyHours > 2 ? "1-2 hour sessions" : "30-60 minute sessions"
            };
            
            schedule.WeeklySubjects.Add(subjectSchedule);
        }
        
        return schedule;
    }
}

public class LearningSchedule
{
    public string UserName { get; set; }
    public DateTime GeneratedDate { get; set; }
    public int StudyDays { get; set; }
    public int DailyHours { get; set; }
    public List<SubjectSchedule> WeeklySubjects { get; set; }
}

public class SubjectSchedule
{
    public string SubjectName { get; set; }
    public int WeeklySlots { get; set; }
    public string RecommendedDuration { get; set; }
}