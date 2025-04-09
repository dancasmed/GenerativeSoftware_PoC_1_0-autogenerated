using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class MathPracticeGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Math Practice Generator";
    
    private readonly Random _random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Math Practice Generator is running...");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            var problems = GenerateMathProblems(10);
            string filePath = Path.Combine(dataFolder, "math_problems.json");
            
            string json = JsonSerializer.Serialize(problems, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            
            Console.WriteLine("Generated 10 math problems and saved to " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private MathProblem[] GenerateMathProblems(int count)
    {
        var problems = new MathProblem[count];
        
        for (int i = 0; i < count; i++)
        {
            problems[i] = GenerateRandomProblem();
        }
        
        return problems;
    }
    
    private MathProblem GenerateRandomProblem()
    {
        int operation = _random.Next(0, 4); // 0: +, 1: -, 2: *, 3: /
        int operand1 = _random.Next(1, 100);
        int operand2;
        string operationSymbol;
        int answer;
        
        switch (operation)
        {
            case 0:
                operand2 = _random.Next(1, 100);
                operationSymbol = "+";
                answer = operand1 + operand2;
                break;
                
            case 1:
                operand2 = _random.Next(1, operand1 + 1);
                operationSymbol = "-";
                answer = operand1 - operand2;
                break;
                
            case 2:
                operand2 = _random.Next(1, 20);
                operationSymbol = "*";
                answer = operand1 * operand2;
                break;
                
            case 3:
                operand2 = _random.Next(1, 20);
                int temp = operand1 * operand2;
                operand1 = temp;
                operationSymbol = "/";
                answer = operand2;
                break;
                
            default:
                throw new InvalidOperationException("Invalid operation");
        }
        
        return new MathProblem
        {
            Question = operand1 + " " + operationSymbol + " " + operand2 + " = ?",
            Answer = answer
        };
    }
}

public class MathProblem
{
    public string Question { get; set; }
    public int Answer { get; set; }
}