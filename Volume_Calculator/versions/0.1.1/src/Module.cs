using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class VolumeCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Volume Calculator";

    private const string DataFileName = "volume_data.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Volume Calculator module is running.");
        Console.WriteLine("This module calculates the volume of various 3D shapes.");

        try
        {
            string filePath = Path.Combine(dataFolder, DataFileName);
            VolumeData data = LoadData(filePath);

            while (true)
            {
                Console.WriteLine("\nAvailable shapes:");
                Console.WriteLine("1. Cube");
                Console.WriteLine("2. Sphere");
                Console.WriteLine("3. Cylinder");
                Console.WriteLine("4. Cone");
                Console.WriteLine("5. Exit");
                Console.Write("Select a shape (1-5): ");

                string input = Console.ReadLine();

                if (input == "5")
                {
                    SaveData(filePath, data);
                    Console.WriteLine("Volume calculations saved. Exiting module.");
                    return true;
                }

                if (!int.TryParse(input, out int choice) || choice < 1 || choice > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
                    continue;
                }

                double volume = 0;
                string shapeName = "";

                switch (choice)
                {
                    case 1:
                        shapeName = "Cube";
                        Console.Write("Enter the side length: ");
                        double side = GetPositiveDoubleInput();
                        volume = Math.Pow(side, 3);
                        break;

                    case 2:
                        shapeName = "Sphere";
                        Console.Write("Enter the radius: ");
                        double radius = GetPositiveDoubleInput();
                        volume = (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);
                        break;

                    case 3:
                        shapeName = "Cylinder";
                        Console.Write("Enter the radius: ");
                        double cylRadius = GetPositiveDoubleInput();
                        Console.Write("Enter the height: ");
                        double cylHeight = GetPositiveDoubleInput();
                        volume = Math.PI * Math.Pow(cylRadius, 2) * cylHeight;
                        break;

                    case 4:
                        shapeName = "Cone";
                        Console.Write("Enter the radius: ");
                        double coneRadius = GetPositiveDoubleInput();
                        Console.Write("Enter the height: ");
                        double coneHeight = GetPositiveDoubleInput();
                        volume = (1.0 / 3.0) * Math.PI * Math.Pow(coneRadius, 2) * coneHeight;
                        break;
                }

                Console.WriteLine("The volume of the " + shapeName + " is: " + volume.ToString("F2"));
                data.Calculations.Add(new Calculation { Shape = shapeName, Volume = volume, Timestamp = DateTime.Now });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private double GetPositiveDoubleInput()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (double.TryParse(input, out double value) && value > 0)
            {
                return value;
            }
            Console.Write("Invalid input. Please enter a positive number: ");
        }
    }

    private VolumeData LoadData(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<VolumeData>(json) ?? new VolumeData();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Warning: Could not load previous data. " + ex.Message);
        }
        return new VolumeData();
    }

    private void SaveData(string filePath, VolumeData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Warning: Could not save data. " + ex.Message);
        }
    }
}

public class VolumeData
{
    public List<Calculation> Calculations { get; set; } = new List<Calculation>();
}

public class Calculation
{
    public string Shape { get; set; } = string.Empty;
    public double Volume { get; set; }
    public DateTime Timestamp { get; set; }
}