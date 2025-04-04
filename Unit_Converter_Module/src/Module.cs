using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class UnitConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Unit Converter Module";

    private class ConversionData
    {
        public double KilometersToMiles { get; set; } = 0.621371;
        public double MilesToKilometers { get; set; } = 1.60934;
        public double MetersToFeet { get; set; } = 3.28084;
        public double FeetToMeters { get; set; } = 0.3048;
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Unit Converter Module is running...");
        
        string configPath = Path.Combine(dataFolder, "unit_conversion_config.json");
        ConversionData conversionData;
        
        try
        {
            if (!File.Exists(configPath))
            {
                conversionData = new ConversionData();
                string jsonData = JsonSerializer.Serialize(conversionData);
                File.WriteAllText(configPath, jsonData);
                Console.WriteLine("Created default conversion configuration file.");
            }
            else
            {
                string jsonData = File.ReadAllText(configPath);
                conversionData = JsonSerializer.Deserialize<ConversionData>(jsonData);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading configuration: " + ex.Message);
            return false;
        }

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nAvailable conversions:");
            Console.WriteLine("1. Kilometers to Miles");
            Console.WriteLine("2. Miles to Kilometers");
            Console.WriteLine("3. Meters to Feet");
            Console.WriteLine("4. Feet to Meters");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option (1-5): ");
            
            string input = Console.ReadLine();
            
            if (!int.TryParse(input, out int option) || option < 1 || option > 5)
            {
                Console.WriteLine("Invalid option. Please try again.");
                continue;
            }
            
            if (option == 5)
            {
                running = false;
                continue;
            }
            
            Console.Write("Enter the value to convert: ");
            input = Console.ReadLine();
            
            if (!double.TryParse(input, out double value))
            {
                Console.WriteLine("Invalid value. Please enter a number.");
                continue;
            }
            
            double result = 0;
            string conversionType = "";
            
            switch (option)
            {
                case 1:
                    result = value * conversionData.KilometersToMiles;
                    conversionType = "kilometers to miles";
                    break;
                case 2:
                    result = value * conversionData.MilesToKilometers;
                    conversionType = "miles to kilometers";
                    break;
                case 3:
                    result = value * conversionData.MetersToFeet;
                    conversionType = "meters to feet";
                    break;
                case 4:
                    result = value * conversionData.FeetToMeters;
                    conversionType = "feet to meters";
                    break;
            }
            
            Console.WriteLine(string.Format("{0} {1} = {2} {3}", 
                value, 
                conversionType.Split(' ')[0], 
                result, 
                conversionType.Split(' ')[2]));
        }
        
        return true;
    }
}