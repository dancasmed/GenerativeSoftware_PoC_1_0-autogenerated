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
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            Console.WriteLine("Enter the duration of your camping trip in days: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int duration) || duration <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number of days.");
                return false;
            }

            var checklist = GenerateChecklist(duration);
            string json = JsonSerializer.Serialize(checklist, new JsonSerializerOptions { WriteIndented = true });
            string filePath = Path.Combine(dataFolder, "camping_checklist.json");
            File.WriteAllText(filePath, json);

            Console.WriteLine("Checklist generated successfully and saved to " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private Dictionary<string, List<string>> GenerateChecklist(int duration)
    {
        var checklist = new Dictionary<string, List<string>>
        {
            {"Essentials", new List<string> {"Tent", "Sleeping bag", "Sleeping pad", "Headlamp", "First aid kit"}},
            {"Clothing", new List<string> {"Hiking boots", "Socks", "Underwear", "Jacket", "Hat"}},
            {"Food", new List<string> {"Water bottles", "Snacks", "Meals", "Utensils", "Cooking stove"}},
            {"Miscellaneous", new List<string> {"Map", "Compass", "Sunscreen", "Bug spray", "Multi-tool"}}
        };

        if (duration > 3)
        {
            checklist["Food"].Add("Extra meals");
            checklist["Clothing"].Add("Extra socks and underwear");
            checklist["Miscellaneous"].Add("Portable charger");
        }

        if (duration > 7)
        {
            checklist["Essentials"].Add("Extra tent stakes");
            checklist["Food"].Add("Bulk food items");
            checklist["Miscellaneous"].Add("Repair kit");
        }

        return checklist;
    }
}