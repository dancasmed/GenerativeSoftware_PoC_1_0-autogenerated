using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;

public class NumberConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Number Converter Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Number Converter Module is running.");
        Console.WriteLine("This module converts numbers between binary, decimal, and hexadecimal formats.");

        string inputFilePath = Path.Combine(dataFolder, "input.txt");
        string outputFilePath = Path.Combine(dataFolder, "output.txt");

        try
        {
            if (!File.Exists(inputFilePath))
            {
                File.WriteAllText(inputFilePath, "Enter numbers here with format (e.g., binary: 1010b, decimal: 42, hexadecimal: 0x2A)");
                Console.WriteLine("Input file created at: " + inputFilePath);
                Console.WriteLine("Please enter numbers in the input file and run the module again.");
                return false;
            }

            string[] lines = File.ReadAllLines(inputFilePath);
            if (lines.Length == 0 || lines[0].StartsWith("Enter numbers here"))
            {
                Console.WriteLine("No numbers found in the input file.");
                return false;
            }

            StringBuilder output = new StringBuilder();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string trimmedLine = line.Trim();
                string result = ConvertNumber(trimmedLine);
                output.AppendLine(result);
            }

            File.WriteAllText(outputFilePath, output.ToString());
            Console.WriteLine("Conversion completed. Results saved to: " + outputFilePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private string ConvertNumber(string input)
    {
        if (input.EndsWith("b") || input.EndsWith("B"))
        {
            string binaryStr = input.Substring(0, input.Length - 1);
            if (long.TryParse(binaryStr, System.Globalization.NumberStyles.None, null, out long binaryNumber))
            {
                long decimalNumber = Convert.ToInt64(binaryStr, 2);
                string hexNumber = "0x" + decimalNumber.ToString("X");
                return string.Format("Binary: {0}b -> Decimal: {1}, Hexadecimal: {2}", binaryStr, decimalNumber, hexNumber);
            }
        }
        else if (input.StartsWith("0x") || input.StartsWith("0X"))
        {
            string hexStr = input.Substring(2);
            if (long.TryParse(hexStr, System.Globalization.NumberStyles.HexNumber, null, out long hexNumber))
            {
                string binaryNumber = Convert.ToString(hexNumber, 2);
                return string.Format("Hexadecimal: {0} -> Decimal: {1}, Binary: {2}b", input, hexNumber, binaryNumber);
            }
        }
        else
        {
            if (long.TryParse(input, out long decimalNumber))
            {
                string binaryNumber = Convert.ToString(decimalNumber, 2);
                string hexNumber = "0x" + decimalNumber.ToString("X");
                return string.Format("Decimal: {0} -> Binary: {1}b, Hexadecimal: {2}", decimalNumber, binaryNumber, hexNumber);
            }
        }

        return string.Format("Invalid number format: {0}", input);
    }
}