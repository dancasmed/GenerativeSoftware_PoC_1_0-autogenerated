using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class MultiplicationQuiz : IGeneratedModule
{
    public string Name { get; set; } = "Multiplication Quiz";
    
    private string _dataFilePath;
    private QuizData _quizData;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Multiplication Quiz...");
        
        _dataFilePath = Path.Combine(dataFolder, "quiz_results.json");
        LoadQuizData();
        
        var random = new Random();
        int score = 0;
        int totalQuestions = 5;
        
        for (int i = 0; i < totalQuestions; i++)
        {
            int a = random.Next(1, 10);
            int b = random.Next(1, 10);
            
            Console.WriteLine("Question " + (i + 1) + " of " + totalQuestions);
            Console.WriteLine("What is " + a + " x " + b + "?");
            
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out int answer))
            {
                if (answer == a * b)
                {
                    Console.WriteLine("Correct!");
                    score++;
                }
                else
                {
                    Console.WriteLine("Incorrect. The correct answer is " + (a * b) + ".");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. The correct answer is " + (a * b) + ".");
            }
        }
        
        Console.WriteLine("Quiz completed! Your score: " + score + " out of " + totalQuestions);
        
        _quizData.TotalQuizzes++;
        _quizData.TotalCorrectAnswers += score;
        _quizData.TotalQuestions += totalQuestions;
        
        SaveQuizData();
        
        return true;
    }
    
    private void LoadQuizData()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _quizData = JsonSerializer.Deserialize<QuizData>(json);
            }
            else
            {
                _quizData = new QuizData();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading quiz data: " + ex.Message);
            _quizData = new QuizData();
        }
    }
    
    private void SaveQuizData()
    {
        try
        {
            string json = JsonSerializer.Serialize(_quizData);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving quiz data: " + ex.Message);
        }
    }
}

public class QuizData
{
    public int TotalQuizzes { get; set; } = 0;
    public int TotalQuestions { get; set; } = 0;
    public int TotalCorrectAnswers { get; set; } = 0;
}