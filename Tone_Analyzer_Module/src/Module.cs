using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ToneAnalyzerModule : IGeneratedModule
{
    public string Name { get; set; } = "Tone Analyzer Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Tone Analyzer Module is running...");
        
        string inputFile = Path.Combine(dataFolder, "input.txt");
        string outputFile = Path.Combine(dataFolder, "tone_analysis.json");
        
        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Error: Input file not found in the data folder.");
            return false;
        }
        
        string text = File.ReadAllText(inputFile);
        
        var toneAnalysis = AnalyzeTone(text);
        
        string jsonResult = JsonSerializer.Serialize(toneAnalysis, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputFile, jsonResult);
        
        Console.WriteLine("Tone analysis completed. Results saved to: " + outputFile);
        return true;
    }
    
    private ToneAnalysis AnalyzeTone(string text)
    {
        var analysis = new ToneAnalysis();
        
        // Simple tone analysis (can be expanded)
        analysis.WordCount = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        
        // Detect positive/negative tone by counting positive/negative words
        var positiveWords = new HashSet<string> { "happy", "joy", "love", "great", "wonderful", "excellent" };
        var negativeWords = new HashSet<string> { "sad", "angry", "hate", "bad", "terrible", "awful" };
        
        int positiveCount = 0;
        int negativeCount = 0;
        
        foreach (var word in text.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
        {
            string lowerWord = word.ToLower();
            if (positiveWords.Contains(lowerWord)) positiveCount++;
            if (negativeWords.Contains(lowerWord)) negativeCount++;
        }
        
        analysis.PositiveToneScore = positiveCount;
        analysis.NegativeToneScore = negativeCount;
        
        // Determine overall tone
        if (positiveCount > negativeCount)
            analysis.OverallTone = "Positive";
        else if (negativeCount > positiveCount)
            analysis.OverallTone = "Negative";
        else
            analysis.OverallTone = "Neutral";
        
        return analysis;
    }
}

public class ToneAnalysis
{
    public int WordCount { get; set; }
    public int PositiveToneScore { get; set; }
    public int NegativeToneScore { get; set; }
    public string OverallTone { get; set; }
}