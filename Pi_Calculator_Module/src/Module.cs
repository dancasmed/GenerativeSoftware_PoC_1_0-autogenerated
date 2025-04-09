using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class PiCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Pi Calculator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Pi Calculator Module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "pi_config.json");
            int decimalPlaces = 10; // Default value
            
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<PiConfig>(json);
                decimalPlaces = config?.DecimalPlaces ?? decimalPlaces;
            }
            else
            {
                var defaultConfig = new PiConfig { DecimalPlaces = decimalPlaces };
                string defaultJson = JsonSerializer.Serialize(defaultConfig);
                File.WriteAllText(configPath, defaultJson);
            }
            
            string piValue = CalculatePi(decimalPlaces);
            Console.WriteLine("Calculated PI with " + decimalPlaces + " decimal places: " + piValue);
            
            string resultPath = Path.Combine(dataFolder, "pi_result.json");
            var result = new PiResult { Value = piValue, CalculatedAt = DateTime.Now };
            string resultJson = JsonSerializer.Serialize(result);
            File.WriteAllText(resultPath, resultJson);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private string CalculatePi(int decimalPlaces)
    {
        // Using Bailey–Borwein–Plouffe formula for demonstration
        // Note: This is a simplified approximation and not suitable for high precision
        double pi = 0.0;
        for (int k = 0; k < decimalPlaces + 1; k++)
        {
            double term = (1.0 / Math.Pow(16, k)) * 
                          (4.0 / (8 * k + 1) - 
                           2.0 / (8 * k + 4) - 
                           1.0 / (8 * k + 5) - 
                           1.0 / (8 * k + 6));
            pi += term;
        }
        
        return pi.ToString("F" + decimalPlaces);
    }
}

public class PiConfig
{
    public int DecimalPlaces { get; set; }
}

public class PiResult
{
    public string Value { get; set; }
    public DateTime CalculatedAt { get; set; }
}