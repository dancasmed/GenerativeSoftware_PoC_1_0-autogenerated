using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class QuadraticEquationSolver : IGeneratedModule
{
    public string Name { get; set; } = "Quadratic Equation Solver";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Quadratic Equation Solver module is running.");
        Console.WriteLine("This module calculates the roots of a quadratic equation (ax^2 + bx + c = 0).");

        try
        {
            double a = GetCoefficient("Enter coefficient a: ");
            double b = GetCoefficient("Enter coefficient b: ");
            double c = GetCoefficient("Enter coefficient c: ");

            var roots = CalculateRoots(a, b, c);
            SaveResults(dataFolder, a, b, c, roots);

            Console.WriteLine("Roots calculated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private double GetCoefficient(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        if (!double.TryParse(input, out double coefficient))
        {
            throw new ArgumentException("Invalid input. Please enter a valid number.");
        }
        return coefficient;
    }

    private (double? root1, double? root2) CalculateRoots(double a, double b, double c)
    {
        double discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            Console.WriteLine("The equation has no real roots.");
            return (null, null);
        }
        else if (discriminant == 0)
        {
            double root = -b / (2 * a);
            Console.WriteLine("The equation has one real root: " + root);
            return (root, null);
        }
        else
        {
            double root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            Console.WriteLine("The equation has two real roots: " + root1 + " and " + root2);
            return (root1, root2);
        }
    }

    private void SaveResults(string dataFolder, double a, double b, double c, (double? root1, double? root2) roots)
    {
        var result = new
        {
            Equation = $"{a}x^2 + {b}x + {c} = 0",
            Roots = new
            {
                Root1 = roots.root1,
                Root2 = roots.root2
            },
            Timestamp = DateTime.Now
        };

        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        string filePath = Path.Combine(dataFolder, "quadratic_equation_results.json");
        File.WriteAllText(filePath, json);
    }
}