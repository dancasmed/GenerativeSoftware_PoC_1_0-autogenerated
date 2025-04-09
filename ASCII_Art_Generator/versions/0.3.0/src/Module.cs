using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;

public class AsciiArtGenerator : IGeneratedModule
{
    public string Name { get; set; } = "ASCII Art Generator";

    private Dictionary<char, string[]> _asciiArtDictionary;

    public AsciiArtGenerator()
    {
        InitializeAsciiArtDictionary();
    }

    private void InitializeAsciiArtDictionary()
    {
        _asciiArtDictionary = new Dictionary<char, string[]>()
        {
            {'A', new string[] {
                "  A  ",
                " A A ",
                "AAAAA",
                "A   A",
                "A   A"}},
            {'B', new string[] {
                "BBBB ",
                "B   B",
                "BBBB ",
                "B   B",
                "BBBB "}},
            {'C', new string[] {
                " CCCC",
                "C    ",
                "C    ",
                "C    ",
                " CCCC"}},
            {'D', new string[] {
                "DDDD ",
                "D   D",
                "D   D",
                "D   D",
                "DDDD "}},
            {'E', new string[] {
                "EEEEE",
                "E    ",
                "EEEEE",
                "E    ",
                "EEEEE"}},
            {'F', new string[] {
                "FFFFF",
                "F    ",
                "FFFF ",
                "F    ",
                "F    "}},
            {'G', new string[] {
                " GGGG",
                "G    ",
                "G  GG",
                "G   G",
                " GGGG"}},
            {'H', new string[] {
                "H   H",
                "H   H",
                "HHHHH",
                "H   H",
                "H   H"}},
            {'I', new string[] {
                "IIIII",
                "  I  ",
                "  I  ",
                "  I  ",
                "IIIII"}},
            {'J', new string[] {
                "    J",
                "    J",
                "    J",
                "J   J",
                " JJJ "}},
            {'K', new string[] {
                "K   K",
                "K  K ",
                "KKK  ",
                "K  K ",
                "K   K"}},
            {'L', new string[] {
                "L    ",
                "L    ",
                "L    ",
                "L    ",
                "LLLLL"}},
            {'M', new string[] {
                "M   M",
                "MM MM",
                "M M M",
                "M   M",
                "M   M"}},
            {'N', new string[] {
                "N   N",
                "NN  N",
                "N N N",
                "N  NN",
                "N   N"}},
            {'O', new string[] {
                " OOO ",
                "O   O",
                "O   O",
                "O   O",
                " OOO "}},
            {'P', new string[] {
                "PPPP ",
                "P   P",
                "PPPP ",
                "P    ",
                "P    "}},
            {'Q', new string[] {
                " QQQ ",
                "Q   Q",
                "Q   Q",
                "Q  Q ",
                " QQ Q"}},
            {'R', new string[] {
                "RRRR ",
                "R   R",
                "RRRR ",
                "R  R ",
                "R   R"}},
            {'S', new string[] {
                " SSSS",
                "S    ",
                " SSS ",
                "    S",
                "SSSS "}},
            {'T', new string[] {
                "TTTTT",
                "  T  ",
                "  T  ",
                "  T  ",
                "  T  "}},
            {'U', new string[] {
                "U   U",
                "U   U",
                "U   U",
                "U   U",
                " UUU "}},
            {'V', new string[] {
                "V   V",
                "V   V",
                "V   V",
                " V V ",
                "  V  "}},
            {'W', new string[] {
                "W   W",
                "W   W",
                "W W W",
                "WW WW",
                "W   W"}},
            {'X', new string[] {
                "X   X",
                " X X ",
                "  X  ",
                " X X ",
                "X   X"}},
            {'Y', new string[] {
                "Y   Y",
                " Y Y ",
                "  Y  ",
                "  Y  ",
                "  Y  "}},
            {'Z', new string[] {
                "ZZZZZ",
                "   Z ",
                "  Z  ",
                " Z   ",
                "ZZZZZ"}},
            {' ', new string[] {
                "     ",
                "     ",
                "     ",
                "     ",
                "     "}}
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("ASCII Art Generator Module");
        Console.WriteLine("Enter text to convert to ASCII art (letters A-Z and space only):");
        
        string input = Console.ReadLine()?.ToUpper() ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("No input provided.");
            return false;
        }
        
        foreach (char c in input)
        {
            if (!_asciiArtDictionary.ContainsKey(c))
            {
                Console.WriteLine("Invalid character detected. Only A-Z and space are supported.");
                return false;
            }
        }
        
        Console.WriteLine("Generated ASCII art:");
        
        for (int i = 0; i < 5; i++)
        {
            foreach (char c in input)
            {
                Console.Write(_asciiArtDictionary[c][i]);
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        
        string outputPath = Path.Combine(dataFolder, "ascii_art.txt");
        try
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                for (int i = 0; i < 5; i++)
                {
                    foreach (char c in input)
                    {
                        writer.Write(_asciiArtDictionary[c][i]);
                        writer.Write(" ");
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine("ASCII art saved to " + outputPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving ASCII art: " + ex.Message);
            return false;
        }
        
        return true;
    }
}