using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DecisionAssistant : IGeneratedModule
{
    public string Name { get; set; } = "Decision Assistant";
    private List<string> _decisionHistory = new List<string>();
    private const string DecisionHistoryFile = "decision_history.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Decision Assistant module is running.");
        Console.WriteLine("Type 'exit' to quit or ask for a decision.");

        LoadDecisionHistory(dataFolder);

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();

            if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
            {
                SaveDecisionHistory(dataFolder);
                return true;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a valid question or type 'exit' to quit.");
                continue;
            }

            string decision = GenerateDecision(input);
            _decisionHistory.Add($"Question: {input} - Decision: {decision}");

            Console.WriteLine(decision);
        }
    }

    private string GenerateDecision(string question)
    {
        string[] possibleDecisions = {
            "Yes, you should proceed.",
            "No, it's better to avoid that.",
            "Maybe, consider more information.",
            "Definitely! Go for it.",
            "Not a good idea at this time.",
            "The answer is unclear. Try again later."
        };

        int index = Math.Abs(question.GetHashCode()) % possibleDecisions.Length;
        return possibleDecisions[index];
    }

    private void LoadDecisionHistory(string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, DecisionHistoryFile);
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                _decisionHistory = JsonSerializer.Deserialize<List<string>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading decision history: {ex.Message}");
            }
        }
    }

    private void SaveDecisionHistory(string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, DecisionHistoryFile);
        try
        {
            string json = JsonSerializer.Serialize(_decisionHistory);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving decision history: {ex.Message}");
        }
    }
}