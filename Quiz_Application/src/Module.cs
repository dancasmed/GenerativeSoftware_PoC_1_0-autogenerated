using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class QuizModule : IGeneratedModule
{
    public string Name { get; set; } = "Quiz Application";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Quiz Application...");

        string questionsFilePath = Path.Combine(dataFolder, "questions.json");
        string scoresFilePath = Path.Combine(dataFolder, "scores.json");

        if (!File.Exists(questionsFilePath))
        {
            Console.WriteLine("No questions found. Creating sample questions...");
            CreateSampleQuestions(questionsFilePath);
        }

        List<Question> questions = LoadQuestions(questionsFilePath);
        if (questions == null || questions.Count == 0)
        {
            Console.WriteLine("No questions available to start the quiz.");
            return false;
        }

        Console.WriteLine("Welcome to the Quiz Application!");
        Console.WriteLine("You will be presented with multiple-choice questions.");
        Console.WriteLine("Enter the letter of your answer (A, B, C, or D).\n");

        int score = 0;
        for (int i = 0; i < questions.Count; i++)
        {
            Question currentQuestion = questions[i];
            Console.WriteLine("Question " + (i + 1) + ": " + currentQuestion.Text);
            Console.WriteLine("A: " + currentQuestion.Options[0]);
            Console.WriteLine("B: " + currentQuestion.Options[1]);
            Console.WriteLine("C: " + currentQuestion.Options[2]);
            Console.WriteLine("D: " + currentQuestion.Options[3]);

            string userAnswer = Console.ReadLine()?.Trim().ToUpper();
            while (string.IsNullOrEmpty(userAnswer) || !"ABCD".Contains(userAnswer))
            {
                Console.WriteLine("Invalid input. Please enter A, B, C, or D.");
                userAnswer = Console.ReadLine()?.Trim().ToUpper();
            }

            if (userAnswer == currentQuestion.CorrectAnswer)
            {
                Console.WriteLine("Correct!");
                score++;
            }
            else
            {
                Console.WriteLine("Incorrect. The correct answer is " + currentQuestion.CorrectAnswer + ".");
            }
            Console.WriteLine();
        }

        Console.WriteLine("Quiz completed! Your score: " + score + " out of " + questions.Count);
        SaveScore(scoresFilePath, score, questions.Count);

        return true;
    }

    private void CreateSampleQuestions(string filePath)
    {
        var sampleQuestions = new List<Question>
        {
            new Question
            {
                Text = "What is the capital of France?",
                Options = new List<string> { "London", "Paris", "Berlin", "Madrid" },
                CorrectAnswer = "B"
            },
            new Question
            {
                Text = "Which planet is known as the Red Planet?",
                Options = new List<string> { "Venus", "Mars", "Jupiter", "Saturn" },
                CorrectAnswer = "B"
            },
            new Question
            {
                Text = "What is 2 + 2?",
                Options = new List<string> { "3", "4", "5", "6" },
                CorrectAnswer = "B"
            }
        };

        string json = JsonSerializer.Serialize(sampleQuestions);
        File.WriteAllText(filePath, json);
    }

    private List<Question> LoadQuestions(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Question>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading questions: " + ex.Message);
            return null;
        }
    }

    private void SaveScore(string filePath, int score, int totalQuestions)
    {
        try
        {
            var scoreData = new ScoreData
            {
                Date = DateTime.Now,
                Score = score,
                TotalQuestions = totalQuestions
            };

            List<ScoreData> scores;
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                scores = JsonSerializer.Deserialize<List<ScoreData>>(existingJson);
            }
            else
            {
                scores = new List<ScoreData>();
            }

            scores.Add(scoreData);
            string json = JsonSerializer.Serialize(scores);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving score: " + ex.Message);
        }
    }
}

public class Question
{
    public string Text { get; set; }
    public List<string> Options { get; set; }
    public string CorrectAnswer { get; set; }
}

public class ScoreData
{
    public DateTime Date { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
}