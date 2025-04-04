using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TriviaQuizModule : IGeneratedModule
{
    public string Name { get; set; } = "Trivia Quiz Module";

    private List<QuizCategory> _categories;
    private string _dataFilePath;

    public TriviaQuizModule()
    {
        _categories = new List<QuizCategory>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Trivia Quiz Module...");
        _dataFilePath = Path.Combine(dataFolder, "trivia_questions.json");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

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

        RunQuiz();
        return true;
    }

    private void InitializeDefaultQuestions()
    {
        _categories = new List<QuizCategory>
        {
            new QuizCategory
            {
                Name = "Science",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "What is the chemical symbol for gold?",
                        Options = new List<string> { "Au", "Ag", "Fe", "Hg" },
                        CorrectAnswerIndex = 0
                    },
                    new Question
                    {
                        Text = "Which planet is known as the Red Planet?",
                        Options = new List<string> { "Venus", "Mars", "Jupiter", "Saturn" },
                        CorrectAnswerIndex = 1
                    }
                }
            },
            new QuizCategory
            {
                Name = "History",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "In which year did World War II end?",
                        Options = new List<string> { "1943", "1945", "1947", "1950" },
                        CorrectAnswerIndex = 1
                    },
                    new Question
                    {
                        Text = "Who was the first President of the United States?",
                        Options = new List<string> { "Thomas Jefferson", "John Adams", "George Washington", "Abraham Lincoln" },
                        CorrectAnswerIndex = 2
                    }
                }
            }
        };
    }

    private void SaveQuestionsToFile()
    {
        string json = JsonSerializer.Serialize(_categories, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataFilePath, json);
    }

    private void LoadQuestionsFromFile()
    {
        string json = File.ReadAllText(_dataFilePath);
        _categories = JsonSerializer.Deserialize<List<QuizCategory>>(json);
    }

    private void RunQuiz()
    {
        Console.WriteLine("Welcome to the Trivia Quiz!");
        Console.WriteLine("Available categories:");

        for (int i = 0; i < _categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_categories[i].Name}");
        }

        Console.Write("Select a category number: ");
        if (int.TryParse(Console.ReadLine(), out int categoryIndex) && categoryIndex > 0 && categoryIndex <= _categories.Count)
        {
            var selectedCategory = _categories[categoryIndex - 1];
            Console.WriteLine($"\nSelected category: {selectedCategory.Name}");

            int score = 0;
            foreach (var question in selectedCategory.Questions)
            {
                Console.WriteLine($"\nQuestion: {question.Text}");
                for (int i = 0; i < question.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.Options[i]}");
                }

                Console.Write("Your answer (number): ");
                if (int.TryParse(Console.ReadLine(), out int answer) && answer > 0 && answer <= question.Options.Count)
                {
                    if (answer - 1 == question.CorrectAnswerIndex)
                    {
                        Console.WriteLine("Correct!");
                        score++;
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect! The correct answer was: {question.Options[question.CorrectAnswerIndex]}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Skipping question.");
                }
            }

            Console.WriteLine($"\nQuiz completed! Your score: {score}/{selectedCategory.Questions.Count}");
        }
        else
        {
            Console.WriteLine("Invalid category selection.");
        }
    }

    private class QuizCategory
    {
        public string Name { get; set; }
        public List<Question> Questions { get; set; }
    }

    private class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
    }
}