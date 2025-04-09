using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class NovelStructureAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Novel Structure Analyzer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Novel Structure Analyzer module is running...");
        
        try
        {
            string inputFile = Path.Combine(dataFolder, "novel_text.txt");
            string outputFile = Path.Combine(dataFolder, "analysis_results.json");

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Error: Input file 'novel_text.txt' not found in data folder.");
                return false;
            }

            string novelText = File.ReadAllText(inputFile);
            var analysis = AnalyzeNovelStructure(novelText);

            string jsonResult = JsonSerializer.Serialize(analysis, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputFile, jsonResult);

            Console.WriteLine("Analysis completed successfully. Results saved to 'analysis_results.json'.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during analysis: " + ex.Message);
            return false;
        }
    }

    private NovelAnalysis AnalyzeNovelStructure(string text)
    {
        var analysis = new NovelAnalysis();
        
        // Basic statistics
        analysis.TotalCharacters = text.Length;
        analysis.TotalWords = CountWords(text);
        analysis.TotalParagraphs = CountParagraphs(text);
        
        // Dialogue analysis
        analysis.DialogueCount = CountDialogues(text);
        analysis.DialoguePercentage = (double)analysis.DialogueCount / analysis.TotalParagraphs * 100;
        
        // Chapter detection
        analysis.Chapters = DetectChapters(text);
        
        // Sentence analysis
        var sentences = SplitSentences(text);
        analysis.AverageSentenceLength = sentences.Count > 0 ? 
            sentences.Average(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length) : 0;
        
        return analysis;
    }

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;
            
        return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private int CountParagraphs(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;
            
        return text.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private int CountDialogues(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;
            
        int count = 0;
        bool inQuotes = false;
        
        foreach (char c in text)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                if (!inQuotes) count++;
            }
        }
        
        return count;
    }

    private List<string> DetectChapters(string text)
    {
        var chapters = new List<string>();
        var lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            if (line.Trim().StartsWith("Chapter ", StringComparison.OrdinalIgnoreCase) ||
                line.Trim().StartsWith("CHAPTER ", StringComparison.OrdinalIgnoreCase))
            {
                chapters.Add(line.Trim());
            }
        }
        
        return chapters;
    }

    private List<string> SplitSentences(string text)
    {
        var sentences = new List<string>();
        if (string.IsNullOrWhiteSpace(text))
            return sentences;
            
        int start = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '.' || text[i] == '!' || text[i] == '?')
            {
                if (i + 1 >= text.Length || char.IsWhiteSpace(text[i + 1]) || char.IsUpper(text[i + 1]))
                {
                    string sentence = text.Substring(start, i - start + 1).Trim();
                    if (!string.IsNullOrEmpty(sentence))
                        sentences.Add(sentence);
                    start = i + 1;
                }
            }
        }
        
        // Add remaining text if any
        if (start < text.Length)
        {
            string remaining = text.Substring(start).Trim();
            if (!string.IsNullOrEmpty(remaining))
                sentences.Add(remaining);
        }
        
        return sentences;
    }
}

public class NovelAnalysis
{
    public int TotalCharacters { get; set; }
    public int TotalWords { get; set; }
    public int TotalParagraphs { get; set; }
    public int DialogueCount { get; set; }
    public double DialoguePercentage { get; set; }
    public List<string> Chapters { get; set; } = new List<string>();
    public double AverageSentenceLength { get; set; }
}