using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class DeliveryRouteOptimizer : IGeneratedModule
{
    public string Name { get; set; } = "Delivery Route Optimizer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Delivery Route Optimizer module is running.");
        
        try
        {
            string inputFilePath = Path.Combine(dataFolder, "delivery_locations.json");
            string outputFilePath = Path.Combine(dataFolder, "optimized_route.json");

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found. Creating a sample file.");
                CreateSampleInputFile(inputFilePath);
            }

            List<DeliveryLocation> locations = LoadLocations(inputFilePath);
            List<DeliveryLocation> optimizedRoute = CalculateOptimalRoute(locations);
            SaveOptimizedRoute(outputFilePath, optimizedRoute);

            Console.WriteLine("Optimized route has been calculated and saved.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    private List<DeliveryLocation> LoadLocations(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<DeliveryLocation>>(json);
    }

    private void SaveOptimizedRoute(string filePath, List<DeliveryLocation> route)
    {
        string json = JsonSerializer.Serialize(route, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private List<DeliveryLocation> CalculateOptimalRoute(List<DeliveryLocation> locations)
    {
        if (locations == null || locations.Count == 0)
            return new List<DeliveryLocation>();

        // Simple implementation of nearest neighbor algorithm
        List<DeliveryLocation> optimizedRoute = new List<DeliveryLocation>();
        List<DeliveryLocation> remainingLocations = new List<DeliveryLocation>(locations);

        // Start with the first location
        DeliveryLocation currentLocation = remainingLocations[0];
        optimizedRoute.Add(currentLocation);
        remainingLocations.RemoveAt(0);

        while (remainingLocations.Count > 0)
        {
            DeliveryLocation nearest = FindNearestLocation(currentLocation, remainingLocations);
            optimizedRoute.Add(nearest);
            remainingLocations.Remove(nearest);
            currentLocation = nearest;
        }

        return optimizedRoute;
    }

    private DeliveryLocation FindNearestLocation(DeliveryLocation current, List<DeliveryLocation> locations)
    {
        DeliveryLocation nearest = null;
        double minDistance = double.MaxValue;

        foreach (var location in locations)
        {
            double distance = CalculateDistance(current, location);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = location;
            }
        }

        return nearest;
    }

    private double CalculateDistance(DeliveryLocation loc1, DeliveryLocation loc2)
    {
        // Simple Euclidean distance calculation
        double dx = loc1.X - loc2.X;
        double dy = loc1.Y - loc2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private void CreateSampleInputFile(string filePath)
    {
        var sampleLocations = new List<DeliveryLocation>
        {
            new DeliveryLocation { Id = 1, Name = "Warehouse", X = 0, Y = 0 },
            new DeliveryLocation { Id = 2, Name = "Customer A", X = 10, Y = 20 },
            new DeliveryLocation { Id = 3, Name = "Customer B", X = 15, Y = 15 },
            new DeliveryLocation { Id = 4, Name = "Customer C", X = 5, Y = 25 },
            new DeliveryLocation { Id = 5, Name = "Customer D", X = 30, Y = 10 }
        };

        string json = JsonSerializer.Serialize(sampleLocations, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

public class DeliveryLocation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}