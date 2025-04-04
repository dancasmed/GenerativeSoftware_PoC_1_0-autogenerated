using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class PaintCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Paint Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Paint Calculator module is running.");
        Console.WriteLine("This module calculates the amount of paint needed to cover a room's walls.");

        try
        {
            string configPath = Path.Combine(dataFolder, "paint_config.json");
            PaintConfig config;

            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<PaintConfig>(json);
                Console.WriteLine("Loaded existing configuration.");
            }
            else
            {
                config = GetUserInput();
                string json = JsonSerializer.Serialize(config);
                File.WriteAllText(configPath, json);
                Console.WriteLine("Configuration saved.");
            }

            double totalArea = CalculateTotalWallArea(config);
            double paintNeeded = CalculatePaintNeeded(totalArea, config.Coats, config.PaintCoverage);

            Console.WriteLine("Calculation results:");
            Console.WriteLine("Total wall area: " + totalArea + " square meters");
            Console.WriteLine("Number of coats: " + config.Coats);
            Console.WriteLine("Paint needed: " + paintNeeded + " liters");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private PaintConfig GetUserInput()
    {
        PaintConfig config = new PaintConfig();

        Console.Write("Enter room length (m): ");
        config.Length = double.Parse(Console.ReadLine());

        Console.Write("Enter room width (m): ");
        config.Width = double.Parse(Console.ReadLine());

        Console.Write("Enter room height (m): ");
        config.Height = double.Parse(Console.ReadLine());

        Console.Write("Enter number of doors: ");
        config.Doors = int.Parse(Console.ReadLine());

        Console.Write("Enter number of windows: ");
        config.Windows = int.Parse(Console.ReadLine());

        Console.Write("Enter number of coats: ");
        config.Coats = int.Parse(Console.ReadLine());

        Console.Write("Enter paint coverage (sq m per liter): ");
        config.PaintCoverage = double.Parse(Console.ReadLine());

        return config;
    }

    private double CalculateTotalWallArea(PaintConfig config)
    {
        double wallArea = 2 * (config.Length + config.Width) * config.Height;
        double doorArea = config.Doors * 1.8; // Standard door size
        double windowArea = config.Windows * 1.2; // Standard window size
        return wallArea - doorArea - windowArea;
    }

    private double CalculatePaintNeeded(double totalArea, int coats, double coverage)
    {
        return (totalArea * coats) / coverage;
    }

    private class PaintConfig
    {
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int Doors { get; set; }
        public int Windows { get; set; }
        public int Coats { get; set; }
        public double PaintCoverage { get; set; }
    }
}