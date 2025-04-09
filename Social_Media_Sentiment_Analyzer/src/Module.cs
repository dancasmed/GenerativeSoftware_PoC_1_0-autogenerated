using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SocialMediaSentimentAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Social Media Sentiment Analyzer";
    
    private readonly Dictionary<string, int> _sentimentScores = new Dictionary<string, int>()
    {
        {"happy", 5}, {"joy", 5}, {"love", 5}, {"great", 4}, {"excellent", 5},
        {"sad", -5}, {"angry", -4}, {"hate", -5}, {"bad", -3}, {"terrible", -5},
        {"good", 3}, {"nice", 3}, {"ok", 1}, {"meh", -1}, {"awesome", 4}
    };

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Social Media Sentiment Analyzer started");
        Console.WriteLine("Loading and analyzing posts...");
        
        try
        {
            string inputFile = Path.Combine(dataFolder, "posts.json");
            string outputFile = Path.Combine(dataFolder, "sentiment_analysis.json");
            
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("No posts.json file found in the data folder");
                return false;
            }
            
            string jsonContent = File.ReadAllText(inputFile);
            var posts = JsonSerializer.Deserialize<List<SocialMediaPost>>(jsonContent);
            
            if (posts == null || posts.Count == 0)
            {
                Console.WriteLine("No posts found in the file");
                return false;
            }
            
            var analysisResults = new List<SentimentAnalysisResult>();
            
            foreach (var post in posts)
            {
                var result = AnalyzePost(post);
                analysisResults.Add(result);
            }
            
            string resultJson = JsonSerializer.Serialize(analysisResults, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputFile, resultJson);
            
            Console.WriteLine("Analysis completed successfully");
            Console.WriteLine("Results saved to sentiment_analysis.json");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during sentiment analysis");
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    
    private SentimentAnalysisResult AnalyzePost(SocialMediaPost post)
    {
        int totalScore = 0;
        int wordCount = 0;
        
        string[] words = post.Content.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string word in words)
        {
            string lowerWord = word.ToLower();
            if (_sentimentScores.TryGetValue(lowerWord, out int score))
            {
                totalScore += score;
                wordCount++;
            }
        }
        
        double averageScore = wordCount > 0 ? (double)totalScore / wordCount : 0;
        string sentiment = averageScore switch
        {
            > 1.5 => "Positive",
            < -1.5 => "Negative",
            _ => "Neutral"
        };
        
        return new SentimentAnalysisResult
        {
            PostId = post.Id,
            Author = post.Author,
            ContentPreview = post.Content.Length > 50 ? post.Content.Substring(0, 50) + "..." : post.Content,
            WordCount = words.Length,
            SentimentWordsCount = wordCount,
            SentimentScore = averageScore,
            Sentiment = sentiment,
            AnalysisDate = DateTime.Now
        };
    }
}

public class SocialMediaPost
{
    public string Id { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
    public DateTime PostDate { get; set; }
}

public class SentimentAnalysisResult
{
    public string PostId { get; set; }
    public string Author { get; set; }
    public string ContentPreview { get; set; }
    public int WordCount { get; set; }
    public int SentimentWordsCount { get; set; }
    public double SentimentScore { get; set; }
    public string Sentiment { get; set; }
    public DateTime AnalysisDate { get; set; }
}