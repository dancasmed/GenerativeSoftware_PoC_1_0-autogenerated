namespace GenerativeSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

public class EmailAnalyzerModule : IGeneratedModule
{
    public string Name { get; set; } = "Email Analyzer Module";

    private TextAnalyticsClient _textAnalyticsClient;

    public EmailAnalyzerModule(string subscriptionKey, string endpoint)
    {
        _textAnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(subscriptionKey))
        {
            Endpoint = endpoint
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Email Analyzer Module...");

        var stopwatch = Stopwatch.StartNew();

        var emailFiles = Directory.GetFiles(dataFolder, "*.eml");
        if (emailFiles.Length == 0)
        {
            Console.WriteLine("No email files found in the specified folder.");
            return false;
        }

        var responses = new List<EmailResponse>();

        foreach (var emailFile in emailFiles)
        {
            var emailContent = File.ReadAllText(emailFile);
            var intent = AnalyzeIntent(emailContent);
            var response = GenerateResponse(intent);
            responses.Add(new EmailResponse { FileName = Path.GetFileName(emailFile), Intent = intent, SuggestedResponse = response });
        }

        stopwatch.Stop();
        Console.WriteLine("Email analysis completed in " + stopwatch.ElapsedMilliseconds + " ms.");

        SaveResponses(responses, dataFolder);

        Console.WriteLine("Responses saved. You can now edit them if necessary.");
        return true;
    }

    private string AnalyzeIntent(string text)
    {
        var result = _textAnalyticsClient.Entities(text);
        var entities = result.Entities.Select(e => e.Name).ToList();

        if (entities.Contains("claim"))
            return "claim";
        else if (entities.Contains("inquiry"))
            return "inquiry";
        else if (entities.Contains("purchase"))
            return "purchase";
        else
            return "other";
    }

    private string GenerateResponse(string intent)
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
                return "Thank you for your email. We will review it and get back to you.";
        }
    }

    private void SaveResponses(List<EmailResponse> responses, string dataFolder)
    {
        var json = JsonConvert.SerializeObject(responses, Formatting.Indented);
        File.WriteAllText(Path.Combine(dataFolder, "responses.json"), json);
    }
}

public class EmailResponse
{
    public string FileName { get; set; }
    public string Intent { get; set; }
    public string SuggestedResponse { get; set; }
}