using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WaterQualityTracker : IGeneratedModule
{
    public string Name { get; set; } = "Water Quality Tracker";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Water Quality Tracker module...");

        try
        {
            string dataFilePath = Path.Combine(dataFolder, "water_quality_data.json");
            List<WaterQualitySample> samples;

            if (File.Exists(dataFilePath))
            {
                string jsonData = File.ReadAllText(dataFilePath);
                samples = JsonSerializer.Deserialize<List<WaterQualitySample>>(jsonData);
                Console.WriteLine("Loaded existing water quality data.");
            }
            else
            {
                samples = new List<WaterQualitySample>();
                Console.WriteLine("No existing data found. Starting new dataset.");
            }

            bool continueRunning = true;
            while (continueRunning)
            {
                Console.WriteLine("\nWater Quality Tracker Menu:");
                Console.WriteLine("1. Add new sample");
                Console.WriteLine("2. View all samples");
                Console.WriteLine("3. Analyze data");
                Console.WriteLine("4. Save and exit");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddSample(samples);
                        break;
                    case "2":
                        DisplaySamples(samples);
                        break;
                    case "3":
                        AnalyzeData(samples);
                        break;
                    case "4":
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

            string updatedJsonData = JsonSerializer.Serialize(samples, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dataFilePath, updatedJsonData);
            Console.WriteLine("Data saved successfully.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void AddSample(List<WaterQualitySample> samples)
    {
        Console.Write("Enter sample location: ");
        string location = Console.ReadLine();

        Console.Write("Enter date (yyyy-MM-dd): ");
        DateTime date;
        while (!DateTime.TryParse(Console.ReadLine(), out date))
        {
            Console.Write("Invalid date format. Please enter date (yyyy-MM-dd): ");
        }

        Console.Write("Enter pH level (0-14): ");
        double ph;
        while (!double.TryParse(Console.ReadLine(), out ph) || ph < 0 || ph > 14)
        {
            Console.Write("Invalid pH. Please enter value between 0-14: ");
        }

        Console.Write("Enter turbidity (NTU): ");
        double turbidity;
        while (!double.TryParse(Console.ReadLine(), out turbidity) || turbidity < 0)
        {
            Console.Write("Invalid turbidity. Please enter positive value: ");
        }

        Console.Write("Enter dissolved oxygen (mg/L): ");
        double dissolvedOxygen;
        while (!double.TryParse(Console.ReadLine(), out dissolvedOxygen) || dissolvedOxygen < 0)
        {
            Console.Write("Invalid value. Please enter positive value: ");
        }

        samples.Add(new WaterQualitySample
        {
            Location = location,
            Date = date,
            PH = ph,
            Turbidity = turbidity,
            DissolvedOxygen = dissolvedOxygen
        });

        Console.WriteLine("Sample added successfully.");
    }

    private void DisplaySamples(List<WaterQualitySample> samples)
    {
        if (samples.Count == 0)
        {
            Console.WriteLine("No samples available.");
            return;
        }

        Console.WriteLine("\nWater Quality Samples:");
        foreach (var sample in samples)
        {
            Console.WriteLine($"Location: {sample.Location}, Date: {sample.Date:yyyy-MM-dd}");
            Console.WriteLine($"pH: {sample.PH}, Turbidity: {sample.Turbidity} NTU, DO: {sample.DissolvedOxygen} mg/L");
            Console.WriteLine();
        }
    }

    private void AnalyzeData(List<WaterQualitySample> samples)
    {
        if (samples.Count == 0)
        {
            Console.WriteLine("No data available for analysis.");
            return;
        }

        double avgPH = 0, avgTurbidity = 0, avgDO = 0;
        foreach (var sample in samples)
        {
            avgPH += sample.PH;
            avgTurbidity += sample.Turbidity;
            avgDO += sample.DissolvedOxygen;
        }

        avgPH /= samples.Count;
        avgTurbidity /= samples.Count;
        avgDO /= samples.Count;

        Console.WriteLine("\nWater Quality Analysis:");
        Console.WriteLine($"Average pH: {avgPH:F2}");
        Console.WriteLine($"Average Turbidity: {avgTurbidity:F2} NTU");
        Console.WriteLine($"Average Dissolved Oxygen: {avgDO:F2} mg/L");

        Console.WriteLine("\nWater Quality Assessment:");
        Console.WriteLine($"pH Status: {(avgPH < 6.5 || avgPH > 8.5 ? "Poor" : "Good")}");
        Console.WriteLine($"Turbidity Status: {(avgTurbidity > 5 ? "Poor" : "Good")}");
        Console.WriteLine($"Dissolved Oxygen Status: {(avgDO < 5 ? "Poor" : "Good")}");
    }
}

public class WaterQualitySample
{
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public double PH { get; set; }
    public double Turbidity { get; set; }
    public double DissolvedOxygen { get; set; }
}