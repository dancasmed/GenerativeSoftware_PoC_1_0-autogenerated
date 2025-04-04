using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MorseCodeModule
{
    public string Name { get; set; } = "Morse Code Encoder/Decoder";

    private readonly Dictionary<char, string> _morseCodeMap = new Dictionary<char, string>
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
        {'-', "-....-"}, {'_', "..--.-"}, {'$', "...-..-"}, {'@', ".--.-."}, {'/', "-..-."}
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
        Console.WriteLine("Morse Code Encoder/Decoder Module is running");
        Console.WriteLine("Options:");
        Console.WriteLine("1. Encode text to Morse code");
        Console.WriteLine("2. Decode Morse code to text");
        Console.WriteLine("3. Save last operation to file");
        Console.WriteLine("4. Exit module");

        string lastResult = string.Empty;
        string operationType = string.Empty;

        while (true)
        {
            Console.Write("Enter your choice (1-4): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter text to encode: ");
                    string textToEncode = Console.ReadLine().ToUpper();
                    lastResult = EncodeToMorse(textToEncode);
                    operationType = "Encode";
                    Console.WriteLine("Encoded Morse code: " + lastResult);
                    break;

                case "2":
                    Console.Write("Enter Morse code to decode (separate letters with space, words with '/'): ");
                    string morseToDecode = Console.ReadLine();
                    lastResult = DecodeFromMorse(morseToDecode);
                    operationType = "Decode";
                    Console.WriteLine("Decoded text: " + lastResult);
                    break;

                case "3":
                    if (!string.IsNullOrEmpty(lastResult))
                    {
                        string fileName = Path.Combine(dataFolder, $"morse_result_{DateTime.Now:yyyyMMddHHmmss}.txt");
                        string content = $"{operationType} result: {lastResult}";
                        File.WriteAllText(fileName, content);
                        Console.WriteLine("Result saved to " + fileName);
                    }
                    else
                    {
                        Console.WriteLine("No operation to save");
                    }
                    break;

                case "4":
                    Console.WriteLine("Exiting Morse Code Module");
                    return true;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private string EncodeToMorse(string input)
    {
        StringBuilder result = new StringBuilder();
        foreach (char c in input)
        {
            if (_morseCodeMap.TryGetValue(c, out string morseChar))
            {
                result.Append(morseChar + " ");
            }
            else
            {
                result.Append("? ");
            }
        }
        return result.ToString().Trim();
    }

    private string DecodeFromMorse(string input)
    {
        StringBuilder result = new StringBuilder();
        string[] words = input.Split(new[] { " / " }, StringSplitOptions.None);

        foreach (string word in words)
        {
            string[] letters = word.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string letter in letters)
            {
                if (_reverseMorseCodeMap.TryGetValue(letter, out char decodedChar))
                {
                    result.Append(decodedChar);
                }
                else
                {
                    result.Append('?');
                }
            }
            result.Append(' ');
        }

        return result.ToString().Trim();
    }
}