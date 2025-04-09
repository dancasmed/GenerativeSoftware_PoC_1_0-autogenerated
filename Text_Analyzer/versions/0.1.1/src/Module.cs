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
        Console.WriteLine("Text Analyzer module is running...");
        Console.WriteLine("Enter the text to analyze (press Enter twice to finish):");

        string inputText = ReadMultilineInput();
        if (string.IsNullOrWhiteSpace(inputText))
        {
            Console.WriteLine("No text provided for analysis.");
            return false;
        }

        var wordFrequencies = CountWordFrequencies(inputText);
        string outputPath = Path.Combine(dataFolder, "word_frequencies.json");
        SaveFrequenciesToJson(wordFrequencies, outputPath);

        Console.WriteLine("Analysis complete. Results saved to " + outputPath);
        return true;
    }

    private string ReadMultilineInput()
    {
        List<string> lines = new List<string>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            lines.Add(line);
        }
        return string.Join(" ", lines);
    }

    private Dictionary<string, int> CountWordFrequencies(string text)
    {
        var frequencies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        char[] separators = new[] { ' ', '\t', '\n', '\r', '.', ',', ';', '!', '?', '(', ')', '[', ']', '{', '}', '"', '\'' };
        
        foreach (var word in text.Split(separators, StringSplitOptions.RemoveEmptyEntries))
        {
            string cleanedWord = word.Trim().ToLower();
            if (frequencies.ContainsKey(cleanedWord))
            {
                frequencies[cleanedWord]++;
            }
            else
            {
                frequencies[cleanedWord] = 1;
            }
        }
        
        return frequencies;
    }

    private void SaveFrequenciesToJson(Dictionary<string, int> frequencies, string filePath)
    {
        try
        {
            string json = "{\n";
            foreach (var pair in frequencies.OrderByDescending(p => p.Value))
            {
                json += string.Format("  \"{0}\": {1},\n", pair.Key, pair.Value);
            }
            json = json.TrimEnd(',', '\n') + "\n}";
            
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving results: " + ex.Message);
        }
    }
}