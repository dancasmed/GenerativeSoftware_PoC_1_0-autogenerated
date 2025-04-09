using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class BodyFatCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Body Fat Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Body Fat Calculator module is running.");
        
        try
        {
            string inputFilePath = Path.Combine(dataFolder, "body_fat_input.json");
            string outputFilePath = Path.Combine(dataFolder, "body_fat_output.json");
            
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found. Creating a sample input file.");
                CreateSampleInputFile(inputFilePath);
                Console.WriteLine("Please fill in the sample input file and run the module again.");
                return false;
            }
            
            BodyFatInput input = ReadInputFile(inputFilePath);
            
            if (input == null)
            {
                Console.WriteLine("Failed to read input file.");
                return false;
            }
            
            double bodyFatPercentage = CalculateBodyFatPercentage(input);
            
            BodyFatResult result = new BodyFatResult
            {
                BodyFatPercentage = bodyFatPercentage,
                Timestamp = DateTime.Now
            };
            
            SaveResult(outputFilePath, result);
            
            Console.WriteLine("Body fat percentage calculated successfully.");
            Console.WriteLine("Result saved to: " + outputFilePath);
            
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
        BodyFatInput sampleInput = new BodyFatInput
        {
            Gender = "male",
            Age = 30,
            WeightKg = 75,
            HeightCm = 175,
            NeckCm = 38,
            WaistCm = 80,
            HipCm = 95 // Only used for female
        };
        
        string json = JsonSerializer.Serialize(sampleInput, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
    
    private BodyFatInput ReadInputFile(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<BodyFatInput>(json);
        }
        catch
        {
            return null;
        }
    }
    
    private double CalculateBodyFatPercentage(BodyFatInput input)
    {
        if (input.Gender.ToLower() == "male")
        {
            // US Navy method for males
            return 86.010 * Math.Log10(input.WaistCm - input.NeckCm) - 70.041 * Math.Log10(input.HeightCm) + 36.76;
        }
        else
        {
            // US Navy method for females
            return 163.205 * Math.Log10(input.WaistCm + input.HipCm - input.NeckCm) - 97.684 * Math.Log10(input.HeightCm) - 78.387;
        }
    }
    
    private void SaveResult(string filePath, BodyFatResult result)
    {
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

public class BodyFatInput
{
    public string Gender { get; set; }
    public int Age { get; set; }
    public double WeightKg { get; set; }
    public double HeightCm { get; set; }
    public double NeckCm { get; set; }
    public double WaistCm { get; set; }
    public double HipCm { get; set; }
}

public class BodyFatResult
{
    public double BodyFatPercentage { get; set; }
    public DateTime Timestamp { get; set; }
}