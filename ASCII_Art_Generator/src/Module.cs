using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;

public class AsciiArtGenerator : IGeneratedModule
{
    public string Name { get; set; } = "ASCII Art Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("ASCII Art Generator Module is running.");
        Console.WriteLine("Enter text to convert to ASCII art:");
        string inputText = Console.ReadLine();

        if (string.IsNullOrEmpty(inputText))
        {
            Console.WriteLine("No input text provided.");
            return false;
        }

        string asciiArt = GenerateAsciiArt(inputText);
        Console.WriteLine("Generated ASCII Art:");
        Console.WriteLine(asciiArt);

        string outputPath = Path.Combine(dataFolder, "ascii_art.txt");
        File.WriteAllText(outputPath, asciiArt);
        Console.WriteLine("ASCII art saved to " + outputPath);

        return true;
    }

    private string GenerateAsciiArt(string text)
    {
        StringBuilder asciiArt = new StringBuilder();

        foreach (char c in text.ToUpper())
        {
            switch (c)
            {
                case 'A':
                    asciiArt.AppendLine("  A  ");
                    asciiArt.AppendLine(" A A ");
                    asciiArt.AppendLine("AAAAA");
                    asciiArt.AppendLine("A   A");
                    asciiArt.AppendLine("A   A");
                    break;
                case 'B':
                    asciiArt.AppendLine("BBBB ");
                    asciiArt.AppendLine("B   B");
                    asciiArt.AppendLine("BBBB ");
                    asciiArt.AppendLine("B   B");
                    asciiArt.AppendLine("BBBB ");
                    break;
                case 'C':
                    asciiArt.AppendLine(" CCCC");
                    asciiArt.AppendLine("C    ");
                    asciiArt.AppendLine("C    ");
                    asciiArt.AppendLine("C    ");
                    asciiArt.AppendLine(" CCCC");
                    break;
                case 'D':
                    asciiArt.AppendLine("DDDD ");
                    asciiArt.AppendLine("D   D");
                    asciiArt.AppendLine("D   D");
                    asciiArt.AppendLine("D   D");
                    asciiArt.AppendLine("DDDD ");
                    break;
                case 'E':
                    asciiArt.AppendLine("EEEEE");
                    asciiArt.AppendLine("E    ");
                    asciiArt.AppendLine("EEE  ");
                    asciiArt.AppendLine("E    ");
                    asciiArt.AppendLine("EEEEE");
                    break;
                case 'F':
                    asciiArt.AppendLine("FFFFF");
                    asciiArt.AppendLine("F    ");
                    asciiArt.AppendLine("FFF  ");
                    asciiArt.AppendLine("F    ");
                    asciiArt.AppendLine("F    ");
                    break;
                case 'G':
                    asciiArt.AppendLine(" GGGG");
                    asciiArt.AppendLine("G    ");
                    asciiArt.AppendLine("G  GG");
                    asciiArt.AppendLine("G   G");
                    asciiArt.AppendLine(" GGGG");
                    break;
                case 'H':
                    asciiArt.AppendLine("H   H");
                    asciiArt.AppendLine("H   H");
                    asciiArt.AppendLine("HHHHH");
                    asciiArt.AppendLine("H   H");
                    asciiArt.AppendLine("H   H");
                    break;
                case 'I':
                    asciiArt.AppendLine("IIIII");
                    asciiArt.AppendLine("  I  ");
                    asciiArt.AppendLine("  I  ");
                    asciiArt.AppendLine("  I  ");
                    asciiArt.AppendLine("IIIII");
                    break;
                case 'J':
                    asciiArt.AppendLine("    J");
                    asciiArt.AppendLine("    J");
                    asciiArt.AppendLine("    J");
                    asciiArt.AppendLine("J   J");
                    asciiArt.AppendLine(" JJJ ");
                    break;
                case 'K':
                    asciiArt.AppendLine("K   K");
                    asciiArt.AppendLine("K  K ");
                    asciiArt.AppendLine("KKK  ");
                    asciiArt.AppendLine("K  K ");
                    asciiArt.AppendLine("K   K");
                    break;
                case 'L':
                    asciiArt.AppendLine("L    ");
                    asciiArt.AppendLine("L    ");
                    asciiArt.AppendLine("L    ");
                    asciiArt.AppendLine("L    ");
                    asciiArt.AppendLine("LLLLL");
                    break;
                case 'M':
                    asciiArt.AppendLine("M   M");
                    asciiArt.AppendLine("MM MM");
                    asciiArt.AppendLine("M M M");
                    asciiArt.AppendLine("M   M");
                    asciiArt.AppendLine("M   M");
                    break;
                case 'N':
                    asciiArt.AppendLine("N   N");
                    asciiArt.AppendLine("NN  N");
                    asciiArt.AppendLine("N N N");
                    asciiArt.AppendLine("N  NN");
                    asciiArt.AppendLine("N   N");
                    break;
                case 'O':
                    asciiArt.AppendLine(" OOO ");
                    asciiArt.AppendLine("O   O");
                    asciiArt.AppendLine("O   O");
                    asciiArt.AppendLine("O   O");
                    asciiArt.AppendLine(" OOO ");
                    break;
                case 'P':
                    asciiArt.AppendLine("PPPP ");
                    asciiArt.AppendLine("P   P");
                    asciiArt.AppendLine("PPPP ");
                    asciiArt.AppendLine("P    ");
                    asciiArt.AppendLine("P    ");
                    break;
                case 'Q':
                    asciiArt.AppendLine(" QQQ ");
                    asciiArt.AppendLine("Q   Q");
                    asciiArt.AppendLine("Q   Q");
                    asciiArt.AppendLine("Q  QQ");
                    asciiArt.AppendLine(" QQQQ");
                    break;
                case 'R':
                    asciiArt.AppendLine("RRRR ");
                    asciiArt.AppendLine("R   R");
                    asciiArt.AppendLine("RRRR ");
                    asciiArt.AppendLine("R R  ");
                    asciiArt.AppendLine("R  RR");
                    break;
                case 'S':
                    asciiArt.AppendLine(" SSS ");
                    asciiArt.AppendLine("S    ");
                    asciiArt.AppendLine(" SSS ");
                    asciiArt.AppendLine("    S");
                    asciiArt.AppendLine("SSSS ");
                    break;
                case 'T':
                    asciiArt.AppendLine("TTTTT");
                    asciiArt.AppendLine("  T  ");
                    asciiArt.AppendLine("  T  ");
                    asciiArt.AppendLine("  T  ");
                    asciiArt.AppendLine("  T  ");
                    break;
                case 'U':
                    asciiArt.AppendLine("U   U");
                    asciiArt.AppendLine("U   U");
                    asciiArt.AppendLine("U   U");
                    asciiArt.AppendLine("U   U");
                    asciiArt.AppendLine(" UUU ");
                    break;
                case 'V':
                    asciiArt.AppendLine("V   V");
                    asciiArt.AppendLine("V   V");
                    asciiArt.AppendLine("V   V");
                    asciiArt.AppendLine(" V V ");
                    asciiArt.AppendLine("  V  ");
                    break;
                case 'W':
                    asciiArt.AppendLine("W   W");
                    asciiArt.AppendLine("W   W");
                    asciiArt.AppendLine("W W W");
                    asciiArt.AppendLine("WW WW");
                    asciiArt.AppendLine("W   W");
                    break;
                case 'X':
                    asciiArt.AppendLine("X   X");
                    asciiArt.AppendLine(" X X ");
                    asciiArt.AppendLine("  X  ");
                    asciiArt.AppendLine(" X X ");
                    asciiArt.AppendLine("X   X");
                    break;
                case 'Y':
                    asciiArt.AppendLine("Y   Y");
                    asciiArt.AppendLine(" Y Y ");
                    asciiArt.AppendLine("  Y  ");
                    asciiArt.AppendLine("  Y  ");
                    asciiArt.AppendLine("  Y  ");
                    break;
                case 'Z':
                    asciiArt.AppendLine("ZZZZZ");
                    asciiArt.AppendLine("   Z ");
                    asciiArt.AppendLine("  Z  ");
                    asciiArt.AppendLine(" Z   ");
                    asciiArt.AppendLine("ZZZZZ");
                    break;
                case ' ':
                    asciiArt.AppendLine("     ");
                    asciiArt.AppendLine("     ");
                    asciiArt.AppendLine("     ");
                    asciiArt.AppendLine("     ");
                    asciiArt.AppendLine("     ");
                    break;
                default:
                    asciiArt.AppendLine("     ");
                    asciiArt.AppendLine("  ?  ");
                    asciiArt.AppendLine("     ");
                    asciiArt.AppendLine("  ?  ");
                    asciiArt.AppendLine("     ");
                    break;
            }
            asciiArt.AppendLine();
        }

        return asciiArt.ToString();
    }
}