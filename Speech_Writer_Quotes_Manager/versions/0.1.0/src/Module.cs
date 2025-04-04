using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SpeechWriterModule : IGeneratedModule
{
    public string Name { get; set; } = "Speech Writer Quotes Manager";
    private string quotesFilePath;
    private List<string> quotes;

    public SpeechWriterModule()
    {
        quotes = new List<string>();
    }

    public bool Main(string dataFolder)
    {
        try
        {
            Console.WriteLine("Speech Writer Quotes Manager module is running.");
            quotesFilePath = Path.Combine(dataFolder, "quotes.json");
            
            if (File.Exists(quotesFilePath))
            {
                string json = File.ReadAllText(quotesFilePath);
                quotes = JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                quotes = new List<string>();
            }

            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        AddQuote();
                        break;
                    case "2":
                        ListQuotes();
                        break;
                    case "3":
                        RemoveQuote();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

            SaveQuotes();
            Console.WriteLine("Quotes saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nSpeech Writer Quotes Manager");
        Console.WriteLine("1. Add a quote");
        Console.WriteLine("2. List all quotes");
        Console.WriteLine("3. Remove a quote");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private void AddQuote()
    {
        Console.Write("Enter the quote to add: ");
        string quote = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(quote))
        {
            quotes.Add(quote);
            Console.WriteLine("Quote added successfully.");
        }
        else
        {
            Console.WriteLine("Quote cannot be empty.");
        }
    }

    private void ListQuotes()
    {
        if (quotes.Count == 0)
        {
            Console.WriteLine("No quotes available.");
            return;
        }

        Console.WriteLine("\nList of Quotes:");
        for (int i = 0; i < quotes.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + quotes[i]);
        }
    }

    private void RemoveQuote()
    {
        ListQuotes();
        if (quotes.Count == 0)
        {
            return;
        }

        Console.Write("Enter the number of the quote to remove: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= quotes.Count)
        {
            quotes.RemoveAt(index - 1);
            Console.WriteLine("Quote removed successfully.");
        }
        else
        {
            Console.WriteLine("Invalid quote number.");
        }
    }

    private void SaveQuotes()
    {
        string json = JsonSerializer.Serialize(quotes);
        File.WriteAllText(quotesFilePath, json);
    }
}