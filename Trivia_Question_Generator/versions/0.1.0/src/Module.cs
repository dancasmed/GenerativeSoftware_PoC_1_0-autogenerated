using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TriviaGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Trivia Question Generator";
    
    private List<TriviaQuestion> _questions;
    private string _dataFilePath;
    
    public TriviaGenerator()
    {
        _questions = new List<TriviaQuestion>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Trivia Generator Module Started");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        _dataFilePath = Path.Combine(dataFolder, "trivia_questions.json");
        
        LoadQuestions();
        
        if (_questions.Count == 0)
        {
            GenerateDefaultQuestions();
            SaveQuestions();
        }
        
        DisplayRandomQuestion();
        
        Console.WriteLine("Trivia Generator Module Completed");
        return true;
    }
    
    private void LoadQuestions()
    {
        if (File.Exists(_dataFilePath))
        {
            try
            {
                string json = File.ReadAllText(_dataFilePath);
                _questions = JsonSerializer.Deserialize<List<TriviaQuestion>>(json);
                Console.WriteLine("Loaded " + _questions.Count + " trivia questions from storage");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading questions: " + ex.Message);
            }
        }
    }
    
    private void SaveQuestions()
    {
        try
        {
            string json = JsonSerializer.Serialize(_questions);
            File.WriteAllText(_dataFilePath, json);
            Console.WriteLine("Saved " + _questions.Count + " trivia questions to storage");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving questions: " + ex.Message);
        }
    }
    
    private void GenerateDefaultQuestions()
    {
        _questions = new List<TriviaQuestion>
        {
            new TriviaQuestion("What is the capital of France?", "Paris"),
            new TriviaQuestion("How many continents are there?", "7"),
            new TriviaQuestion("What is the largest planet in our solar system?", "Jupiter"),
            new TriviaQuestion("Who painted the Mona Lisa?", "Leonardo da Vinci"),
            new TriviaQuestion("What is the chemical symbol for gold?", "Au")
        };
        
        Console.WriteLine("Generated " + _questions.Count + " default trivia questions");
    }
    
    private void DisplayRandomQuestion()
    {
        if (_questions.Count == 0)
        {
            Console.WriteLine("No questions available");
            return;
        }
        
        Random random = new Random();
        int index = random.Next(_questions.Count);
        TriviaQuestion question = _questions[index];
        
        Console.WriteLine("Question: " + question.QuestionText);
        Console.WriteLine("Answer: " + question.Answer);
    }
}

public class TriviaQuestion
{
    public string QuestionText { get; set; }
    public string Answer { get; set; }
    
    public TriviaQuestion(string questionText, string answer)
    {
        QuestionText = questionText;
        Answer = answer;
    }
}