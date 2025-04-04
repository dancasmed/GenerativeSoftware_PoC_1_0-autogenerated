using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

public class RomanNumeralConverter : IGeneratedModule
{
    public string Name { get; set; } = "Roman Numeral Converter";

    private readonly Dictionary<char, int> romanToIntMap = new Dictionary<char, int>
    {
        {'I', 1},
        {'V', 5},
        {'X', 10},
        {'L', 50},
        {'C', 100},
        {'D', 500},
        {'M', 1000}
    };

    private readonly List<(int Value, string Symbol)> intToRomanMap = new List<(int, string)>
    {
        (1000, "M"),
        (900, "CM"),
        (500, "D"),
        (400, "CD"),
        (100, "C"),
        (90, "XC"),
        (50, "L"),
        (40, "XL"),
        (10, "X"),
        (9, "IX"),
        (5, "V"),
        (4, "IV"),
        (1, "I")
    };

    public RomanNumeralConverter() { }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Roman Numeral Converter module is running.");
        Console.WriteLine("This module converts Roman numerals to integers and vice versa.");

        string inputFilePath = Path.Combine(dataFolder, "input.json");
        string outputFilePath = Path.Combine(dataFolder, "output.json");

        try
        {
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found. Creating a sample input file.");
                CreateSampleInputFile(inputFilePath);
                Console.WriteLine("Please edit the input.json file and run the module again.");
                return false;
            }

            string jsonInput = File.ReadAllText(inputFilePath);
            var inputData = JsonSerializer.Deserialize<ConversionInput>(jsonInput);

            if (inputData == null)
            {
                Console.WriteLine("Invalid input data format.");
                return false;
            }

            var result = new ConversionResult();

            if (!string.IsNullOrEmpty(inputData.RomanNumeral))
            {
                result.IntegerValue = RomanToInt(inputData.RomanNumeral);
                Console.WriteLine("Converted Roman numeral " + inputData.RomanNumeral + " to integer: " + result.IntegerValue);
            }
            else if (inputData.IntegerValue.HasValue)
            {
                result.RomanNumeral = IntToRoman(inputData.IntegerValue.Value);
                Console.WriteLine("Converted integer " + inputData.IntegerValue + " to Roman numeral: " + result.RomanNumeral);
            }
            else
            {
                Console.WriteLine("No valid input provided in the input file.");
                return false;
            }

            string jsonOutput = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputFilePath, jsonOutput);
            Console.WriteLine("Conversion result saved to output.json");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private int RomanToInt(string roman)
    {
        if (string.IsNullOrEmpty(roman))
            throw new ArgumentException("Roman numeral cannot be null or empty.");

        roman = roman.ToUpper();
        int total = 0;
        int previousValue = 0;

        for (int i = roman.Length - 1; i >= 0; i--)
        {
            if (!romanToIntMap.TryGetValue(roman[i], out int currentValue))
                throw new ArgumentException("Invalid Roman numeral character: " + roman[i]);

            if (currentValue < previousValue)
                total -= currentValue;
            else
                total += currentValue;

            previousValue = currentValue;
        }

        return total;
    }

    private string IntToRoman(int number)
    {
        if (number < 1 || number > 3999)
            throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 3999.");

        var roman = new StringBuilder();

        foreach (var (value, symbol) in intToRomanMap)
        {
            while (number >= value)
            {
                roman.Append(symbol);
                number -= value;
            }
        }

        return roman.ToString();
    }

    private void CreateSampleInputFile(string filePath)
    {
        var sampleInput = new ConversionInput
        {
            RomanNumeral = "XIV",
            IntegerValue = null
        };

        string json = JsonSerializer.Serialize(sampleInput, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

public class ConversionInput
{
    public string RomanNumeral { get; set; }
    public int? IntegerValue { get; set; }
}

public class ConversionResult
{
    public string RomanNumeral { get; set; }
    public int? IntegerValue { get; set; }
}