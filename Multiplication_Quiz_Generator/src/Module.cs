using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MultiplicationQuizGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Multiplication Quiz Generator";
    private Random _random = new Random();
    private List<QuizQuestion> _questions = new List<QuizQuestion>();
    private string _quizResultsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Multiplication Quiz Generator...");
        Console.WriteLine("Generating practice quizzes for multiplication tables.");

        _quizResultsFilePath = Path.Combine(dataFolder, "quiz_results.json");

        GenerateQuestions();
        RunQuiz();
        SaveResults();

        Console.WriteLine("Quiz completed. Results saved to " + _quizResultsFilePath);
        return true;
    }

    private void GenerateQuestions()
    {
        _questions.Clear();
        for (int i = 0; i < 10; i++)
        {
            int a = _random.Next(1, 13);
            int b = _random.Next(1, 13);
            _questions.Add(new QuizQuestion(a, b));
        }
    }

    private void RunQuiz()
    {
        int correctAnswers = 0;
        Console.WriteLine("Answer the following multiplication questions:");

        foreach (var question in _questions)
        {
            Console.Write(question.QuestionText + " ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int answer))
            {
                question.UserAnswer = answer;
                if (answer == question.CorrectAnswer)
                {
                    correctAnswers++;
                    Console.WriteLine("Correct!");
                }
                else
                {
                    Console.WriteLine("Incorrect. The correct answer is " + question.CorrectAnswer);
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                question.UserAnswer = null;
            }
        }

        Console.WriteLine("You got " + correctAnswers + " out of " + _questions.Count + " correct.");
    }

    private void SaveResults()
    {
        try
        {
            string json = JsonSerializer.Serialize(_questions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_quizResultsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving quiz results: " + ex.Message);
        }
    }
}

public class QuizQuestion
{
    public int Multiplier1 { get; set; }
    public int Multiplier2 { get; set; }
    public int? UserAnswer { get; set; }
    public int CorrectAnswer => Multiplier1 * Multiplier2;
    public string QuestionText => Multiplier1 + " x " + Multiplier2 + " = ?";

    public QuizQuestion(int a, int b)
    {
        Multiplier1 = a;
        Multiplier2 = b;
    }
}