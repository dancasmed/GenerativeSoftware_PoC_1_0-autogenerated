using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class OnlineLearningPlatform : IGeneratedModule
{
    public string Name { get; set; } = "Online Learning Platform";

    private List<Course> courses;
    private List<Quiz> quizzes;
    private string coursesFilePath;
    private string quizzesFilePath;

    public OnlineLearningPlatform()
    {
        courses = new List<Course>();
        quizzes = new List<Quiz>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Online Learning Platform...");

        coursesFilePath = Path.Combine(dataFolder, "courses.json");
        quizzesFilePath = Path.Combine(dataFolder, "quizzes.json");

        LoadData();

        bool running = true;
        while (running)
        {
            DisplayMenu();
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    CreateCourse();
                    break;
                case "2":
                    CreateQuiz();
                    break;
                case "3":
                    ListCourses();
                    break;
                case "4":
                    ListQuizzes();
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        SaveData();
        Console.WriteLine("Online Learning Platform session ended.");
        return true;
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(coursesFilePath))
            {
                string json = File.ReadAllText(coursesFilePath);
                courses = JsonSerializer.Deserialize<List<Course>>(json);
            }

            if (File.Exists(quizzesFilePath))
            {
                string json = File.ReadAllText(quizzesFilePath);
                quizzes = JsonSerializer.Deserialize<List<Quiz>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data: " + ex.Message);
        }
    }

    private void SaveData()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(coursesFilePath));
            string coursesJson = JsonSerializer.Serialize(courses);
            File.WriteAllText(coursesFilePath, coursesJson);

            string quizzesJson = JsonSerializer.Serialize(quizzes);
            File.WriteAllText(quizzesFilePath, quizzesJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving data: " + ex.Message);
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nOnline Learning Platform Menu:");
        Console.WriteLine("1. Create a new course");
        Console.WriteLine("2. Create a new quiz");
        Console.WriteLine("3. List all courses");
        Console.WriteLine("4. List all quizzes");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
    }

    private void CreateCourse()
    {
        Console.Write("Enter course title: ");
        string title = Console.ReadLine();

        Console.Write("Enter course description: ");
        string description = Console.ReadLine();

        Console.Write("Enter instructor name: ");
        string instructor = Console.ReadLine();

        Console.Write("Enter course duration (hours): ");
        if (int.TryParse(Console.ReadLine(), out int duration))
        {
            var course = new Course
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Description = description,
                Instructor = instructor,
                DurationHours = duration
            };

            courses.Add(course);
            Console.WriteLine("Course created successfully!");
        }
        else
        {
            Console.WriteLine("Invalid duration. Please enter a valid number.");
        }
    }

    private void CreateQuiz()
    {
        Console.Write("Enter quiz title: ");
        string title = Console.ReadLine();

        Console.Write("Enter course ID this quiz belongs to: ");
        string courseId = Console.ReadLine();

        var quiz = new Quiz
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            CourseId = courseId,
            Questions = new List<Question>()
        };

        bool addingQuestions = true;
        while (addingQuestions)
        {
            Console.Write("Add a question (Y/N)? ");
            if (Console.ReadLine().ToUpper() != "Y")
            {
                addingQuestions = false;
                continue;
            }

            Console.Write("Enter question text: ");
            string questionText = Console.ReadLine();

            var question = new Question
            {
                Text = questionText,
                Options = new List<string>(),
                CorrectAnswerIndex = 0
            };

            Console.WriteLine("Enter options (enter 'done' when finished):");
            string option;
            int optionCount = 0;
            do
            {
                Console.Write("Option " + (optionCount + 1) + ": ");
                option = Console.ReadLine();
                if (option.ToLower() != "done" && !string.IsNullOrWhiteSpace(option))
                {
                    question.Options.Add(option);
                    optionCount++;
                }
            } while (option.ToLower() != "done" && optionCount < 10);

            if (question.Options.Count > 0)
            {
                Console.Write("Enter the index of the correct answer (1-" + question.Options.Count + "): ");
                if (int.TryParse(Console.ReadLine(), out int correctIndex) && correctIndex > 0 && correctIndex <= question.Options.Count)
                {
                    question.CorrectAnswerIndex = correctIndex - 1;
                    quiz.Questions.Add(question);
                    Console.WriteLine("Question added successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid correct answer index. Question not added.");
                }
            }
            else
            {
                Console.WriteLine("No options added. Question not added.");
            }
        }

        quizzes.Add(quiz);
        Console.WriteLine("Quiz created successfully!");
    }

    private void ListCourses()
    {
        Console.WriteLine("\nAvailable Courses:");
        if (courses.Count == 0)
        {
            Console.WriteLine("No courses available.");
            return;
        }

        foreach (var course in courses)
        {
            Console.WriteLine("ID: " + course.Id);
            Console.WriteLine("Title: " + course.Title);
            Console.WriteLine("Description: " + course.Description);
            Console.WriteLine("Instructor: " + course.Instructor);
            Console.WriteLine("Duration: " + course.DurationHours + " hours");
            Console.WriteLine();
        }
    }

    private void ListQuizzes()
    {
        Console.WriteLine("\nAvailable Quizzes:");
        if (quizzes.Count == 0)
        {
            Console.WriteLine("No quizzes available.");
            return;
        }

        foreach (var quiz in quizzes)
        {
            Console.WriteLine("ID: " + quiz.Id);
            Console.WriteLine("Title: " + quiz.Title);
            Console.WriteLine("Course ID: " + quiz.CourseId);
            Console.WriteLine("Questions: " + quiz.Questions.Count);
            Console.WriteLine();
        }
    }
}

public class Course
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Instructor { get; set; }
    public int DurationHours { get; set; }
}

public class Quiz
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string CourseId { get; set; }
    public List<Question> Questions { get; set; }
}

public class Question
{
    public string Text { get; set; }
    public List<string> Options { get; set; }
    public int CorrectAnswerIndex { get; set; }
}