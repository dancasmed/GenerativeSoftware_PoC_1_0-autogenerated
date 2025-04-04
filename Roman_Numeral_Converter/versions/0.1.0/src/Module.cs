using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RomanNumeralConverter : IGeneratedModule
{
    public string Name { get; set; } = "Roman Numeral Converter";

    private Dictionary<char, int> romanToIntMap = new Dictionary<char, int>
    {
        {'I', 1},
        {'V', 5},
        {'X', 10},
        {'L', 50},
        {'C', 100},
        {'D', 500},
        {'M', 1000}
    };

    private List<(int Value, string Symbol)> intToRomanMap = new List<(int, string)>
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

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Roman Numeral Converter module started.");
        Console.WriteLine("This module converts between Roman numerals and integers.");

        string configPath = Path.Combine(dataFolder, "converter_config.json");
        LoadConfiguration(configPath);

        while (true)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Convert Roman numeral to integer");
            Console.WriteLine("2. Convert integer to Roman numeral");
            Console.WriteLine("3. Exit");

            string input = Console.ReadLine();

            if (input == "3")
            {
                SaveConfiguration(configPath);
                Console.WriteLine("Exiting Roman Numeral Converter module.");
                return true;
            }

            switch (input)
            {
                case "1":
                    Console.Write("Enter Roman numeral: ");
                    string romanInput = Console.ReadLine().Trim().ToUpper();
                    try
                    {
                        int result = RomanToInt(romanInput);
                        Console.WriteLine("Integer value: " + result);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    break;

                case "2":
                    Console.Write("Enter integer (1-3999): ");
                    if (int.TryParse(Console.ReadLine(), out int intInput))
                    {
                        try
                        {
                            string romanResult = IntToRoman(intInput);
                            Console.WriteLine("Roman numeral: " + romanResult);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid integer input.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private int RomanToInt(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException("Input cannot be empty.");

        int total = 0;
        int previousValue = 0;

        for (int i = s.Length - 1; i >= 0; i--)
        {
            if (!romanToIntMap.TryGetValue(s[i], out int currentValue))
                throw new ArgumentException("Invalid Roman numeral character: " + s[i]);

            if (currentValue < previousValue)
                total -= currentValue;
            else
                total += currentValue;

            previousValue = currentValue;
        }

        // Validate the Roman numeral format by converting back
        if (IntToRoman(total) != s)
            throw new ArgumentException("Invalid Roman numeral format.");

        return total;
    }

    private string IntToRoman(int num)
    {
        if (num < 1 || num > 3999)
            throw new ArgumentException("Number must be between 1 and 3999.");

        StringBuilder roman = new StringBuilder();

        foreach (var (value, symbol) in intToRomanMap)
        {
            while (num >= value)
            {
                roman.Append(symbol);
                num -= value;
            }
        }

        return roman.ToString();
    }

    private void LoadConfiguration(string configPath)
    {
        try
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<ConverterConfig>(json);
                Console.WriteLine("Configuration loaded successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading configuration: " + ex.Message);
        }
    }

    private void SaveConfiguration(string configPath)
    {
        try
        {
            var config = new ConverterConfig { LastRun = DateTime.Now };
            string json = JsonSerializer.Serialize(config);
            File.WriteAllText(configPath, json);
            Console.WriteLine("Configuration saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving configuration: " + ex.Message);
        }
    }

    private class ConverterConfig
    {
        public DateTime LastRun { get; set; }
    }
}