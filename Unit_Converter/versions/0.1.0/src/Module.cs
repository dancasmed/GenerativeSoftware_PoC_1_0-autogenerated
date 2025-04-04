using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class UnitConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Unit Converter";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Unit Converter Module is running.");
        Console.WriteLine("Available units: kilometers (km), miles (mi), meters (m), feet (ft)");

        try
        {
            string inputUnit = GetInput("Enter the source unit (km, mi, m, ft): ");
            string outputUnit = GetInput("Enter the target unit (km, mi, m, ft): ");
            double value = GetDoubleInput("Enter the value to convert: ");

            double result = ConvertUnit(value, inputUnit, outputUnit);
            Console.WriteLine(string.Format("{0} {1} is equal to {2} {3}", value, inputUnit, result, outputUnit));

            SaveConversionHistory(dataFolder, value, inputUnit, result, outputUnit);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("An error occurred: {0}", ex.Message));
            return false;
        }
    }

    private string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine().Trim().ToLower();
    }

    private double GetDoubleInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out double result))
                return result;
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
    }

    private double ConvertUnit(double value, string fromUnit, string toUnit)
    {
        double meters = 0;

        switch (fromUnit)
        {
            case "km":
                meters = value * 1000;
                break;
            case "mi":
                meters = value * 1609.344;
                break;
            case "m":
                meters = value;
                break;
            case "ft":
                meters = value * 0.3048;
                break;
            default:
                throw new ArgumentException("Invalid source unit");
        }

        switch (toUnit)
        {
            case "km":
                return meters / 1000;
            case "mi":
                return meters / 1609.344;
            case "m":
                return meters;
            case "ft":
                return meters / 0.3048;
            default:
                throw new ArgumentException("Invalid target unit");
        }
    }

    private void SaveConversionHistory(string dataFolder, double inputValue, string inputUnit, double outputValue, string outputUnit)
    {
        try
        {
            var history = new
            {
                Timestamp = DateTime.Now,
                InputValue = inputValue,
                InputUnit = inputUnit,
                OutputValue = outputValue,
                OutputUnit = outputUnit
            };

            string historyPath = Path.Combine(dataFolder, "conversion_history.json");
            string json = JsonSerializer.Serialize(history);
            File.AppendAllText(historyPath, json + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("Failed to save conversion history: {0}", ex.Message));
        }
    }
}