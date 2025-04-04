using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class VolumeCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Volume Calculator Module";

    private const string ConfigFileName = "volume_config.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Volume Calculator Module is running...");
        Console.WriteLine("This module calculates volumes of various 3D shapes.");

        try
        {
            string configPath = Path.Combine(dataFolder, ConfigFileName);
            VolumeConfig config = LoadOrCreateConfig(configPath);

            while (true)
            {
                Console.WriteLine("\nAvailable shapes:");
                Console.WriteLine("1. Cube");
                Console.WriteLine("2. Sphere");
                Console.WriteLine("3. Cylinder");
                Console.WriteLine("4. Cone");
                Console.WriteLine("5. Rectangular Prism");
                Console.WriteLine("6. Exit");

                Console.Write("Select a shape (1-6): ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice) || choice < 1 || choice > 6)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                    continue;
                }

                if (choice == 6)
                {
                    Console.WriteLine("Exiting Volume Calculator Module.");
                    break;
                }

                double volume = 0;
                string shapeName = "";

                switch (choice)
                {
                    case 1:
                        shapeName = "Cube";
                        Console.Write("Enter the side length: ");
                        double side = GetPositiveDouble();
                        volume = CalculateCubeVolume(side);
                        break;
                    case 2:
                        shapeName = "Sphere";
                        Console.Write("Enter the radius: ");
                        double radius = GetPositiveDouble();
                        volume = CalculateSphereVolume(radius);
                        break;
                    case 3:
                        shapeName = "Cylinder";
                        Console.Write("Enter the radius: ");
                        double cylRadius = GetPositiveDouble();
                        Console.Write("Enter the height: ");
                        double cylHeight = GetPositiveDouble();
                        volume = CalculateCylinderVolume(cylRadius, cylHeight);
                        break;
                    case 4:
                        shapeName = "Cone";
                        Console.Write("Enter the radius: ");
                        double coneRadius = GetPositiveDouble();
                        Console.Write("Enter the height: ");
                        double coneHeight = GetPositiveDouble();
                        volume = CalculateConeVolume(coneRadius, coneHeight);
                        break;
                    case 5:
                        shapeName = "Rectangular Prism";
                        Console.Write("Enter the length: ");
                        double length = GetPositiveDouble();
                        Console.Write("Enter the width: ");
                        double width = GetPositiveDouble();
                        Console.Write("Enter the height: ");
                        double height = GetPositiveDouble();
                        volume = CalculateRectangularPrismVolume(length, width, height);
                        break;
                }

                Console.WriteLine("The volume of the {0} is: {1:F2}", shapeName, volume);
                config.LastCalculatedShape = shapeName;
                config.LastCalculatedVolume = volume;
                SaveConfig(configPath, config);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private double GetPositiveDouble()
    {
        while (true)
        {
            string input = Console.ReadLine();
            if (double.TryParse(input, out double value) && value > 0)
            {
                return value;
            }
            Console.WriteLine("Invalid input. Please enter a positive number.");
        }
    }

    private VolumeConfig LoadOrCreateConfig(string configPath)
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<VolumeConfig>(json) ?? new VolumeConfig();
        }
        return new VolumeConfig();
    }

    private void SaveConfig(string configPath, VolumeConfig config)
    {
        string json = JsonSerializer.Serialize(config);
        File.WriteAllText(configPath, json);
    }

    private double CalculateCubeVolume(double side)
    {
        return Math.Pow(side, 3);
    }

    private double CalculateSphereVolume(double radius)
    {
        return (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);
    }

    private double CalculateCylinderVolume(double radius, double height)
    {
        return Math.PI * Math.Pow(radius, 2) * height;
    }

    private double CalculateConeVolume(double radius, double height)
    {
        return (1.0 / 3.0) * Math.PI * Math.Pow(radius, 2) * height;
    }

    private double CalculateRectangularPrismVolume(double length, double width, double height)
    {
        return length * width * height;
    }
}

public class VolumeConfig
{
    public string LastCalculatedShape { get; set; } = string.Empty;
    public double LastCalculatedVolume { get; set; } = 0;
}