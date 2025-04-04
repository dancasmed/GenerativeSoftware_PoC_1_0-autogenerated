using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class NumberConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Number Converter Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Number Converter Module is running...");
        Console.WriteLine("This module converts decimal numbers to binary and hexadecimal.");

        string input;
        int decimalNumber;

        Console.Write("Enter a decimal number: ");
        input = Console.ReadLine();

        while (!int.TryParse(input, out decimalNumber))
        {
            Console.WriteLine("Invalid input. Please enter a valid decimal number.");
            Console.Write("Enter a decimal number: ");
            input = Console.ReadLine();
        }

        string binary = Convert.ToString(decimalNumber, 2);
        string hexadecimal = Convert.ToString(decimalNumber, 16).ToUpper();

        Console.WriteLine("Decimal: " + decimalNumber);
        Console.WriteLine("Binary: " + binary);
        Console.WriteLine("Hexadecimal: " + hexadecimal);

        string result = $"Decimal: {decimalNumber}, Binary: {binary}, Hexadecimal: {hexadecimal}";
        string filePath = Path.Combine(dataFolder, "conversion_results.json");

        try
        {
            File.WriteAllText(filePath, result);
            Console.WriteLine("Results saved to " + filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving results: " + ex.Message);
            return false;
        }
    }
}