using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PoemAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Poem and Short Story Analyzer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Poem and Short Story Analyzer...");

        try
        {
            string inputFile = Path.Combine(dataFolder, "input.txt");
            string outputFile = Path.Combine(dataFolder, "analysis_results.json");

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Error: Input file not found. Please ensure 'input.txt' exists in the data folder.");
                return false;
            }

            string text = File.ReadAllText(inputFile);
            var analysis = AnalyzeText(text);

            string jsonResult = JsonSerializer.Serialize(analysis, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputFile, jsonResult);

            Console.WriteLine("Analysis completed successfully. Results saved to 'analysis_results.json'.");
            Console.WriteLine("Summary:");
            Console.WriteLine("Total Lines: " + analysis.LineCount);
            Console.WriteLine("Total Words: " + analysis.WordCount);
            Console.WriteLine("Unique Words: " + analysis.UniqueWordCount);
            Console.WriteLine("Average Word Length: " + analysis.AverageWordLength.ToString("0.00"));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during analysis: " + ex.Message);
            return false;
        }
    }

    private AnalysisResult AnalyzeText(string text)
    {
        var result = new AnalysisResult();
        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        result.LineCount = lines.Length;


        var wordCounts = new Dictionary<string, int>();
        int totalWordLength = 0;

        foreach (var line in lines)
        {
            var words = line.Split(new[] { ' ', '\t', ',', '.', '!', '?', ';', ':', '(', ')', '[', ']', '{', '}', '"', '\'' }, 
                                StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                string normalizedWord = word.ToLowerInvariant();
                if (wordCounts.ContainsKey(normalizedWord))
                {
                    wordCounts[normalizedWord]++;
                }
                else
                {
                    wordCounts[normalizedWord] = 1;
                }
                totalWordLength += word.Length;
                result.WordCount++;
            }
        }

        result.UniqueWordCount = wordCounts.Count;
        result.AverageWordLength = result.WordCount > 0 ? (double)totalWordLength / result.WordCount : 0;
        result.WordFrequency = wordCounts;

        return result;
    }
}

public class AnalysisResult
{
    public int LineCount { get; set; }
    public int WordCount { get; set; }
    public int UniqueWordCount { get; set; }
    public double AverageWordLength { get; set; }
    public Dictionary<string, int> WordFrequency { get; set; } = new Dictionary<string, int>();
}