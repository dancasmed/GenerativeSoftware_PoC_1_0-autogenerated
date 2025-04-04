using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class NumberConverter : IGeneratedModule
{
    public string Name { get; set; } = "Number Converter";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Number Converter Module is running.");
        Console.WriteLine("This module converts decimal numbers to binary and hexadecimal.");

        try
        {
            string inputFile = Path.Combine(dataFolder, "input.txt");
            string outputFile = Path.Combine(dataFolder, "output.txt");

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Input file not found. Creating a sample input file.");
                File.WriteAllText(inputFile, "10\n255\n16");
            }

            string[] lines = File.ReadAllLines(inputFile);
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                foreach (string line in lines)
                {
                    if (int.TryParse(line, out int number))
                    {
                        string binary = Convert.ToString(number, 2);
                        string hexadecimal = Convert.ToString(number, 16).ToUpper();
                        writer.WriteLine($"Decimal: {number}, Binary: {binary}, Hexadecimal: {hexadecimal}");
                    }
                }
            }

            Console.WriteLine("Conversion completed. Results saved to output.txt.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}