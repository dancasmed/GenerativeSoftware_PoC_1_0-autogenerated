using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ToneAnalyzerModule : IGeneratedModule
{
    public string Name { get; set; } = "Conversation Tone Analyzer";

    private readonly Dictionary<string, int> _positiveWords = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        {"happy", 1}, {"joy", 1}, {"excited", 1}, {"great", 1}, {"wonderful", 1}, 
        {"awesome", 1}, {"fantastic", 1}, {"love", 1}, {"like", 1}, {"good", 1}
    };

    private readonly Dictionary<string, int> _negativeWords = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        {"sad", 1}, {"angry", 1}, {"mad", 1}, {"hate", 1}, {"bad", 1}, 
        {"terrible", 1}, {"awful", 1}, {"horrible", 1}, {"dislike", 1}, {"upset", 1}
    };

    private readonly Dictionary<string, int> _neutralWords = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        {"ok", 1}, {"fine", 1}, {"neutral", 1}, {"alright", 1}, {"average", 1}, 
        {"normal", 1}, {"regular", 1}, {"usual", 1}, {"standard", 1}, {"moderate", 1}
    };

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Conversation Tone Analyzer Module Started");
        Console.WriteLine("This module analyzes the tone of a conversation based on text input.");

        string inputText = GetUserInput("Enter the conversation text to analyze: ");
        if (string.IsNullOrWhiteSpace(inputText))
        {
            Console.WriteLine("No text provided for analysis.");
            return false;
        }

        var toneResult = AnalyzeTone(inputText);
        DisplayToneAnalysis(toneResult);

        string savePath = Path.Combine(dataFolder, "tone_analysis_results.json");
        SaveAnalysisResults(savePath, toneResult, inputText);

        Console.WriteLine("Analysis completed. Results saved to " + savePath);
        return true;
    }

    private string GetUserInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    private ToneAnalysisResult AnalyzeTone(string text)
    {
        string[] words = text.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        int positiveCount = 0;
        int negativeCount = 0;
        int neutralCount = 0;

        foreach (var word in words)
        {
            if (_positiveWords.ContainsKey(word))
                positiveCount++;
            else if (_negativeWords.ContainsKey(word))
                negativeCount++;
            else if (_neutralWords.ContainsKey(word))
                neutralCount++;
        }

        int totalWords = words.Length;
        int totalToneWords = positiveCount + negativeCount + neutralCount;

        return new ToneAnalysisResult
        {
            PositivePercentage = totalToneWords > 0 ? (double)positiveCount / totalToneWords * 100 : 0,
            NegativePercentage = totalToneWords > 0 ? (double)negativeCount / totalToneWords * 100 : 0,
            NeutralPercentage = totalToneWords > 0 ? (double)neutralCount / totalToneWords * 100 : 0,
            TotalWords = totalWords,
            ToneWordsCount = totalToneWords,
            DominantTone = GetDominantTone(positiveCount, negativeCount, neutralCount)
        };
    }

    private string GetDominantTone(int positive, int negative, int neutral)
    {
        if (positive > negative && positive > neutral) return "Positive";
        if (negative > positive && negative > neutral) return "Negative";
        if (neutral > positive && neutral > negative) return "Neutral";
        return "Balanced";
    }

    private void DisplayToneAnalysis(ToneAnalysisResult result)
    {
        Console.WriteLine("\nTone Analysis Results:");
        Console.WriteLine("-------------------");
        Console.WriteLine("Total words analyzed: " + result.TotalWords);
        Console.WriteLine("Tone words detected: " + result.ToneWordsCount);
        Console.WriteLine("Positive tone: " + result.PositivePercentage.ToString("0.00") + "%");
        Console.WriteLine("Negative tone: " + result.NegativePercentage.ToString("0.00") + "%");
        Console.WriteLine("Neutral tone: " + result.NeutralPercentage.ToString("0.00") + "%");
        Console.WriteLine("Dominant tone: " + result.DominantTone);
    }

    private void SaveAnalysisResults(string filePath, ToneAnalysisResult result, string originalText)
    {
        var resultData = new
        {
            Timestamp = DateTime.Now,
            OriginalText = originalText,
            AnalysisResult = result
        };

        string json = JsonSerializer.Serialize(resultData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private class ToneAnalysisResult
    {
        public double PositivePercentage { get; set; }
        public double NegativePercentage { get; set; }
        public double NeutralPercentage { get; set; }
        public int TotalWords { get; set; }
        public int ToneWordsCount { get; set; }
        public string DominantTone { get; set; }
    }
}