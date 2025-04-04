using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class MultiplicationQuiz : IGeneratedModule
{
    public string Name { get; set; } = "Multiplication Quiz";
    private Random random = new Random();
    private string statsFilePath;
    private QuizStats stats;

    public bool Main(string dataFolder)
    {
        statsFilePath = Path.Combine(dataFolder, "quiz_stats.json");
        LoadStats();

        Console.WriteLine("Welcome to the Multiplication Quiz!");
        Console.WriteLine("Type 'exit' at any time to quit.");
        Console.WriteLine();

        bool running = true;
        while (running)
        {
            int num1 = random.Next(1, 13);
            int num2 = random.Next(1, 13);
            int correctAnswer = num1 * num2;

            Console.WriteLine("What is " + num1 + " x " + num2 + "?");
            string input = Console.ReadLine();

            if (input.ToLower() == "exit")
            {
                running = false;
                continue;
            }

            if (int.TryParse(input, out int userAnswer))
            {
                if (userAnswer == correctAnswer)
                {
                    Console.WriteLine("Correct! Well done!");
                    stats.CorrectAnswers++;
                }
                else
                {
                    Console.WriteLine("Incorrect. The correct answer is " + correctAnswer + ".");
                    stats.IncorrectAnswers++;
                }
                stats.TotalQuestions++;
            }
            else
            {
                Console.WriteLine("Please enter a valid number or 'exit' to quit.");
            }

            Console.WriteLine();
        }

        SaveStats();
        DisplayStats();
        return true;
    }

    private void LoadStats()
    {
        try
        {
            if (File.Exists(statsFilePath))
            {
                string json = File.ReadAllText(statsFilePath);
                stats = JsonSerializer.Deserialize<QuizStats>(json);
            }
            else
            {
                stats = new QuizStats();
            }
        }
        catch
        {
            stats = new QuizStats();
        }
    }

    private void SaveStats()
    {
        try
        {
            string json = JsonSerializer.Serialize(stats);
            File.WriteAllText(statsFilePath, json);
        }
        catch
        {
            Console.WriteLine("Could not save statistics.");
        }
    }

    private void DisplayStats()
    {
        Console.WriteLine("Quiz Statistics:");
        Console.WriteLine("Total questions: " + stats.TotalQuestions);
        Console.WriteLine("Correct answers: " + stats.CorrectAnswers);
        Console.WriteLine("Incorrect answers: " + stats.IncorrectAnswers);
        
        if (stats.TotalQuestions > 0)
        {
            double percentage = (double)stats.CorrectAnswers / stats.TotalQuestions * 100;
            Console.WriteLine("Success rate: " + percentage.ToString("0.00") + "%");
        }
    }

    private class QuizStats
    {
        public int TotalQuestions { get; set; } = 0;
        public int CorrectAnswers { get; set; } = 0;
        public int IncorrectAnswers { get; set; } = 0;
    }
}