using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class BasicCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Basic Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Basic Calculator Module is running.");
        Console.WriteLine("Available operations: +, -, *, /");

        try
        {
            double num1 = GetNumberFromUser("Enter the first number: ");
            double num2 = GetNumberFromUser("Enter the second number: ");
            char operation = GetOperationFromUser("Enter the operation (+, -, *, /): ");

            double result = PerformCalculation(num1, num2, operation);
            Console.WriteLine("Result: " + result);

            SaveCalculationToFile(dataFolder, num1, num2, operation, result);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private double GetNumberFromUser(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (double.TryParse(input, out double number))
            {
                return number;
            }
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }

    private char GetOperationFromUser(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (input.Length == 1 && "+-*/".Contains(input[0]))
            {
                return input[0];
            }
            Console.WriteLine("Invalid operation. Please enter one of: +, -, *, /");
        }
    }

    private double PerformCalculation(double num1, double num2, char operation)
    {
        switch (operation)
        {
            case '+':
                return num1 + num2;
            case '-':
                return num1 - num2;
            case '*':
                return num1 * num2;
            case '/':
                if (num2 == 0)
                {
                    throw new DivideByZeroException("Cannot divide by zero.");
                }
                return num1 / num2;
            default:
                throw new InvalidOperationException("Invalid operation.");
        }
    }

    private void SaveCalculationToFile(string dataFolder, double num1, double num2, char operation, double result)
    {
        try
        {
            var calculation = new
            {
                Number1 = num1,
                Number2 = num2,
                Operation = operation,
                Result = result,
                Timestamp = DateTime.Now
            };

            string fileName = Path.Combine(dataFolder, "calculations.json");
            string jsonString = JsonSerializer.Serialize(calculation);
            File.AppendAllText(fileName, jsonString + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Warning: Could not save calculation to file. " + ex.Message);
        }
    }
}