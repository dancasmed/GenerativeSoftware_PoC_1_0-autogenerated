using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TriviaQuizGame : IGeneratedModule
{
    public string Name { get; set; } = "Trivia Quiz Game";
    
    private List<TriviaQuestion> _questions;
    private string _dataFilePath;
    
    public TriviaQuizGame()
    {
        _questions = new List<TriviaQuestion>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Trivia Quiz Game...");
        
        _dataFilePath = Path.Combine(dataFolder, "trivia_questions.json");
        
        if (!File.Exists(_dataFilePath))
        {
            InitializeDefaultQuestions();
            SaveQuestionsToFile();
            Console.WriteLine("Default trivia questions created.");
        }
        else
        {
            LoadQuestionsFromFile();
        }
        
        PlayQuiz();
        
        Console.WriteLine("Trivia Quiz Game finished.");
        return true;
    }
    
    private void InitializeDefaultQuestions()
    {
        _questions = new List<TriviaQuestion>
        {
            new TriviaQuestion
            {
                Question = "What is the capital of France?",
                Options = new List<string> { "London", "Paris", "Berlin", "Madrid" },
                CorrectAnswerIndex = 1
            },
            new TriviaQuestion
            {
                Question = "Which planet is known as the Red Planet?",
                Options = new List<string> { "Venus", "Mars", "Jupiter", "Saturn" },
                CorrectAnswerIndex = 1
            },
            new TriviaQuestion
            {
                Question = "What is the largest mammal?",
                Options = new List<string> { "Elephant", "Blue Whale", "Giraffe", "Polar Bear" },
                CorrectAnswerIndex = 1
            }
        };
    }
    
    private void SaveQuestionsToFile()
    {
        var json = JsonSerializer.Serialize(_questions);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void LoadQuestionsFromFile()
    {
        var json = File.ReadAllText(_dataFilePath);
        _questions = JsonSerializer.Deserialize<List<TriviaQuestion>>(json);
    }
    
    private void PlayQuiz()
    {
        var random = new Random();
        var score = 0;
        
        Console.WriteLine("Welcome to the Trivia Quiz Game!");
        Console.WriteLine("You will be presented with random questions. Choose the correct answer.");
        
        var questions = new List<TriviaQuestion>(_questions);
        
        while (questions.Count > 0)
        {
            int index = random.Next(questions.Count);
            var question = questions[index];
            questions.RemoveAt(index);
            
            Console.WriteLine();
            Console.WriteLine(question.Question);
            
            for (int i = 0; i < question.Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Options[i]}");
            }
            
            Console.Write("Enter your choice (1-{0}): ", question.Options.Count);
            var input = Console.ReadLine();
            
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= question.Options.Count)
            {
                if (choice - 1 == question.CorrectAnswerIndex)
                {
                    Console.WriteLine("Correct!");
                    score++;
                }
                else
                {
                    Console.WriteLine("Incorrect! The correct answer was: " + question.Options[question.CorrectAnswerIndex]);
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Skipping question.");
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Quiz completed! Your score: " + score + " out of " + _questions.Count);
    }
}

public class TriviaQuestion
{
    public string Question { get; set; }
    public List<string> Options { get; set; }
    public int CorrectAnswerIndex { get; set; }
}