using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class GeoDistanceCalculator : IGeneratedModule
{
    public string Name { get; set; } = "GeoDistanceCalculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("GeoDistanceCalculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "locations.json");
            
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Configuration file not found. Creating default locations file.");
                CreateDefaultLocationsFile(configPath);
                Console.WriteLine("Please edit the locations.json file with your coordinates and run the module again.");
                return false;
            }
            
            var locations = LoadLocations(configPath);
            
            if (locations == null)
            {
                Console.WriteLine("Failed to load locations from configuration file.");
                return false;
            }
            
            double distance = CalculateDistance(locations.Location1, locations.Location2);
            Console.WriteLine("Distance between the two locations is: " + distance.ToString("F2") + " kilometers");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void CreateDefaultLocationsFile(string path)
    {
        var defaultLocations = new 
        {
            Location1 = new { Latitude = 0.0, Longitude = 0.0 },
            Location2 = new { Latitude = 0.0, Longitude = 0.0 }
        };
        
        string json = JsonSerializer.Serialize(defaultLocations, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
    
    private dynamic LoadLocations(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<dynamic>(json);
    }
    
    private double CalculateDistance(dynamic location1, dynamic location2)
    {
        double lat1 = Convert.ToDouble(location1.Latitude);
        double lon1 = Convert.ToDouble(location1.Longitude);
        double lat2 = Convert.ToDouble(location2.Latitude);
        double lon2 = Convert.ToDouble(location2.Longitude);
        
        // Haversine formula
        double R = 6371; // Earth's radius in km
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
}