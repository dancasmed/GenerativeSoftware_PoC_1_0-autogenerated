using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class GeometryCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Geometry Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Geometry Calculator module is running.");
        Console.WriteLine("Calculating area and perimeter of geometric shapes.");

        try
        {
            string shapesFilePath = Path.Combine(dataFolder, "shapes.json");
            string resultsFilePath = Path.Combine(dataFolder, "results.json");

            if (!File.Exists(shapesFilePath))
            {
                Console.WriteLine("No shapes data found. Creating sample data.");
                CreateSampleShapesFile(shapesFilePath);
            }

            var shapes = LoadShapes(shapesFilePath);
            var results = ProcessShapes(shapes);
            SaveResults(resultsFilePath, results);

            Console.WriteLine("Calculations completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            return false;
        }
    }

    private void CreateSampleShapesFile(string filePath)
    {
        var sampleShapes = new Shape[]
        {
            new Shape { Type = "Circle", Radius = 5 },
            new Shape { Type = "Rectangle", Width = 4, Height = 6 },
            new Shape { Type = "Square", Side = 3 },
            new Shape { Type = "Triangle", SideA = 3, SideB = 4, SideC = 5 }
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(sampleShapes, options);
        File.WriteAllText(filePath, json);
    }

    private Shape[] LoadShapes(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Shape[]>(json);
    }

    private CalculationResult[] ProcessShapes(Shape[] shapes)
    {
        var results = new CalculationResult[shapes.Length];

        for (int i = 0; i < shapes.Length; i++)
        {
            var shape = shapes[i];
            double area = 0;
            double perimeter = 0;

            switch (shape.Type)
            {
                case "Circle":
                    area = Math.PI * shape.Radius * shape.Radius;
                    perimeter = 2 * Math.PI * shape.Radius;
                    break;
                case "Rectangle":
                    area = shape.Width * shape.Height;
                    perimeter = 2 * (shape.Width + shape.Height);
                    break;
                case "Square":
                    area = shape.Side * shape.Side;
                    perimeter = 4 * shape.Side;
                    break;
                case "Triangle":
                    // Using Heron's formula for area
                    double s = (shape.SideA + shape.SideB + shape.SideC) / 2;
                    area = Math.Sqrt(s * (s - shape.SideA) * (s - shape.SideB) * (s - shape.SideC));
                    perimeter = shape.SideA + shape.SideB + shape.SideC;
                    break;
            }

            results[i] = new CalculationResult
            {
                ShapeType = shape.Type,
                Area = Math.Round(area, 2),
                Perimeter = Math.Round(perimeter, 2)
            };
        }

        return results;
    }

    private void SaveResults(string filePath, CalculationResult[] results)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(results, options);
        File.WriteAllText(filePath, json);
    }
}

public class Shape
{
    public string Type { get; set; }
    public double Radius { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Side { get; set; }
    public double SideA { get; set; }
    public double SideB { get; set; }
    public double SideC { get; set; }
}

public class CalculationResult
{
    public string ShapeType { get; set; }
    public double Area { get; set; }
    public double Perimeter { get; set; }
}