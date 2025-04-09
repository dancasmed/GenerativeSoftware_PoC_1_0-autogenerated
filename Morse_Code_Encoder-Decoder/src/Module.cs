using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MorseCodeModule : IGeneratedModule
{
    public string Name { get; set; } = "Morse Code Encoder/Decoder";

    private static readonly Dictionary<char, string> CharToMorse = new Dictionary<char, string>()
    {
        {'A', ".-"}, {'B', "-..."}, {'C', "-.-."}, {'D', "-.."}, {'E', "."},
        {'F', "..-."}, {'G', "--."}, {'H', "...."}, {'I', ".."}, {'J', ".---"},
        {'K', "-.-"}, {'L', ".-.."}, {'M', "--"}, {'N', "-."}, {'O', "---"},
        {'P', ".--."}, {'Q', "--.-"}, {'R', ".-."}, {'S', "..."}, {'T', "-"},
        {'U', "..-"}, {'V', "...-"}, {'W', ".--"}, {'X', "-..-"}, {'Y', "-.--"},
        {'Z', "--.."}, {'0', "-----"}, {'1', ".----"}, {'2', "..---"}, {'3', "...--"},
        {'4', "....-"}, {'5', "....."}, {'6', "-...."}, {'7', "--..."}, {'8', "---.."},
        {'9', "----."}, {' ', "/"}
    };

    private static readonly Dictionary<string, char> MorseToChar = new Dictionary<string, char>()
    {
        {".-", 'A'}, {"-...", 'B'}, {"-.-.", 'C'}, {"-..", 'D'}, {".", 'E'},
        {"..-.", 'F'}, {"--.", 'G'}, {"....", 'H'}, {"..", 'I'}, {".---", 'J'},
        {"-.-", 'K'}, {".-..", 'L'}, {"--", 'M'}, {"-.", 'N'}, {"---", 'O'},
        {".--.", 'P'}, {"--.-", 'Q'}, {".-.", 'R'}, {"...", 'S'}, {"-", 'T'},
        {"..-", 'U'}, {"...-", 'V'}, {".--", 'W'}, {"-..-", 'X'}, {"-.--", 'Y'},
        {"--..", 'Z'}, {"-----", '0'}, {".----", '1'}, {"..---", '2'}, {"...--", '3'},
        {"....-", '4'}, {".....", '5'}, {"-....", '6'}, {"--...", '7'}, {"---..", '8'},
        {"----.", '9'}, {"/", ' '}
    };

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Morse Code Encoder/Decoder Module");
        Console.WriteLine("Available operations:");
        Console.WriteLine("1. Encode text to Morse code");
        Console.WriteLine("2. Decode Morse code to text");
        Console.WriteLine("3. Exit");

        while (true)
        {
            Console.Write("Select operation (1-3): ");
            string input = Console.ReadLine();

            if (input == "3")
            {
                Console.WriteLine("Exiting Morse Code Module...");
                return true;
            }

            if (input == "1")
            {
                Console.Write("Enter text to encode: ");
                string text = Console.ReadLine().ToUpper();
                string morseCode = EncodeToMorse(text);
                Console.WriteLine("Encoded Morse code: " + morseCode);
                SaveToFile(dataFolder, "encoded.txt", morseCode);
            }
            else if (input == "2")
            {
                Console.Write("Enter Morse code to decode (separate letters with space, words with '/'): ");
                string morseCode = Console.ReadLine();
                string text = DecodeFromMorse(morseCode);
                Console.WriteLine("Decoded text: " + text);
                SaveToFile(dataFolder, "decoded.txt", text);
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }

    private string EncodeToMorse(string text)
    {
        StringBuilder morse = new StringBuilder();
        foreach (char c in text)
        {
            if (CharToMorse.TryGetValue(c, out string morseChar))
            {
                morse.Append(morseChar + " ");
            }
            else
            {
                morse.Append("? ");
            }
        }
        return morse.ToString().Trim();
    }

    private string DecodeFromMorse(string morseCode)
    {
        StringBuilder text = new StringBuilder();
        string[] morseWords = morseCode.Split(new[] { " / " }, StringSplitOptions.None);

        foreach (string word in morseWords)
        {
            string[] morseChars = word.Split(' ');
            foreach (string morseChar in morseChars)
            {
                if (MorseToChar.TryGetValue(morseChar, out char c))
                {
                    text.Append(c);
                }
                else
                {
                    text.Append('?');
                }
            }
            text.Append(' ');
        }
        return text.ToString().Trim();
    }

    private void SaveToFile(string dataFolder, string fileName, string content)
    {
        try
        {
            string filePath = Path.Combine(dataFolder, fileName);
            File.WriteAllText(filePath, content);
            Console.WriteLine("Result saved to " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }
    }
}