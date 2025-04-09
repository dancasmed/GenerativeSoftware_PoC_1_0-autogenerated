using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BasicChatbot : IGeneratedModule
{
    public string Name { get; set; } = "BasicChatbot";
    private Dictionary<string, string> responses;
    private string responsesFilePath;

    public BasicChatbot()
    {
        responses = new Dictionary<string, string>()
        {
            { "hello", "Hi there! How can I help you?" },
            { "how are you", "I'm just a program, but I'm doing great!" },
            { "bye", "Goodbye! Have a nice day!" },
            { "thanks", "You're welcome!" }
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Basic Chatbot module is running.");
        responsesFilePath = Path.Combine(dataFolder, "responses.json");

        LoadResponses();
        RunChatbot();
        SaveResponses();

        return true;
    }

    private void LoadResponses()
    {
        if (File.Exists(responsesFilePath))
        {
            try
            {
                string json = File.ReadAllText(responsesFilePath);
                responses = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                Console.WriteLine("Responses loaded from file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading responses: " + ex.Message);
            }
        }
    }

    private void SaveResponses()
    {
        try
        {
            string json = JsonSerializer.Serialize(responses);
            File.WriteAllText(responsesFilePath, json);
            Console.WriteLine("Responses saved to file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving responses: " + ex.Message);
        }
    }

    private void RunChatbot()
    {
        Console.WriteLine("Type your message or 'exit' to quit.");
        string input;
        
        do
        {
            Console.Write("You: ");
            input = Console.ReadLine()?.ToLower().Trim();
            
            if (input == "exit")
                break;
                
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Please enter a valid message.");
                continue;
            }
            
            if (responses.TryGetValue(input, out string response))
            {
                Console.WriteLine("Bot: " + response);
            }
            else
            {
                Console.WriteLine("Bot: I don't understand that. Can you try something else?");
            }
        } while (true);
    }
}