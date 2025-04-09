using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class FuelEfficiencyCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Fuel Efficiency Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Fuel Efficiency Calculator module is running.");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string dataFilePath = Path.Combine(dataFolder, "fuel_efficiency_data.json");
            
            Console.WriteLine("Enter the total distance traveled (in kilometers):");
            double distance = double.Parse(Console.ReadLine());
            
            Console.WriteLine("Enter the total fuel consumed (in liters):");
            double fuelConsumed = double.Parse(Console.ReadLine());
            
            if (distance <= 0 || fuelConsumed <= 0)
            {
                Console.WriteLine("Error: Distance and fuel consumed must be greater than zero.");
                return false;
            }
            
            double efficiency = distance / fuelConsumed;
            
            var efficiencyData = new
            {
                DistanceKm = distance,
                FuelConsumedLiters = fuelConsumed,
                EfficiencyKmPerLiter = efficiency,
                CalculationDate = DateTime.Now
            };
            
            string jsonData = JsonSerializer.Serialize(efficiencyData);
            File.WriteAllText(dataFilePath, jsonData);
            
            Console.WriteLine("Fuel efficiency calculated successfully: " + efficiency + " km/l");
            Console.WriteLine("Data saved to: " + dataFilePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}