using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PolygonAreaCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Polygon Area Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Polygon Area Calculator module is running.");
        
        try
        {
            string inputFile = Path.Combine(dataFolder, "polygon_vertices.json");
            string outputFile = Path.Combine(dataFolder, "polygon_area.json");

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Input file not found. Creating a sample file.");
                CreateSampleInputFile(inputFile);
                Console.WriteLine("Please add your polygon vertices to the input file and run the module again.");
                return false;
            }

            List<Point> vertices = ReadVerticesFromFile(inputFile);
            if (vertices.Count < 3)
            {
                Console.WriteLine("A polygon must have at least 3 vertices.");
                return false;
            }

            double area = CalculatePolygonArea(vertices);
            Console.WriteLine("Calculated polygon area: " + area);

            SaveResultToFile(outputFile, area);
            Console.WriteLine("Result saved to: " + outputFile);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private List<Point> ReadVerticesFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Point>>(json);
    }

    private void SaveResultToFile(string filePath, double area)
    {
        var result = new { Area = area };
        string json = JsonSerializer.Serialize(result);
        File.WriteAllText(filePath, json);
    }

    private void CreateSampleInputFile(string filePath)
    {
        var sampleVertices = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 4, Y = 0 },
            new Point { X = 4, Y = 3 }
        };

        string json = JsonSerializer.Serialize(sampleVertices);
        File.WriteAllText(filePath, json);
    }

    private double CalculatePolygonArea(List<Point> vertices)
    {
        double area = 0.0;
        int n = vertices.Count;

        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            area += vertices[i].X * vertices[j].Y;
            area -= vertices[j].X * vertices[i].Y;
        }

        return Math.Abs(area) / 2.0;
    }
}

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }
}