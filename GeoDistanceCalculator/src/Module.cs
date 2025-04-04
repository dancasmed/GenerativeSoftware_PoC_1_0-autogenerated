using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class GeoDistanceCalculator : IGeneratedModule
{
    public string Name { get; set; } = "GeoDistanceCalculator";

    public GeoDistanceCalculator() { }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("GeoDistanceCalculator module is running.");
        Console.WriteLine("Calculating distance between two geographical coordinates.");

        try
        {
            var configPath = Path.Combine(dataFolder, "coordinates.json");
            if (!File.Exists(configPath))
            {
                Console.WriteLine("No coordinates file found. Creating a default one.");
                var defaultCoords = new Coordinates
                {
                    Latitude1 = 40.7128,
                    Longitude1 = -74.0060,
                    Latitude2 = 34.0522,
                    Longitude2 = -118.2437
                };
                File.WriteAllText(configPath, JsonSerializer.Serialize(defaultCoords));
            }

            var json = File.ReadAllText(configPath);
            var coords = JsonSerializer.Deserialize<Coordinates>(json);

            if (coords == null)
            {
                Console.WriteLine("Failed to deserialize coordinates.");
                return false;
            }

            double distance = CalculateDistance(coords.Latitude1, coords.Longitude1, coords.Latitude2, coords.Longitude2);
            Console.WriteLine("Distance between the two points is " + distance.ToString("F2") + " kilometers.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double angle)
    {
        return Math.PI * angle / 180.0;
    }

    private class Coordinates
    {
        public double Latitude1 { get; set; }
        public double Longitude1 { get; set; }
        public double Latitude2 { get; set; }
        public double Longitude2 { get; set; }
    }
}