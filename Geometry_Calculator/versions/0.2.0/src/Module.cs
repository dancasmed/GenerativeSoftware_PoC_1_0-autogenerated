using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class GeometryCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Geometry Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Geometry Calculator Module is running.");
        Console.WriteLine("Calculating area and perimeter of different geometric shapes.");

        try
        {
            string configPath = Path.Combine(dataFolder, "geometry_config.json");
            if (!File.Exists(configPath))
            {
                Console.WriteLine("No configuration file found. Using default values.");
                var defaultShapes = new Shape[]
                {
                    new Circle { Radius = 5 },
                    new Rectangle { Width = 4, Height = 6 },
                    new Triangle { SideA = 3, SideB = 4, SideC = 5 }
                };
                
                string json = JsonSerializer.Serialize(defaultShapes);
                File.WriteAllText(configPath, json);
            }

            string jsonData = File.ReadAllText(configPath);
            Shape[] shapes = JsonSerializer.Deserialize<Shape[]>(jsonData);

            foreach (var shape in shapes)
            {
                Console.WriteLine("Shape: " + shape.GetType().Name);
                Console.WriteLine("Area: " + shape.CalculateArea());
                Console.WriteLine("Perimeter: " + shape.CalculatePerimeter());
                Console.WriteLine();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
}

public abstract class Shape
{
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }

    public override double CalculatePerimeter()
    {
        return 2 * Math.PI * Radius;
    }
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public override double CalculateArea()
    {
        return Width * Height;
    }

    public override double CalculatePerimeter()
    {
        return 2 * (Width + Height);
    }
}

public class Triangle : Shape
{
    public double SideA { get; set; }
    public double SideB { get; set; }
    public double SideC { get; set; }

    public override double CalculateArea()
    {
        double s = (SideA + SideB + SideC) / 2;
        return Math.Sqrt(s * (s - SideA) * (s - SideB) * (s - SideC));
    }

    public override double CalculatePerimeter()
    {
        return SideA + SideB + SideC;
    }
}