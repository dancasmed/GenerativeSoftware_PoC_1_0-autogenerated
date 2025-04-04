using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CampingChecklistGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Camping Checklist Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Camping Checklist Generator is running...");
        Console.WriteLine("Generating a checklist for your camping trip.");

        try
        {
            int duration = GetCampingDuration();
            List<string> checklist = GenerateChecklist(duration);
            SaveChecklist(dataFolder, checklist, duration);
            Console.WriteLine("Checklist generated successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private int GetCampingDuration()
    {
        Console.WriteLine("Enter the duration of your camping trip in days:");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int duration) && duration > 0)
        {
            return duration;
        }
        else
        {
            Console.WriteLine("Invalid input. Using default duration of 2 days.");
            return 2;
        }
    }

    private List<string> GenerateChecklist(int duration)
    {
        List<string> checklist = new List<string>
        {
            "Tent",
            "Sleeping bag",
            "Sleeping pad",
            "Headlamp or flashlight",
            "First aid kit",
            "Map and compass or GPS",
            "Multi-tool or knife",
            "Fire starter (matches, lighter, fire steel)",
            "Stove and fuel",
            "Cooking utensils",
            "Food and snacks",
            "Water bottles or hydration system",
            "Water filter or purification tablets",
            "Trash bags",
            "Toilet paper and trowel"
        };

        if (duration > 3)
        {
            checklist.Add("Extra clothing layers");
            checklist.Add("Additional food supplies");
            checklist.Add("Portable water container");
        }

        if (duration > 7)
        {
            checklist.Add("Solar charger or extra batteries");
            checklist.Add("Repair kit for gear");
            checklist.Add("Bear canister or food storage");
        }

        return checklist;
    }

    private void SaveChecklist(string dataFolder, List<string> checklist, int duration)
    {
        string fileName = Path.Combine(dataFolder, $"CampingChecklist_{duration}Days.json");
        string json = JsonSerializer.Serialize(checklist, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
        Console.WriteLine("Checklist saved to: " + fileName);
    }
}