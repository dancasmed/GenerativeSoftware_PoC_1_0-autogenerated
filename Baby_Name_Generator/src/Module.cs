using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BabyNameGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Baby Name Generator";
    
    private List<string> _maleNames = new List<string> { "Liam", "Noah", "Oliver", "Elijah", "James", "William", "Benjamin", "Lucas", "Henry", "Alexander" };
    private List<string> _femaleNames = new List<string> { "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Charlotte", "Mia", "Amelia", "Harper", "Evelyn" };
    private List<string> _unisexNames = new List<string> { "Taylor", "Jordan", "Alex", "Casey", "Jamie", "Morgan", "Riley", "Avery", "Peyton", "Quinn" };

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Baby Name Generator module is running.");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var namesDataPath = Path.Combine(dataFolder, "babynames_config.json");
            var filterOptions = LoadFilterOptions(namesDataPath);
            
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions
                {
                    Gender = "all",
                    StartsWith = "",
                    EndsWith = "",
                    Contains = "",
                    MinLength = 3,
                    MaxLength = 10
                };
                SaveFilterOptions(namesDataPath, filterOptions);
            }

            var filteredNames = GenerateFilteredNames(filterOptions);
            
            var outputPath = Path.Combine(dataFolder, "baby_names_list.txt");
            File.WriteAllLines(outputPath, filteredNames);
            
            Console.WriteLine("Generated " + filteredNames.Count + " baby names and saved to " + outputPath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private FilterOptions LoadFilterOptions(string filePath)
    {
        if (!File.Exists(filePath))
            return null;
            
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<FilterOptions>(json);
    }

    private void SaveFilterOptions(string filePath, FilterOptions options)
    {
        var json = JsonSerializer.Serialize(options);
        File.WriteAllText(filePath, json);
    }

    private List<string> GenerateFilteredNames(FilterOptions options)
    {
        var allNames = new List<string>();
        
        if (options.Gender == "male" || options.Gender == "all")
            allNames.AddRange(_maleNames);
            
        if (options.Gender == "female" || options.Gender == "all")
            allNames.AddRange(_femaleNames);
            
        if (options.Gender == "unisex" || options.Gender == "all")
            allNames.AddRange(_unisexNames);
        
        var filteredNames = new List<string>();
        
        foreach (var name in allNames)
        {
            if (name.Length < options.MinLength || name.Length > options.MaxLength)
                continue;
                
            if (!string.IsNullOrEmpty(options.StartsWith) && !name.StartsWith(options.StartsWith, StringComparison.OrdinalIgnoreCase))
                continue;
                
            if (!string.IsNullOrEmpty(options.EndsWith) && !name.EndsWith(options.EndsWith, StringComparison.OrdinalIgnoreCase))
                continue;
                
            if (!string.IsNullOrEmpty(options.Contains) && !name.Contains(options.Contains, StringComparison.OrdinalIgnoreCase))
                continue;
                
            filteredNames.Add(name);
        }
        
        return filteredNames;
    }

    private class FilterOptions
    {
        public string Gender { get; set; } // "male", "female", "unisex", "all"
        public string StartsWith { get; set; }
        public string EndsWith { get; set; }
        public string Contains { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
    }
}