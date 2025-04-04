using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class TouchTypingPracticeModule : IGeneratedModule
{
    public string Name { get; set; } = "Touch Typing Practice Module";
    
    private List<string> _typingPrompts = new List<string>
    {
        "The quick brown fox jumps over the lazy dog.",
        "Pack my box with five dozen liquor jugs.",
        "How vexingly quick daft zebras jump!",
        "Bright vixens jump; dozy fowl quack.",
        "Sphinx of black quartz, judge my vow."
    };
    
    private string _resultsFilePath;
    
    public bool Main(string dataFolder)
    {
        _resultsFilePath = Path.Combine(dataFolder, "typing_results.json");
        
        Console.WriteLine("Welcome to the Touch Typing Practice Module!");
        Console.WriteLine("This will help you improve your typing speed and accuracy.");
        Console.WriteLine("You will be given a sentence to type as quickly and accurately as possible.");
        
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Start typing test");
            Console.WriteLine("2. View previous results");
            Console.WriteLine("3. Exit");
            
            string choice = Console.ReadLine();
            
            if (choice == "1")
            {
                RunTypingTest();
            }
            else if (choice == "2")
            {
                ShowPreviousResults();
            }
            else if (choice == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }
        
        return true;
    }
    
    private void RunTypingTest()
    {
        var random = new Random();
        string prompt = _typingPrompts[random.Next(_typingPrompts.Count)];
        
        Console.WriteLine();
        Console.WriteLine("Type the following sentence:");
        Console.WriteLine(prompt);
        Console.WriteLine("Press Enter when you're ready to start typing...");
        Console.ReadLine();
        
        Console.Clear();
        Console.WriteLine("Start typing now:");
        Console.WriteLine(prompt);
        
        DateTime startTime = DateTime.Now;
        string userInput = Console.ReadLine();
        DateTime endTime = DateTime.Now;
        
        TimeSpan duration = endTime - startTime;
        double wordsPerMinute = CalculateWordsPerMinute(prompt, duration.TotalMinutes);
        int accuracy = CalculateAccuracy(prompt, userInput);
        
        Console.WriteLine();
        Console.WriteLine("Test complete!");
        Console.WriteLine("Time taken: " + duration.TotalSeconds.ToString("0.00") + " seconds");
        Console.WriteLine("Typing speed: " + wordsPerMinute.ToString("0.00") + " WPM");
        Console.WriteLine("Accuracy: " + accuracy + "%");
        
        SaveResult(new TypingTestResult
        {
            Date = DateTime.Now,
            Prompt = prompt,
            WPM = wordsPerMinute,
            Accuracy = accuracy,
            DurationSeconds = duration.TotalSeconds
        });
    }
    
    private double CalculateWordsPerMinute(string prompt, double minutes)
    {
        int wordCount = prompt.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        return wordCount / minutes;
    }
    
    private int CalculateAccuracy(string prompt, string userInput)
    {
        int correctChars = 0;
        int minLength = Math.Min(prompt.Length, userInput.Length);
        
        for (int i = 0; i < minLength; i++)
        {
            if (prompt[i] == userInput[i])
            {
                correctChars++;
            }
        }
        
        double accuracy = (double)correctChars / prompt.Length * 100;
        return (int)Math.Round(accuracy);
    }
    
    private void SaveResult(TypingTestResult result)
    {
        List<TypingTestResult> results = new List<TypingTestResult>();
        
        if (File.Exists(_resultsFilePath))
        {
            string json = File.ReadAllText(_resultsFilePath);
            results = JsonSerializer.Deserialize<List<TypingTestResult>>(json);
        }
        
        results.Add(result);
        
        string newJson = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_resultsFilePath, newJson);
    }
    
    private void ShowPreviousResults()
    {
        if (!File.Exists(_resultsFilePath))
        {
            Console.WriteLine("No previous results found.");
            return;
        }
        
        string json = File.ReadAllText(_resultsFilePath);
        var results = JsonSerializer.Deserialize<List<TypingTestResult>>(json);
        
        Console.WriteLine();
        Console.WriteLine("Previous Results:");
        Console.WriteLine("----------------");
        
        foreach (var result in results.OrderByDescending(r => r.Date))
        {
            Console.WriteLine($"{result.Date:yyyy-MM-dd HH:mm} - {result.WPM:0.00} WPM - {result.Accuracy}% accuracy");
        }
    }
}

public class TypingTestResult
{
    public DateTime Date { get; set; }
    public string Prompt { get; set; }
    public double WPM { get; set; }
    public int Accuracy { get; set; }
    public double DurationSeconds { get; set; }
}