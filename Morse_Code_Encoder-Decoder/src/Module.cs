using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MorseCodeModule : IGeneratedModule
{
    public string Name { get; set; } = "Morse Code Encoder/Decoder";

    private readonly Dictionary<char, string> _morseCodeMap = new Dictionary<char, string>()
    {
        {'A', ".-"}, {'B', "-..."}, {'C', "-.-."}, {'D', "-.."}, {'E', "."},
        {'F', "..-."}, {'G', "--."}, {'H', "...."}, {'I', ".."}, {'J', ".---"},
        {'K', "-.-"}, {'L', ".-.."}, {'M', "--"}, {'N', "-."}, {'O', "---"},
        {'P', ".--."}, {'Q', "--.-"}, {'R', ".-."}, {'S', "..."}, {'T', "-"},
        {'U', "..-"}, {'V', "...-"}, {'W', ".--"}, {'X', "-..-"}, {'Y', "-.--"},
        {'Z', "--.."}, {'0', "-----"}, {'1', ".----"}, {'2', "..---"}, {'3', "...--"},
        {'4', "....-"}, {'5', "....."}, {'6', "-...."}, {'7', "--..."}, {'8', "---.."},
        {'9', "----."}, {' ', "/"}, {'.', ".-.-.-"}, {',', "--..--"}, {'?', "..--.."},
        {'!', "-.-.--"}, {'\"', ".-..-."}, {'\'', ".----."}, {'(', "-.--."}, {')', "-.--.-"},
        {'&', ".-..."}, {':', "---..."}, {';', "-.-.-."}, {'=', "-...-"}, {'+', ".-.-."},
        {'-', "-....-"}, {'_', "..--.-"}, {'@', ".--.-."}, {'$', "...-..-"}, {'¿', "..-.-"},
        {'¡', "--...-"}
    };

    private readonly Dictionary<string, char> _reverseMorseCodeMap;

    public MorseCodeModule()
    {
        _reverseMorseCodeMap = new Dictionary<string, char>();
        foreach (var pair in _morseCodeMap)
        {
            _reverseMorseCodeMap[pair.Value] = pair.Key;
        }
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Morse Code Encoder/Decoder Module");
        Console.WriteLine("--------------------------------");
        Console.WriteLine("Options:");
        Console.WriteLine("1. Encode text to Morse code");
        Console.WriteLine("2. Decode Morse code to text");
        Console.WriteLine("3. Exit");

        while (true)
        {
            Console.Write("\nSelect an option (1-3): ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }

            if (input == "3")
            {
                Console.WriteLine("Exiting Morse Code Module...");
                return true;
            }

            if (input == "1")
            {
                Console.Write("Enter text to encode: ");
                var text = Console.ReadLine()?.ToUpper();
                if (string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("No text provided.");
                    continue;
                }

                try
                {
                    var morseCode = EncodeToMorse(text);
                    Console.WriteLine("Encoded Morse code: " + morseCode);
                    SaveToFile(dataFolder, "encoded.txt", morseCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error encoding: " + ex.Message);
                }
            }
            else if (input == "2")
            {
                Console.Write("Enter Morse code to decode (separate letters with space, words with '/'): ");
                var morseCode = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(morseCode))
                {
                    Console.WriteLine("No Morse code provided.");
                    continue;
                }

                try
                {
                    var text = DecodeFromMorse(morseCode);
                    Console.WriteLine("Decoded text: " + text);
                    SaveToFile(dataFolder, "decoded.txt", text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error decoding: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }

    private string EncodeToMorse(string text)
    {
        var result = new StringBuilder();
        foreach (char c in text)
        {
            if (_morseCodeMap.TryGetValue(c, out string morse))
            {
                result.Append(morse + " ");
            }
            else
            {
                throw new ArgumentException($"Character '{c}' cannot be encoded to Morse code.");
            }
        }
        return result.ToString().Trim();
    }

    private string DecodeFromMorse(string morseCode)
    {
        var words = morseCode.Split(new[] { " / " }, StringSplitOptions.None);
        var result = new StringBuilder();

        foreach (var word in words)
        {
            var letters = word.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var letter in letters)
            {
                if (_reverseMorseCodeMap.TryGetValue(letter, out char c))
                {
                    result.Append(c);
                }
                else
                {
                    throw new ArgumentException($"Morse code '{letter}' cannot be decoded to a character.");
                }
            }
            result.Append(' ');
        }

        return result.ToString().Trim();
    }

    private void SaveToFile(string dataFolder, string fileName, string content)
    {
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var filePath = Path.Combine(dataFolder, fileName);
            File.WriteAllText(filePath, content);
            Console.WriteLine("Result saved to: " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }
    }
}