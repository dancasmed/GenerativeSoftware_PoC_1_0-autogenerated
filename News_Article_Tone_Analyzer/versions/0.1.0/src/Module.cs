using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ToneAnalyzerModule : IGeneratedModule
{
    public string Name { get; set; } = "News Article Tone Analyzer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting News Article Tone Analyzer...");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        string inputPath = Path.Combine(dataFolder, "input_article.txt");
        string outputPath = Path.Combine(dataFolder, "tone_analysis.json");

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Input article file not found. Please place your article in 'input_article.txt' inside the data folder.");
            return false;
        }

        string articleText = File.ReadAllText(inputPath);
        var toneAnalysis = AnalyzeTone(articleText);

        string jsonResult = JsonSerializer.Serialize(toneAnalysis, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputPath, jsonResult);

        Console.WriteLine("Tone analysis completed. Results saved to 'tone_analysis.json'.");
        return true;
    }

    private Dictionary<string, int> AnalyzeTone(string text)
    {
        var toneCategories = new Dictionary<string, int>
        {
            { "Positive", 0 },
            { "Negative", 0 },
            { "Neutral", 0 },
            { "Strong", 0 },
            { "Weak", 0 }
        };

        string[] positiveWords = { "good", "great", "excellent", "happy", "joy", "success" };
        string[] negativeWords = { "bad", "terrible", "horrible", "sad", "failure", "angry" };
        string[] strongWords = { "must", "urgent", "critical", "demand", "require" };
        string[] weakWords = { "maybe", "perhaps", "possibly", "suggest", "consider" };

        string[] words = text.Split(new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            string lowerWord = word.ToLower();

            if (Array.Exists(positiveWords, w => w == lowerWord))
                toneCategories["Positive"]++;
            else if (Array.Exists(negativeWords, w => w == lowerWord))
                toneCategories["Negative"]++;
            else if (Array.Exists(strongWords, w => w == lowerWord))
                toneCategories["Strong"]++;
            else if (Array.Exists(weakWords, w => w == lowerWord))
                toneCategories["Weak"]++;
            else
                toneCategories["Neutral"]++;
        }

        return toneCategories;
    }
}