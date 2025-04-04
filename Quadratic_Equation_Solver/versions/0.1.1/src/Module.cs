using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class QuadraticEquationSolver : IGeneratedModule
{
    public string Name { get; set; } = "Quadratic Equation Solver";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Quadratic Equation Solver Module is running...");
        
        double a = 0, b = 0, c = 0;
        bool validInput = false;
        
        while (!validInput)
        {
            try
            {
                Console.Write("Enter coefficient a: ");
                a = double.Parse(Console.ReadLine());
                
                Console.Write("Enter coefficient b: ");
                b = double.Parse(Console.ReadLine());
                
                Console.Write("Enter coefficient c: ");
                c = double.Parse(Console.ReadLine());
                
                validInput = true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter valid numbers.");
            }
        }
        
        var roots = CalculateRoots(a, b, c);
        
        Console.WriteLine("Calculating roots...");
        
        if (roots == null)
        {
            Console.WriteLine("The equation has no real roots.");
        }
        else if (roots.Item1 == roots.Item2)
        {
            Console.WriteLine("The equation has one real root: " + roots.Item1);
        }
        else
        {
            Console.WriteLine("The equation has two real roots: " + roots.Item1 + " and " + roots.Item2);
        }
        
        SaveResults(dataFolder, a, b, c, roots);
        
        return true;
    }
    
    private Tuple<double, double> CalculateRoots(double a, double b, double c)
    {
        double discriminant = b * b - 4 * a * c;
        
        if (discriminant < 0)
        {
            return null;
        }
        
        double sqrtDiscriminant = Math.Sqrt(discriminant);
        double root1 = (-b + sqrtDiscriminant) / (2 * a);
        double root2 = (-b - sqrtDiscriminant) / (2 * a);
        
        return new Tuple<double, double>(root1, root2);
    }
    
    private void SaveResults(string dataFolder, double a, double b, double c, Tuple<double, double> roots)
    {
        try
        {
            object result;
            
            if (roots == null)
            {
                result = new
                {
                    Coefficients = new { a, b, c },
                    Roots = "No real roots",
                    Timestamp = DateTime.Now
                };
            }
            else if (roots.Item1 == roots.Item2)
            {
                result = new
                {
                    Coefficients = new { a, b, c },
                    Roots = new { Root = roots.Item1 },
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                result = new
                {
                    Coefficients = new { a, b, c },
                    Roots = new { Root1 = roots.Item1, Root2 = roots.Item2 },
                    Timestamp = DateTime.Now
                };
            }
            
            string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            
            string filePath = Path.Combine(dataFolder, "quadratic_results.json");
            File.WriteAllText(filePath, json);
            
            Console.WriteLine("Results saved to " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving results: " + ex.Message);
        }
    }
}