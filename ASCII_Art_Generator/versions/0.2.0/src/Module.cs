using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;

public class AsciiArtGenerator : IGeneratedModule
{
    public string Name { get; set; } = "ASCII Art Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting ASCII Art Generator...");
        Console.WriteLine("This module generates text-based image art using ASCII characters.");

        try
        {
            string outputPath = Path.Combine(dataFolder, "ascii_art.txt");
            string asciiArt = GenerateAsciiArt();
            File.WriteAllText(outputPath, asciiArt);

            Console.WriteLine("ASCII art has been generated and saved to: " + outputPath);
            Console.WriteLine("Here is a preview of the generated art:");
            Console.WriteLine(asciiArt);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating ASCII art: " + ex.Message);
            return false;
        }
    }

    private string GenerateAsciiArt()
    {
        StringBuilder sb = new StringBuilder();

        // Simple smiley face as an example
        sb.AppendLine("  *****  ");
        sb.AppendLine(" *     * ");
        sb.AppendLine("*  O O  *");
        sb.AppendLine("*   ^   *");
        sb.AppendLine("* \___/ *");
        sb.AppendLine(" *     * ");
        sb.AppendLine("  *****  ");

        return sb.ToString();
    }
}