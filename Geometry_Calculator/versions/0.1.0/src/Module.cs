using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class GeometryCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Geometry Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Geometry Calculator Module is running.");
        Console.WriteLine("This module calculates the area and perimeter of geometric shapes.");

        try
        {
            string shapesFilePath = Path.Combine(dataFolder, "shapes.json");
            if (!File.Exists(shapesFilePath))
            {
                Console.WriteLine("No shapes data found. Creating a default shapes file.");
                CreateDefaultShapesFile(shapesFilePath);
            }

            var shapes = LoadShapes(shapesFilePath);
            if (shapes == null || shapes.Length == 0)
            {
                Console.WriteLine("No shapes data available to process.");
                return false;
            }

            foreach (var shape in shapes)
            {
                double area = CalculateArea(shape);
                double perimeter = CalculatePerimeter(shape);

                Console.WriteLine("Shape: " + shape.Type);
                Console.WriteLine("Area: " + area);
                Console.WriteLine("Perimeter: " + perimeter);
                Console.WriteLine();
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void CreateDefaultShapesFile(string filePath)
    {
        var defaultShapes = new Shape[]
        {
            new Shape { Type = "Circle", Radius = 5 },
            new Shape { Type = "Rectangle", Width = 4, Height = 6 },
            new Shape { Type = "Square", Side = 5 },
            new Shape { Type = "Triangle", Side1 = 3, Side2 = 4, Side3 = 5 }
        };

        string json = JsonSerializer.Serialize(defaultShapes);
        File.WriteAllText(filePath, json);
    }

    private Shape[] LoadShapes(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Shape[]>(json);
    }

    private double CalculateArea(Shape shape)
    {
        switch (shape.Type)
        {
            case "Circle":
                return Math.PI * Math.Pow(shape.Radius, 2);
            case "Rectangle":
                return shape.Width * shape.Height;
            case "Square":
                return Math.Pow(shape.Side, 2);
            case "Triangle":
                double s = (shape.Side1 + shape.Side2 + shape.Side3) / 2;
                return Math.Sqrt(s * (s - shape.Side1) * (s - shape.Side2) * (s - shape.Side3));
            default:
                throw new ArgumentException("Unknown shape type: " + shape.Type);
        }
    }

    private double CalculatePerimeter(Shape shape)
    {
        switch (shape.Type)
        {
            case "Circle":
                return 2 * Math.PI * shape.Radius;
            case "Rectangle":
                return 2 * (shape.Width + shape.Height);
            case "Square":
                return 4 * shape.Side;
            case "Triangle":
                return shape.Side1 + shape.Side2 + shape.Side3;
            default:
                throw new ArgumentException("Unknown shape type: " + shape.Type);
        }
    }
}

public class Shape
{
    public string Type { get; set; }
    public double Radius { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Side { get; set; }
    public double Side1 { get; set; }
    public double Side2 { get; set; }
    public double Side3 { get; set; }
}