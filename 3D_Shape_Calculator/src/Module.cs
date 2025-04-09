using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class ShapeCalculator : IGeneratedModule
{
    public string Name { get; set; } = "3D Shape Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("3D Shape Calculator module is running.");
        Console.WriteLine("Calculating surface area and volume of 3D shapes.");

        try
        {
            string shapesDataPath = Path.Combine(dataFolder, "shapes.json");
            if (!File.Exists(shapesDataPath))
            {
                Console.WriteLine("No shapes data found. Creating sample data.");
                CreateSampleShapesData(shapesDataPath);
            }

            var shapes = LoadShapes(shapesDataPath);
            foreach (var shape in shapes)
            {
                CalculateAndDisplayShapeProperties(shape);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void CreateSampleShapesData(string filePath)
    {
        var sampleShapes = new Shape[]
        {
            new Shape { Type = "Cube", Dimensions = new double[] { 5 } },
            new Shape { Type = "Sphere", Dimensions = new double[] { 3 } },
            new Shape { Type = "Cylinder", Dimensions = new double[] { 2, 5 } },
            new Shape { Type = "Cone", Dimensions = new double[] { 3, 7 } }
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

    private void CalculateAndDisplayShapeProperties(Shape shape)
    {
        double surfaceArea = 0;
        double volume = 0;

        switch (shape.Type)
        {
            case "Cube":
                double side = shape.Dimensions[0];
                surfaceArea = 6 * side * side;
                volume = side * side * side;
                break;
            case "Sphere":
                double radius = shape.Dimensions[0];
                surfaceArea = 4 * Math.PI * radius * radius;
                volume = (4.0 / 3.0) * Math.PI * radius * radius * radius;
                break;
            case "Cylinder":
                radius = shape.Dimensions[0];
                double height = shape.Dimensions[1];
                surfaceArea = 2 * Math.PI * radius * (radius + height);
                volume = Math.PI * radius * radius * height;
                break;
            case "Cone":
                radius = shape.Dimensions[0];
                height = shape.Dimensions[1];
                double slantHeight = Math.Sqrt(radius * radius + height * height);
                surfaceArea = Math.PI * radius * (radius + slantHeight);
                volume = (1.0 / 3.0) * Math.PI * radius * radius * height;
                break;
        }

        Console.WriteLine("Shape: " + shape.Type);
        Console.WriteLine("Surface Area: " + surfaceArea.ToString("F2"));
        Console.WriteLine("Volume: " + volume.ToString("F2"));
        Console.WriteLine();
    }
}

public class Shape
{
    public string Type { get; set; }
    public double[] Dimensions { get; set; }
}