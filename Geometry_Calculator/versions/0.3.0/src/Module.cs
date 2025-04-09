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
        Console.WriteLine("This module calculates the area and perimeter of various geometric shapes.");

        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string resultFilePath = Path.Combine(dataFolder, "geometry_results.json");

            while (true)
            {
                Console.WriteLine("\nSelect a shape to calculate:");
                Console.WriteLine("1. Rectangle");
                Console.WriteLine("2. Circle");
                Console.WriteLine("3. Triangle");
                Console.WriteLine("4. Exit");

                string input = Console.ReadLine();

                if (input == "4")
                {
                    break;
                }

                switch (input)
                {
                    case "1":
                        CalculateRectangle(resultFilePath);
                        break;
                    case "2":
                        CalculateCircle(resultFilePath);
                        break;
                    case "3":
                        CalculateTriangle(resultFilePath);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            Console.WriteLine("Geometry calculations completed. Results saved to " + Path.Combine(dataFolder, "geometry_results.json"));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void CalculateRectangle(string resultFilePath)
    {
        Console.WriteLine("Enter the length of the rectangle:");
        double length = double.Parse(Console.ReadLine());

        Console.WriteLine("Enter the width of the rectangle:");
        double width = double.Parse(Console.ReadLine());

        double area = length * width;
        double perimeter = 2 * (length + width);

        var result = new
        {
            Shape = "Rectangle",
            Length = length,
            Width = width,
            Area = area,
            Perimeter = perimeter,
            Timestamp = DateTime.Now
        };

        SaveResult(result, resultFilePath);
        Console.WriteLine("Rectangle - Area: " + area + ", Perimeter: " + perimeter);
    }

    private void CalculateCircle(string resultFilePath)
    {
        Console.WriteLine("Enter the radius of the circle:");
        double radius = double.Parse(Console.ReadLine());

        double area = Math.PI * Math.Pow(radius, 2);
        double perimeter = 2 * Math.PI * radius;

        var result = new
        {
            Shape = "Circle",
            Radius = radius,
            Area = area,
            Perimeter = perimeter,
            Timestamp = DateTime.Now
        };

        SaveResult(result, resultFilePath);
        Console.WriteLine("Circle - Area: " + area + ", Perimeter: " + perimeter);
    }

    private void CalculateTriangle(string resultFilePath)
    {
        Console.WriteLine("Enter the length of side 1:");
        double side1 = double.Parse(Console.ReadLine());

        Console.WriteLine("Enter the length of side 2:");
        double side2 = double.Parse(Console.ReadLine());

        Console.WriteLine("Enter the length of side 3:");
        double side3 = double.Parse(Console.ReadLine());

        // Using Heron's formula for area
        double s = (side1 + side2 + side3) / 2;
        double area = Math.Sqrt(s * (s - side1) * (s - side2) * (s - side3));
        double perimeter = side1 + side2 + side3;

        var result = new
        {
            Shape = "Triangle",
            Side1 = side1,
            Side2 = side2,
            Side3 = side3,
            Area = area,
            Perimeter = perimeter,
            Timestamp = DateTime.Now
        };

        SaveResult(result, resultFilePath);
        Console.WriteLine("Triangle - Area: " + area + ", Perimeter: " + perimeter);
    }

    private void SaveResult(object result, string filePath)
    {
        string jsonString = JsonSerializer.Serialize(result);
        File.AppendAllText(filePath, jsonString + Environment.NewLine);
    }
}