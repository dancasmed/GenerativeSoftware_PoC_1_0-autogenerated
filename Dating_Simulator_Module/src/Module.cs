using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DatingSimulatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Dating Simulator Module";

    private List<Profile> profiles;
    private string profilesFilePath;
    private string matchesFilePath;

    public DatingSimulatorModule()
    {
        profiles = new List<Profile>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Dating Simulator Module is running...");
        
        profilesFilePath = Path.Combine(dataFolder, "profiles.json");
        matchesFilePath = Path.Combine(dataFolder, "matches.json");

        LoadProfiles();
        
        if (profiles.Count < 2)
        {
            GenerateSampleProfiles();
            SaveProfiles();
        }

        var matches = FindMatches();
        SaveMatches(matches);

        DisplayResults(matches);

        return true;
    }

    private void LoadProfiles()
    {
        if (File.Exists(profilesFilePath))
        {
            try
            {
                string json = File.ReadAllText(profilesFilePath);
                profiles = JsonSerializer.Deserialize<List<Profile>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading profiles: " + ex.Message);
            }
        }
    }

    private void SaveProfiles()
    {
        try
        {
            string json = JsonSerializer.Serialize(profiles);
            File.WriteAllText(profilesFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving profiles: " + ex.Message);
        }
    }

    private void GenerateSampleProfiles()
    {
        profiles = new List<Profile>
        {
            new Profile
            {
                Id = 1,
                Name = "Alex",
                Age = 28,
                Gender = "Male",
                Interests = new List<string> { "Hiking", "Reading", "Cooking" },
                Bio = "Outdoorsy guy who loves to cook"
            },
            new Profile
            {
                Id = 2,
                Name = "Sam",
                Age = 25,
                Gender = "Female",
                Interests = new List<string> { "Reading", "Traveling", "Photography" },
                Bio = "Book lover who enjoys exploring new places"
            },
            new Profile
            {
                Id = 3,
                Name = "Jordan",
                Age = 30,
                Gender = "Non-binary",
                Interests = new List<string> { "Music", "Cooking", "Art" },
                Bio = "Creative soul who loves to make things"
            }
        };
    }

    private List<Match> FindMatches()
    {
        var matches = new List<Match>();
        
        for (int i = 0; i < profiles.Count; i++)
        {
            for (int j = i + 1; j < profiles.Count; j++)
            {
                var profile1 = profiles[i];
                var profile2 = profiles[j];
                
                int compatibilityScore = CalculateCompatibility(profile1, profile2);
                
                if (compatibilityScore >= 2) // At least 2 common interests
                {
                    matches.Add(new Match
                    {
                        Profile1Id = profile1.Id,
                        Profile2Id = profile2.Id,
                        CompatibilityScore = compatibilityScore,
                        Timestamp = DateTime.Now
                    });
                }
            }
        }
        
        return matches;
    }

    private int CalculateCompatibility(Profile profile1, Profile profile2)
    {
        int commonInterests = 0;
        
        foreach (var interest in profile1.Interests)
        {
            if (profile2.Interests.Contains(interest))
            {
                commonInterests++;
            }
        }
        
        return commonInterests;
    }

    private void SaveMatches(List<Match> matches)
    {
        try
        {
            string json = JsonSerializer.Serialize(matches);
            File.WriteAllText(matchesFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving matches: " + ex.Message);
        }
    }

    private void DisplayResults(List<Match> matches)
    {
        Console.WriteLine("\n=== Dating Simulator Results ===");
        Console.WriteLine("Profiles count: " + profiles.Count);
        Console.WriteLine("Matches found: " + matches.Count);
        
        if (matches.Count > 0)
        {
            Console.WriteLine("\nTop matches:");
            
            // Sort by compatibility score
            matches.Sort((a, b) => b.CompatibilityScore.CompareTo(a.CompatibilityScore));
            
            for (int i = 0; i < Math.Min(3, matches.Count); i++)
            {
                var match = matches[i];
                var profile1 = profiles.Find(p => p.Id == match.Profile1Id);
                var profile2 = profiles.Find(p => p.Id == match.Profile2Id);
                
                Console.WriteLine(string.Format("Match {0}: {1} and {2} (Compatibility: {3} common interests)",
                    i + 1, profile1.Name, profile2.Name, match.CompatibilityScore));
            }
        }
    }
}

public class Profile
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public List<string> Interests { get; set; }
    public string Bio { get; set; }
}

public class Match
{
    public int Profile1Id { get; set; }
    public int Profile2Id { get; set; }
    public int CompatibilityScore { get; set; }
    public DateTime Timestamp { get; set; }
}