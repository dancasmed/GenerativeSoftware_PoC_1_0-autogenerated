using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MorseCodeModule : IGeneratedModule
{
    public string Name { get; set; } = "Morse Code Encoder/Decoder";

    private readonly Dictionary<char, string> _morseCodeDictionary = new Dictionary<char, string>()
    {
        {'A', ".-"},
        {'B', "-..."},
        {'C', "-.-."},
        {'D', "-.."},
        {'E', "."},
        {'F', "..-."},
        {'G', "--."},
        {'H', "...."},
        {'I', ".."},
        {'J', ".---"},
        {'K', "-.-"},
        {'L', ".-.."},
        {'M', "--"},
        {'N', "-."},
        {'O', "---"},
        {'P', ".--."},
        {'Q', "--.-"},
        {'R', ".-."},
        {'S', "..."},
        {'T', "-"},
        {'U', "..-"},
        {'V', "...-"},
        {'W', ".--"},
        {'X', "-..-"},
        {'Y', "-.--"},
        {'Z', "--.."},
        {'0', "-----"},
        {'1', ".----"},
        {'2', "..---"},
        {'3', "...--"},
        {'4', "....-"},
        {'5', "....."},
        {'6', "-...."},
        {'7', "--..."},
        {'8', "---.."},
        {'9', "----."},
        {' ', "/"}
    };

    private readonly Dictionary<string, char> _reverseMorseCodeDictionary;

    public MorseCodeModule()
    {
        _reverseMorseCodeDictionary = new Dictionary<string, char>();
        foreach (var pair in _morseCodeDictionary)
        {
            _reverseMorseCodeDictionary[pair.Value] = pair.Key;
        }
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Morse Code Encoder/Decoder Module is running.");
        Console.WriteLine("Options:");
        Console.WriteLine("1. Encode text to Morse code");
        Console.WriteLine("2. Decode Morse code to text");
        Console.WriteLine("Enter your choice (1 or 2):");

        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.WriteLine("Enter text to encode:");
            string text = Console.ReadLine().ToUpper();
            string morseCode = EncodeToMorse(text);
            Console.WriteLine("Encoded Morse code: " + morseCode);
            SaveToFile(dataFolder, "encoded.txt", morseCode);
        }
        else if (choice == "2")
        {
            Console.WriteLine("Enter Morse code to decode (separate letters with space, words with '/'):");
            string morseCode = Console.ReadLine();
            string text = DecodeFromMorse(morseCode);
            Console.WriteLine("Decoded text: " + text);
            SaveToFile(dataFolder, "decoded.txt", text);
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            return false;
        }

        return true;
    }

    private string EncodeToMorse(string text)
    {
        StringBuilder morseCode = new StringBuilder();

        foreach (char c in text)
        {
            if (_morseCodeDictionary.TryGetValue(c, out string morse))
            {
                morseCode.Append(morse + " ");
            }
        }

        return morseCode.ToString().Trim();
    }

    private string DecodeFromMorse(string morseCode)
    {
        StringBuilder text = new StringBuilder();
        string[] letters = morseCode.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string letter in letters)
        {
            if (_reverseMorseCodeDictionary.TryGetValue(letter, out char c))
            {
                text.Append(c);
            }
            else if (letter == "/")
            {
                text.Append(' ');
            }
        }

        return text.ToString();
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