using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TextAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Text Analyzer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Text Analyzer module is running.");
        Console.WriteLine("Enter the text to analyze:");
        string inputText = Console.ReadLine();

        if (string.IsNullOrEmpty(inputText))
        {
            Console.WriteLine("No text provided. Exiting.");
            return false;
        }

        var vowelCounts = new Dictionary<char, int>
        {
            { 'a', 0 },
            { 'e', 0 },
            { 'i', 0 },
            { 'o', 0 },
            { 'u', 0 }
        };

        int consonantCount = 0;
        int totalLetters = 0;

        foreach (char c in inputText.ToLower())
        {
            if (char.IsLetter(c))
            {
                totalLetters++;
                if (vowelCounts.ContainsKey(c))
                {
                    vowelCounts[c]++;
                }
                else
                {
                    consonantCount++;
                }
            }
        }

        Console.WriteLine("Analysis Results:");
        Console.WriteLine("----------------");
        Console.WriteLine("Vowel Counts:");
        foreach (var kvp in vowelCounts)
        {
            Console.WriteLine(kvp.Key + ": " + kvp.Value);
        }
        Console.WriteLine("Consonant Count: " + consonantCount);
        Console.WriteLine("Total Letters: " + totalLetters);

        var result = new
        {
            VowelCounts = vowelCounts,
            ConsonantCount = consonantCount,
            TotalLetters = totalLetters,
            InputText = inputText
        };

        string resultFilePath = Path.Combine(dataFolder, "text_analysis_results.json");
        File.WriteAllText(resultFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
        Console.WriteLine("Results saved to: " + resultFilePath);

        return true;
    }
}