using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpellCheckerModule : IGeneratedModule
{
    public string Name { get; set; } = "Spell Checker Module";
    private HashSet<string> dictionary;

    public SpellCheckerModule()
    {
        dictionary = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Spell Checker Module is running...");
        
        string dictionaryPath = Path.Combine(dataFolder, "dictionary.txt");
        if (!File.Exists(dictionaryPath))
        {
            Console.WriteLine("Dictionary file not found. Creating a default dictionary.");
            CreateDefaultDictionary(dictionaryPath);
        }

        LoadDictionary(dictionaryPath);
        
        Console.WriteLine("Enter text to check spelling (press Enter twice to finish):");
        string input = ReadMultiLineInput();
        
        var misspelledWords = CheckSpelling(input);
        
        if (misspelledWords.Count == 0)
        {
            Console.WriteLine("No spelling errors found.");
        }
        else
        {
            Console.WriteLine("Misspelled words:");
            foreach (var word in misspelledWords)
            {
                Console.WriteLine(word);
            }
        }
        
        return true;
    }

    private void CreateDefaultDictionary(string path)
    {
        string[] defaultWords = new[]
        {
            "the", "and", "for", "are", "but", "not", "you", "all", "any", "can",
            "her", "was", "one", "our", "out", "day", "get", "has", "him", "his",
            "how", "man", "new", "now", "old", "see", "two", "way", "who", "boy",
            "did", "its", "let", "put", "say", "she", "too", "use"
        };
        
        File.WriteAllLines(path, defaultWords);
    }

    private void LoadDictionary(string path)
    {
        var words = File.ReadAllLines(path);
        foreach (var word in words)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                dictionary.Add(word.Trim());
            }
        }
    }

    private string ReadMultiLineInput()
    {
        string input = "";
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            input += line + "\n";
        }
        return input.Trim();
    }

    private List<string> CheckSpelling(string text)
    {
        var misspelled = new List<string>();
        var punctuation = text.Where(char.IsPunctuation).Distinct().ToArray();
        var words = text.Split().Select(x => x.Trim(punctuation));
        
        foreach (var word in words)
        {
            if (!string.IsNullOrWhiteSpace(word) && !dictionary.Contains(word))
            {
                misspelled.Add(word);
            }
        }
        
        return misspelled.Distinct().ToList();
    }
}