using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TemperatureConverter : IGeneratedModule
{
    public string Name { get; set; } = "Temperature Converter";

    private string dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Temperature Converter module is running.");
        
        dataFilePath = Path.Combine(dataFolder, "temperature_conversion_history.json");
        
        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Convert Celsius to Fahrenheit");
            Console.WriteLine("2. Convert Fahrenheit to Celsius");
            Console.WriteLine("3. Convert Celsius to Kelvin");
            Console.WriteLine("4. Convert Kelvin to Celsius");
            Console.WriteLine("5. Convert Fahrenheit to Kelvin");
            Console.WriteLine("6. Convert Kelvin to Fahrenheit");
            Console.WriteLine("7. View conversion history");
            Console.WriteLine("8. Exit");
            
            Console.Write("Enter your choice: ");
            string input = Console.ReadLine();
            
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > 8)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                continue;
            }
            
            if (choice == 8)
            {
                Console.WriteLine("Exiting Temperature Converter.");
                return true;
            }
            
            if (choice == 7)
            {
                DisplayConversionHistory();
                continue;
            }
            
            Console.Write("Enter temperature value: ");
            input = Console.ReadLine();
            
            if (!double.TryParse(input, out double temperature))
            {
                Console.WriteLine("Invalid temperature value. Please try again.");
                continue;
            }
            
            double result = 0;
            string conversionType = "";
            
            switch (choice)
            {
                case 1:
                    result = CelsiusToFahrenheit(temperature);
                    conversionType = "Celsius to Fahrenheit";
                    Console.WriteLine($"{temperature}°C is {result}°F");
                    break;
                case 2:
                    result = FahrenheitToCelsius(temperature);
                    conversionType = "Fahrenheit to Celsius";
                    Console.WriteLine($"{temperature}°F is {result}°C");
                    break;
                case 3:
                    result = CelsiusToKelvin(temperature);
                    conversionType = "Celsius to Kelvin";
                    Console.WriteLine($"{temperature}°C is {result}K");
                    break;
                case 4:
                    result = KelvinToCelsius(temperature);
                    conversionType = "Kelvin to Celsius";
                    Console.WriteLine($"{temperature}K is {result}°C");
                    break;
                case 5:
                    result = FahrenheitToKelvin(temperature);
                    conversionType = "Fahrenheit to Kelvin";
                    Console.WriteLine($"{temperature}°F is {result}K");
                    break;
                case 6:
                    result = KelvinToFahrenheit(temperature);
                    conversionType = "Kelvin to Fahrenheit";
                    Console.WriteLine($"{temperature}K is {result}°F");
                    break;
            }
            
            SaveConversionHistory(conversionType, temperature, result);
        }
    }
    
    private double CelsiusToFahrenheit(double celsius)
    {
        return (celsius * 9 / 5) + 32;
    }
    
    private double FahrenheitToCelsius(double fahrenheit)
    {
        return (fahrenheit - 32) * 5 / 9;
    }
    
    private double CelsiusToKelvin(double celsius)
    {
        return celsius + 273.15;
    }
    
    private double KelvinToCelsius(double kelvin)
    {
        return kelvin - 273.15;
    }
    
    private double FahrenheitToKelvin(double fahrenheit)
    {
        return CelsiusToKelvin(FahrenheitToCelsius(fahrenheit));
    }
    
    private double KelvinToFahrenheit(double kelvin)
    {
        return CelsiusToFahrenheit(KelvinToCelsius(kelvin));
    }
    
    private void SaveConversionHistory(string conversionType, double inputValue, double resultValue)
    {
        try
        {
            ConversionRecord record = new ConversionRecord
            {
                ConversionType = conversionType,
                InputValue = inputValue,
                ResultValue = resultValue,
                Timestamp = DateTime.Now
            };
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(record, options);
            
            File.AppendAllText(dataFilePath, jsonString + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving conversion history: {ex.Message}");
        }
    }
    
    private void DisplayConversionHistory()
    {
        try
        {
            if (!File.Exists(dataFilePath))
            {
                Console.WriteLine("No conversion history available.");
                return;
            }
            
            Console.WriteLine("\nConversion History:");
            string[] lines = File.ReadAllLines(dataFilePath);
            
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        var record = JsonSerializer.Deserialize<ConversionRecord>(line);
                        Console.WriteLine($"{record.Timestamp}: {record.ConversionType} - {record.InputValue} => {record.ResultValue}");
                    }
                    catch
                    {
                        Console.WriteLine("Invalid record format.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading conversion history: {ex.Message}");
        }
    }
    
    private class ConversionRecord
    {
        public string ConversionType { get; set; }
        public double InputValue { get; set; }
        public double ResultValue { get; set; }
        public DateTime Timestamp { get; set; }
    }
}