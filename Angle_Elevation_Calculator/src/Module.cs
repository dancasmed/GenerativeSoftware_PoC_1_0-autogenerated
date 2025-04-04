using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class AngleElevationCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Angle Elevation Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Angle Elevation Calculator module is running.");
        
        try
        {
            string inputFile = Path.Combine(dataFolder, "input.json");
            string outputFile = Path.Combine(dataFolder, "output.json");
            
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Input file not found. Creating a sample input file.");
                CreateSampleInputFile(inputFile);
                Console.WriteLine("Please fill the input file with the required values and run the module again.");
                return false;
            }
            
            var inputData = ReadInputData(inputFile);
            if (inputData == null)
            {
                Console.WriteLine("Failed to read input data.");
                return false;
            }
            
            double angle = CalculateAngleOfElevation(inputData.Height, inputData.Distance);
            
            var result = new 
            {
                AngleOfElevation = angle,
                Height = inputData.Height,
                Distance = inputData.Distance,
                CalculationTime = DateTime.Now
            };
            
            File.WriteAllText(outputFile, Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine("Calculation completed successfully. Result saved to output file.");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private void CreateSampleInputFile(string filePath)
    {
        var sampleData = new 
        {
            Height = 10.0,
            Distance = 20.0
        };
        
        File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(sampleData, Newtonsoft.Json.Formatting.Indented));
    }
    
    private dynamic ReadInputData(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
    }
    
    private double CalculateAngleOfElevation(double height, double distance)
    {
        if (distance <= 0)
        {
            throw new ArgumentException("Distance must be greater than zero.");
        }
        
        double radians = Math.Atan(height / distance);
        return radians * (180.0 / Math.PI);
    }
}