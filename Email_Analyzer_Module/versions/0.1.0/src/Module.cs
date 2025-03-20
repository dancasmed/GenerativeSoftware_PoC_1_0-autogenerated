namespace GenerativeSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using Azure.AI.TextAnalytics;
using Azure;

public class EmailAnalyzerModule : IGeneratedModule
{
    public string Name { get; set; } = "Email Analyzer Module";

    private TextAnalyticsClient textAnalyticsClient;
    private Stopwatch stopwatch;

    public EmailAnalyzerModule()
    {
        var endpoint = new Uri("YOUR_AZURE_ENDPOINT");
        var credential = new AzureKeyCredential("YOUR_AZURE_KEY");
        textAnalyticsClient = new TextAnalyticsClient(endpoint, credential);
        stopwatch = new Stopwatch();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Email Analyzer Module...");

        var emlFiles = Directory.GetFiles(dataFolder, "*.eml");
        if (emlFiles.Length == 0)
        {
            Console.WriteLine("No .eml files found in the specified folder.");
            return false;
        }

        foreach (var emlFile in emlFiles)
        {
            stopwatch.Restart();
            var emailContent = File.ReadAllText(emlFile);
            var intent = AnalyzeIntent(emailContent);
            var response = GenerateBaseResponse(intent);
            Console.WriteLine("Generated response: " + response);
            stopwatch.Stop();
            Console.WriteLine("Response time: " + stopwatch.ElapsedMilliseconds + " ms");

            Console.WriteLine("Would you like to edit the response? (yes/no)");
            var editResponse = Console.ReadLine();
            if (editResponse.ToLower() == "yes")
            {
                Console.WriteLine("Enter your edited response:");
                response = Console.ReadLine();
            }

            SaveResponse(dataFolder, Path.GetFileNameWithoutExtension(emlFile), response);
        }

        Console.WriteLine("Email analysis completed.");
        return true;
    }

    private string AnalyzeIntent(string text)
    {
        var response = textAnalyticsClient.AnalyzeSentiment(text);
        var sentiment = response.Value.Sentiment;

        if (text.Contains("claim"))
            return "claim";
        else if (text.Contains("inquiry"))
            return "inquiry";
        else if (text.Contains("purchase"))
            return "purchase";
        else
            return "other";
    }

    private string GenerateBaseResponse(string intent)
    {
        switch (intent)
        {
            case "claim":
                return "Thank you for your claim. We will process it shortly.";
            case "inquiry":
                return "Thank you for your inquiry. We will get back to you soon.";
            case "purchase":
                return "Thank you for your purchase. Your order is being processed.";
            default:
                return "Thank you for your message. We will review it shortly.";
        }
    }

    private void SaveResponse(string dataFolder, string fileName, string response)
    {
        var responseFilePath = Path.Combine(dataFolder, fileName + "_response.txt");
        File.WriteAllText(responseFilePath, response);
        Console.WriteLine("Response saved to " + responseFilePath);
    }
}