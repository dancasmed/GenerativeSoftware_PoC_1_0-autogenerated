using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class WordDistributionAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Word Distribution Analyzer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Word Distribution Analyzer module is running.");
        Console.WriteLine("This module analyzes the distribution of words in a document.");

        string inputFilePath = Path.Combine(dataFolder, "input.txt");
        string outputFilePath = Path.Combine(dataFolder, "word_distribution.json");

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Error: Input file 'input.txt' not found in the data folder.");
            return false;
        }

        try
        {
            string text = File.ReadAllText(inputFilePath);
            var wordCounts = AnalyzeWordDistribution(text);
            SaveWordDistribution(wordCounts, outputFilePath);
            Console.WriteLine("Word distribution analysis completed successfully.");
            Console.WriteLine("Results saved to 'word_distribution.json'.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during word distribution analysis: " + ex.Message);
            return false;
        }
    }

    private Dictionary<string, int> AnalyzeWordDistribution(string text)
    {
        var separators = new char[] { ' ', '\t', '\n', '\r', '.', ',', ';', '!', '?', '(', ')', '[', ']', '{', '}', '"', '\'', ':' };
        var words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var word in words)
        {
            string cleanedWord = word.Trim().ToLower();
            if (wordCounts.ContainsKey(cleanedWord))
            {
                wordCounts[cleanedWord]++;
            }
            else
            {
                wordCounts[cleanedWord] = 1;
            }
        }

        return wordCounts;
    }

    private void SaveWordDistribution(Dictionary<string, int> wordCounts, string outputFilePath)
    {
        var sortedWordCounts = wordCounts.OrderByDescending(pair => pair.Value)
                                         .ThenBy(pair => pair.Key)
                                         .ToDictionary(pair => pair.Key, pair => pair.Value);

        var jsonLines = sortedWordCounts.Select(pair => string.Format("\"{0}\": {1}", pair.Key, pair.Value));
        string jsonContent = "{\n  " + string.Join(",\n  ", jsonLines) + "\n}";

        File.WriteAllText(outputFilePath, jsonContent);
    }
}