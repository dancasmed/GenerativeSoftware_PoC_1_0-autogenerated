using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RandomNamePicker : IGeneratedModule
{
    public string Name { get; set; } = "Random Name Picker";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Random Name Picker Module is running...");
        
        string participantsFile = Path.Combine(dataFolder, "participants.json");
        List<string> participants;
        
        try
        {
            if (!File.Exists(participantsFile))
            {
                participants = GenerateSampleParticipants();
                string json = JsonSerializer.Serialize(participants);
                File.WriteAllText(participantsFile, json);
                Console.WriteLine("Sample participants file created.");
            }
            else
            {
                string json = File.ReadAllText(participantsFile);
                participants = JsonSerializer.Deserialize<List<string>>(json);
            }
            
            if (participants == null || participants.Count == 0)
            {
                Console.WriteLine("No participants found.");
                return false;
            }
            
            Random random = new Random();
            int winnerIndex = random.Next(participants.Count);
            string winner = participants[winnerIndex];
            
            Console.WriteLine("The winner is: " + winner);
            
            string winnersFile = Path.Combine(dataFolder, "winners.json");
            List<string> winners = new List<string>();
            
            if (File.Exists(winnersFile))
            {
                string winnersJson = File.ReadAllText(winnersFile);
                winners = JsonSerializer.Deserialize<List<string>>(winnersJson);
            }
            
            winners.Add(winner);
            string updatedWinnersJson = JsonSerializer.Serialize(winners);
            File.WriteAllText(winnersFile, updatedWinnersJson);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private List<string> GenerateSampleParticipants()
    {
        return new List<string>
        {
            "Alice",
            "Bob",
            "Charlie",
            "Diana",
            "Eve",
            "Frank",
            "Grace",
            "Henry"
        };
    }
}