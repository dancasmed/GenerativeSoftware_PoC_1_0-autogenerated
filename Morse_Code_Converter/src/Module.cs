using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MorseCodeConverter : IGeneratedModule
{
    public string Name { get; set; } = "Morse Code Converter";

    private readonly Dictionary<char, string> _textToMorse = new Dictionary<char, string>()
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
        {' ', " "}
    };

    private readonly Dictionary<string, char> _morseToText;

    public MorseCodeConverter()
    {
        _morseToText = new Dictionary<string, char>();
        foreach (var pair in _textToMorse)
        {
            _morseToText[pair.Value] = pair.Key;
        }
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Morse Code Converter module is running.");
        Console.WriteLine("Enter '1' to convert text to Morse code or '2' to convert Morse code to text:");
        
        string input = Console.ReadLine();
        
        if (input == "1")
        {
            Console.WriteLine("Enter the text to convert to Morse code:");
            string text = Console.ReadLine().ToUpper();
            string morseCode = ConvertTextToMorse(text);
            Console.WriteLine("Morse code: " + morseCode);
            
            string filePath = Path.Combine(dataFolder, "morse_code_output.txt");
            File.WriteAllText(filePath, morseCode);
            Console.WriteLine("Result saved to " + filePath);
        }
        else if (input == "2")
        {
            Console.WriteLine("Enter the Morse code to convert to text (separate letters with spaces and words with ' / '):");
            string morseCode = Console.ReadLine();
            string text = ConvertMorseToText(morseCode);
            Console.WriteLine("Text: " + text);
            
            string filePath = Path.Combine(dataFolder, "text_output.txt");
            File.WriteAllText(filePath, text);
            Console.WriteLine("Result saved to " + filePath);
        }
        else
        {
            Console.WriteLine("Invalid input.");
            return false;
        }
        
        return true;
    }

    private string ConvertTextToMorse(string text)
    {
        StringBuilder morseCode = new StringBuilder();
        
        foreach (char c in text)
        {
            if (_textToMorse.TryGetValue(c, out string morse))
            {
                morseCode.Append(morse + " ");
            }
            else
            {
                morseCode.Append("? ");
            }
        }
        
        return morseCode.ToString().Trim();
    }

    private string ConvertMorseToText(string morseCode)
    {
        StringBuilder text = new StringBuilder();
        string[] words = morseCode.Split(new[] { " / " }, StringSplitOptions.None);
        
        foreach (string word in words)
        {
            string[] letters = word.Split(' ');
            
            foreach (string letter in letters)
            {
                if (_morseToText.TryGetValue(letter, out char c))
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
}